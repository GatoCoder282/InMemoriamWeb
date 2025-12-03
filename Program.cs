using Asp.Versioning;
using InMemoriam.Core.Interfaces;
using InMemoriam.Core.Services;
using InMemoriam.Infraestructure.Data;
using InMemoriam.Infraestructure.Filters;
using InMemoriam.Infraestructure.Mappings;
using InMemoriam.Infraestructure.Repositories;
using InMemoriam.Infraestructure.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace InMemoriam
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var cfg = builder.Configuration;

            // Cargar user secrets en desarrollo (opcional)
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
            }

            // Registrar handlers de Dapper para DateOnly (importante antes de usar Dapper)
            DapperTypeHandlerRegistration.Register();

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

                // Agregar esquema JWT para Swagger UI
                var jwtScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Ingrese 'Bearer {token}'"
                };
                c.AddSecurityDefinition("Bearer", jwtScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtScheme, Array.Empty<string>() }
                });
            });

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Versionado API
            builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("x-api-version"),
                    new QueryStringApiVersionReader("api-version")
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
            builder.Services.AddValidatorsFromAssemblyContaining<MemorialDtoValidator>();

            // Dapper infra
            builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();

            // Repos & UoW
            builder.Services.AddScoped<IMemorialRepository, MemorialRepository>();
            builder.Services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
            builder.Services.AddScoped<IMemorialMemberRepository, MemorialMemberRepository>();

            // Services
            builder.Services.AddScoped<IMemorialService, MemorialService>();
            builder.Services.AddScoped<IMediaAssetService, MediaAssetService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IInvitationService, InvitationService>();
            builder.Services.AddScoped<IMemorialMemberService, MemorialMemberService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            // ------- JWT Authentication -------
            var issuer = cfg["Authentication:Issuer"];
            var audience = cfg["Authentication:Audience"];
            var secret = cfg["Authentication:SecretKey"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
            {
                if (builder.Environment.IsProduction())
                {
                    throw new InvalidOperationException("Authentication:SecretKey ausente o insegura en Production. Use secrets/variables de entorno.");
                }
                Console.WriteLine("Warning: Authentication:SecretKey ausente o débil. Use secrets en desarrollo o variables de entorno en producción.");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                };
            });
            // ------------------------------------

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

            // Añadir autenticación antes de autorización
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}