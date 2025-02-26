using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.Migrations.Logging
{
    public partial class AddRowGoogleExternalProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                insert into ""LabsoftAccountExternalProvider"" (""Id"", ""AccountDomain"", ""ExternalProviderName"", ""TenantId"", ""ClientId"", ""SecretId"", ""Created"", ""Updated"", ""Enabled"")
                values('fb90fa6e-91f7-426e-afdb-f2dce74697e8', 'portal', 'gmail', null, '1052649566567-3oi2j6d5im9allhhu9aav3e7enh3nfc2.apps.googleusercontent.com', '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', '2023-04-24 00:00:00.000', '2023-04-24 00:00:00.000', true);
            ");
        }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                delete from ""LabsoftAccountExternalProvider"" where ""Id"" = 'fb90fa6e-91f7-426e-afdb-f2dce74697e8';
            ");
        }
    }
}
