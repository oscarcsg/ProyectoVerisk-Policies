using PoliciesService.Application.Repositories;
using PoliciesService.Domain;
using Microsoft.EntityFrameworkCore;

namespace PoliciesService.Infrastructure.Repositories
{
    public class PolicyHolderRepository : IPolicyHolderRepository
    {
        private readonly AppDbContext _context;

        public PolicyHolderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PolicyHolder> AddAsync(PolicyHolder policyHolder)
        {
            _context.PolicyHolders.Add(policyHolder);
            await _context.SaveChangesAsync();
            return policyHolder;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.PolicyHolders.AnyAsync(x => x.Email == email);
        }
    }
}
