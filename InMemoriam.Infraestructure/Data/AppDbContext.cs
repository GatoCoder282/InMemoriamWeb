using InMemoriam.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Memorial> Memorials => Set<Memorial>();
        public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
        public DbSet<Tribute> Tributes => Set<Tribute>();
        public DbSet<Condolence> Condolences => Set<Condolence>();
        public DbSet<AccessPolicy> AccessPolicies => Set<AccessPolicy>();
        public DbSet<QrCode> QrCodes => Set<QrCode>();
        public DbSet<FamilyGroup> FamilyGroups => Set<FamilyGroup>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var isActiveProp = entityType.FindProperty("IsActive");
                if (isActiveProp != null && isActiveProp.ClrType == typeof(bool))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, "IsActive"),
                        Expression.Constant(true));
                    var lambda = Expression.Lambda(body, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
