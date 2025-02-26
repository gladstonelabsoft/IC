using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.LogBusinessLogic;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class LabsoftAccountExternalProviderService : ILabsoftAccountExternalProviderService
    {
        protected readonly ILabsoftAccountExternalProviderRepository LabsoftAccountExternalProviderRepository;

        public LabsoftAccountExternalProviderService(ILabsoftAccountExternalProviderRepository labsoftAccountExternalProviderRepository)
        {
            LabsoftAccountExternalProviderRepository = labsoftAccountExternalProviderRepository;
        }

        public async Task<IEnumerable<LabsoftAccountExternalProviderDto>> GetByAccountDomain(string accountDomain)
        {
            var labsoftAccountExternalProviders = await LabsoftAccountExternalProviderRepository.GetByAccountDomain(accountDomain);
            if (labsoftAccountExternalProviders == null) 
            {
                return new List<LabsoftAccountExternalProviderDto> {
                    new LabsoftAccountExternalProviderDto(
                        accountDomain: string.Empty,
                        externalProviderName: string.Empty,
                        tenantId: string.Empty,
                        clientId: string.Empty,
                        secretId: string.Empty)
                };
            }

            return labsoftAccountExternalProviders
                .Select(l => new LabsoftAccountExternalProviderDto(
                    accountDomain: l.AccountDomain,
                    externalProviderName: l.ExternalProviderName,
                    tenantId: l.TenantId,
                    clientId: l.ClientId,
                    secretId: l.SecretId))
                .ToList();
        }
    }
}
