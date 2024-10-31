using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Flight_Altar_ThucTap.Services
{
    public class PermissionService : IPermission
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
        public PermissionService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Permission> AddPermission(string namePermission)
        {
            var userInfo = GetUserInfoFromClaims();

            if (!userInfo.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("You do not have permission to add permissions.");
            }

            var newPer = new Permission
            {
                PermissionName = namePermission,
            };
            await _context.permissions.AddAsync(newPer);
            await _context.SaveChangesAsync();
            return newPer;
        }

        public async Task<Permission> DeletePermission(int id)
        {
            var userInfo = GetUserInfoFromClaims();

            if (!userInfo.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("You do not have permission to delete permissions.");
            }

            var perFind = await _context.permissions.FindAsync(id);
            if (perFind != null)
            {
                _context.permissions.Remove(perFind);
                await _context.SaveChangesAsync();
                return perFind;
            }
            throw new NotImplementedException("Permission not found");
        }

        public async Task<IEnumerable<Permission>> GetPermission()
        {
            var userInfo = GetUserInfoFromClaims();

            if (!userInfo.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("You do not have permission to view permissions.");
            }

            var permission = await _context.permissions.ToListAsync();
            return permission;
        }

        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;

            if (userClaim == null || !userClaim.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Please log in to the system");
            }

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_blacklistedTokens.Contains(token))
            {
                throw new UnauthorizedAccessException("Token has been invalidated.");
            }

            var idClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
            var roleClaim = userClaim.FindFirst(ClaimTypes.Role);

            if (idClaim == null || emailClaim == null || roleClaim == null)
            {
                throw new InvalidOperationException("User claims are missing.");
            }

            return (int.Parse(idClaim.Value), emailClaim.Value, roleClaim.Value);
        }

    }
}
