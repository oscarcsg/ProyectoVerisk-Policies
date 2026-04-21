using PoliciesService.Domain;

namespace PoliciesService.Application.Repositories
{
    public interface IPolicyHolderRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<PolicyHolder> AddAsync(PolicyHolder policyHolder);
    }
}
