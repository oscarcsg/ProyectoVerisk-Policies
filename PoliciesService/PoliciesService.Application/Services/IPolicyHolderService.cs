using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.PolicyHolder;
namespace PoliciesService.Application.Services
{
    public interface IPolicyHolderService
    {
        Task<Result<PolicyHolderResponseDTO>> CreatePolicyHolderAsync(PolicyHolderRequestDTO dto);
    }
}
