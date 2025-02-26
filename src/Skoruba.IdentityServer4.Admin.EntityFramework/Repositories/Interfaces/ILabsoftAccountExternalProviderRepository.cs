using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces
{
    public interface ILabsoftAccountExternalProviderRepository
    {
        Task<IEnumerable<LabsoftAccountExternalProvider>> GetByAccountDomain(string accountDomain);
    }
}
