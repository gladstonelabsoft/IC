using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.Migrations.Logging
{
    public partial class AddLabsoftAccountExternalProviderClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabsoftAccountExternalProvider",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountDomain = table.Column<string>(type: "text", nullable: true),
                    ExternalProviderName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    SecretId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabsoftAccountExternalProvider", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabsoftAccountExternalProvider");
        }
    }
}
