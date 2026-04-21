using Refit;

namespace PoliciesService.Application.Interfaces
{
    public interface IReferenceDataClient
    {
        // Validate PolicyTypeCode
        [Get("/api/v1/policy-types/by-code/{code}")]
        Task<HttpResponseMessage> CheckPolicyTypeAsync(string code);

        // Validate CoverageTypeCode
        [Get("/api/v1/coverage-types/by-code/{code}")]
        Task<HttpResponseMessage> CheckCoverageTypeAsync(string code);

        // Validate RegionCode
        // POSSIBLE ERROR: if the endpoint is literally "/api/v1/regions", it will return ALL the regions at once
        [Get("/api/v1/regions")]
        Task<HttpResponseMessage> GetAllRegionsAsync();
    }
}
