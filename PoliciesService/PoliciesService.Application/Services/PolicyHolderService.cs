using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.PolicyHolder;
using PoliciesService.Application.Interfaces;
using PoliciesService.Application.Repositories;
using PoliciesService.Domain;

namespace PoliciesService.Application.Services
{
    public class PolicyHolderService : IPolicyHolderService
    {
        private readonly IPolicyHolderRepository _repository;
        private readonly IReferenceDataClient _referenceDataClient;

        public PolicyHolderService(IPolicyHolderRepository repository, IReferenceDataClient referenceDataClient)
        {
            _repository = repository;
            _referenceDataClient = referenceDataClient;
        }

        public async Task<Result<PolicyHolderResponseDTO>> GetByIdAsync(int id)
        {
            var holder = await _repository.GetByIdAsync(id);
            if (holder == null) return Result<PolicyHolderResponseDTO>.Failure("Policy holder not found.");

            return Result<PolicyHolderResponseDTO>.Success(MapToResponseDto(holder));
        }

        public async Task<Result<IEnumerable<PolicyHolderResponseDTO>>> GetAllAsync(int page, int pageSize)
        {
            var holders = await _repository.GetAllAsync(page, pageSize);
            return Result<IEnumerable<PolicyHolderResponseDTO>>.Success(holders.Select(MapToResponseDto));
        }

        public async Task<Result<PolicyHolderResponseDTO>> CreatePolicyHolderAsync(PolicyHolderRequestDTO dto)
        {
            // 1. Validation
            var validationResult = await ValidateExternalDataAsync(dto);
            if (!validationResult.IsSuccess) return Result<PolicyHolderResponseDTO>.Failure(validationResult.ErrorMessage!);

            // 2. Does email already exist?
            bool emailExists = await _repository.EmailExistsAsync(dto.Email);
            if (emailExists)
            {
                return Result<PolicyHolderResponseDTO>.Failure("Email already registered.");
            }

            var newHolder = new PolicyHolder
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                RegionCode = dto.RegionCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var savedHolder = await _repository.AddAsync(newHolder);
            return Result<PolicyHolderResponseDTO>.Success(MapToResponseDto(savedHolder));
        }

        public async Task<Result<PolicyHolderResponseDTO>> UpdatePolicyHolderAsync(int id, PolicyHolderRequestDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return Result<PolicyHolderResponseDTO>.Failure("Policy holder not found.");

            // 1. Validation
            var validationResult = await ValidateExternalDataAsync(dto);
            if (!validationResult.IsSuccess) return Result<PolicyHolderResponseDTO>.Failure(validationResult.ErrorMessage!);

            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.Email = dto.Email;
            existing.Phone = dto.Phone;
            existing.DateOfBirth = dto.DateOfBirth;
            existing.RegionCode = dto.RegionCode;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing);
            return Result<PolicyHolderResponseDTO>.Success(MapToResponseDto(updated));
        }

        private async Task<Result<bool>> ValidateExternalDataAsync(PolicyHolderRequestDTO dto)
        {
            try
            {
                var regionsResponse = await _referenceDataClient.GetAllRegionsAsync();
                if (!regionsResponse.IsSuccessStatusCode)
                {
                    return Result<bool>.Failure("Reference Data service error while fetching regions.");
                }

                var regions = regionsResponse.Content;
                if (regions == null || !regions.Any(r => r.Code == dto.RegionCode))
                {
                    return Result<bool>.Failure($"Invalid RegionCode: {dto.RegionCode}");
                }

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure("Reference Data service is currently unavailable (503).");
            }
        }

        public async Task<Result<bool>> DeletePolicyHolderAsync(int id)
        {
            // Check if has active policies
            bool hasPolicies = await _repository.HasActivePoliciesAsync(id);
            if (hasPolicies)
            {
                return Result<bool>.Failure("Cannot delete a policy holder with active policies.");
            }

            bool deleted = await _repository.DeleteAsync(id);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Policy holder not found.");
        }

        private static PolicyHolderResponseDTO MapToResponseDto(PolicyHolder holder)
        {
            return new PolicyHolderResponseDTO
            {
                Id = holder.Id,
                FirstName = holder.FirstName,
                LastName = holder.LastName,
                Email = holder.Email,
                Phone = holder.Phone,
                DateOfBirth = holder.DateOfBirth,
                RegionCode = holder.RegionCode,
                CreatedAt = holder.CreatedAt,
                UpdatedAt = holder.UpdatedAt
            };
        }
    }
}
