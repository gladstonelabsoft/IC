using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.LogBusinessLogic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface ILabsoftAccountExternalProviderService
    {
        Task<IEnumerable<LabsoftAccountExternalProviderDto>> GetByAccountDomain(string accountDomain);
    }
}
