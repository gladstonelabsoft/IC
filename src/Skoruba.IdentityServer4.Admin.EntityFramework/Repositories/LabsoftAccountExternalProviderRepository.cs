using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories
{
    public class LabsoftAccountExternalProviderRepository<TDbContext> : 
        ILabsoftAccountExternalProviderRepository where TDbContext : DbContext, IAdminLogDbContext
    {
        protected readonly TDbContext DbContext;
        public LabsoftAccountExternalProviderRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<IEnumerable<LabsoftAccountExternalProvider>> GetByAccountDomain(string accountDomain)
        {
            var labsoftAccountExternalProviders = await DbContext.LabsoftAccountExternalProviders
                .Where(x => x.AccountDomain == accountDomain)
                .Where(x => x.Enabled == true)
                .ToListAsync();

            return labsoftAccountExternalProviders;
        }
    }
}
