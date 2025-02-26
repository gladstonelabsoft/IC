using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.Migrations.Logging
{
    public partial class AddRowLabsoftAccountExternalProviderfrimesa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                insert into ""LabsoftAccountExternalProvider"" (""Id"", ""AccountDomain"", ""ExternalProviderName"", ""TenantId"", ""ClientId"", ""SecretId"", ""Created"", ""Updated"", ""Enabled"")
                values('e8c0fa87-0180-4d5e-84ff-2e508e1965fa', 'frimesa', 'azure', '2f87287a-770e-40b4-93b4-8dad7ea553f5', 'db02ecd1-4a09-4e67-a577-a78f5a0c8639', '16adb681-1c06-40fd-ba27-d25b5afd1e38', '2023-03-05 00:00:00.000', '2023-03-05 00:00:00.000', true);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                delete from ""LabsoftAccountExternalProvider"" where ""Id"" = 'e8c0fa87-0180-4d5e-84ff-2e508e1965fa';
            ");
        }
    }
}
