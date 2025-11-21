using Asp.Versioning;
using InMemoriam.Core.Interfaces;
using InMemoriam.Core.Services;
using InMemoriam.Infraestructure.Data;
using InMemoriam.Infraestructure.Filters;
using InMemoriam.Infraestructure.Mappings;
using InMemoriam.Infraestructure.Repositories;
using InMemoriam.Infraestructure.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;


namespace InMemoriam
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var cfg = builder.Configuration;


            // MVC + filtros globales
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
                options.Filters.Add<ValidationFilter>();
            })
            .AddNewtonsoftJson();


            // Swagger/Swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InMemoriam API", Version = "v1" });
                c.EnableAnnotations();
            });


            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));


            // Versionado API
            builder.Services.AddApiVersioning(options =>
            {
                // Reporta las versiones soportadas y obsoletas en encabezados de respuesta
                options.ReportApiVersions = true;

                // Versión por defecto si no se especifica
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Soporta versionado mediante URL, Header o QueryString
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),       // Ejemplo: /api/v1/...
                    new HeaderApiVersionReader("x-api-version"), // Ejemplo: Header ? x-api-version: 1.0
                    new QueryStringApiVersionReader("api-version") // Ejemplo: ?api-version=1.0
                );
            });


            // DB: elegir proveedor desde config
            var provider = cfg.GetValue<string>("Database:Provider") ?? "SqlServer";
            if (provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddDbContext<AppDbContext>(o =>
                o.UseMySql(cfg.GetConnectionString("MySql"), new MySqlServerVersion(new Version(8, 0, 36))));
            }
            else
            {
                builder.Services.AddDbContext<AppDbContext>(o =>
                o.UseSqlServer(cfg.GetConnectionString("SqlServer")));
            }

            // Registro de servicio de validación y validators
            builder.Services.AddScoped<IValidatorService, ValidatorService>();
            builder.Services.AddScoped<ValidationFilter>();

            // Registro de validators (necesita FluentValidation.AspNetCore NuGet)
            builder.Services.AddValidatorsFromAssemblyContaining<MemorialDtoValidator>();

            // Dapper infra
            builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();


            // Repos & UoW
            builder.Services.AddScoped<IMemorialRepository, MemorialRepository>();
            builder.Services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            // Services
            builder.Services.AddScoped<IMemorialService, MemorialService>();
            builder.Services.AddScoped<IMediaAssetService, MediaAssetService>();
            builder.Services.AddScoped<IUserService, UserService>();


            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InMemoriam v1");
                });
            }


            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}