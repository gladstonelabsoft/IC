using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.Migrations.Logging
{
    public partial class AddRowLabsoftAccountExternalProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                insert into ""LabsoftAccountExternalProvider"" (""Id"", ""AccountDomain"", ""ExternalProviderName"", ""TenantId"", ""ClientId"", ""SecretId"", ""Created"", ""Updated"", ""Enabled"")
                values('735dd0d1-1cca-4dae-856d-7950781a5e43', 'labsoft', 'azure', 'bd404ee8-ff3b-41bb-a738-ba4430d2977f', 'f8d1bf38-932c-4c4a-b0ad-5a8d8df1ea00', 'jKG8Q~JYP7HGpJCu5FPAP8KmAU-HaOGVWPILrcz8', '2023-02-22 08:44:00.000', '2023-02-22 08:44:00.000', true);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                delete from ""LabsoftAccountExternalProvider"" where ""Id"" = '735dd0d1-1cca-4dae-856d-7950781a5e43';
            ");
        }
    }
}
