using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.PolicyHolder;
using PoliciesService.Application.Repositories;
using PoliciesService.Domain;

namespace PoliciesService.Application.Services
{
    public class PolicyHolderService : IPolicyHolderService
    {
        private readonly IPolicyHolderRepository _repository;

        public PolicyHolderService(IPolicyHolderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PolicyHolderResponseDTO>> CreatePolicyHolderAsync(PolicyHolderRequestDTO dto)
        {
            // Does email already exist?
            bool emailExists = await _repository.EmailExistsAsync(dto.Email);
            if (emailExists)
            {
                // Return an error, but NOT an exception
                return Result<PolicyHolderResponseDTO>.Failure("Email already registered.");
            }

            // If all good, send data to a new entity
            var newHolder = new PolicyHolder
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                RegionCode = dto.RegionCode
            };

            // Repository, store it in SQL
            var savedHolder = await _repository.AddAsync(newHolder);

            // Prepare the response DTO for the user
            var responseDto = new PolicyHolderResponseDTO
            {
                Id = savedHolder.Id,
                FirstName = savedHolder.FirstName,
                LastName = savedHolder.LastName,
                Email = savedHolder.Email,
                Phone = savedHolder.Phone,
                DateOfBirth = savedHolder.DateOfBirth,
                RegionCode = savedHolder.RegionCode,
                CreatedAt = savedHolder.CreatedAt,
                UpdatedAt = savedHolder.UpdatedAt
            };

            // Return a success message with the data
            return Result<PolicyHolderResponseDTO>.Success(responseDto);
        }
    }
}
