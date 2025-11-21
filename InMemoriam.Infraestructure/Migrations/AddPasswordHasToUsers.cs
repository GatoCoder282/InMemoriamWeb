using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InMemoriam.Infraestructure.Migrations
{
    public partial class AddPasswordHashToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Añade columna PasswordHash (nullable para no romper datos existentes)
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Para SQL Server EF generará tipo nvarchar(max) automáticamente si usas SQL Server provider.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");
        }
    }
}
