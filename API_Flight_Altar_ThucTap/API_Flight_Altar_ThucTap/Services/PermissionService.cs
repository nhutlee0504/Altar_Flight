using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;

namespace API_Flight_Altar_ThucTap.Services
{
    public class PermissionService : IPermission
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public PermissionService(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }
    
        public async Task<Permission> AddPermission(string namePermission)
        {
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
            var perFind = await _context.permissions.FindAsync(id);
            if (perFind != null)
            {
                _context.permissions.Remove(perFind);
                await _context.SaveChangesAsync();
                return perFind;
            }
            throw new NotImplementedException("Không tìm thấy quyền này");
        }

        public async Task<IEnumerable<Permission>> GetPermission()
        {
            var permission = await _context.permissions.ToListAsync();
            return permission;
        }
    }
}
