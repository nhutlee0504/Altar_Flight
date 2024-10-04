using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Flight_Altar_ThucTap.Services
{
    public class GroupService : IGroup
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GroupService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GroupModel> CreateGroup(GroupDto group)
        {
            // Lấy IdUser từ claims
            var userId = GetUserIdFromClaims();

            var newGr = new GroupModel
            {
                GroupName = group.GroupName,
                Note = group.Note,
                CreatedDate = DateTime.UtcNow,
                UserId = userId,
            };

            await _context.groups.AddAsync(newGr);
            await _context.SaveChangesAsync();
            return newGr;
        }

        public async Task<IEnumerable<GroupModel>> GetGroups()
        {
            return await _context.groups.ToListAsync();
        }

        public async Task<GroupModel> UpdateGroup(int id, GroupDto group)
        {
            var groupFind = await _context.groups.FindAsync(id);
            if (groupFind != null)
            {
                groupFind.GroupName = group.GroupName;
                groupFind.Note = group.Note;
                await _context.SaveChangesAsync();
                return groupFind;
            }
            throw new NotImplementedException("Không tìm thấy nhóm");
        }

        private int GetUserIdFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var userIdClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier); // Lấy IdUser từ claim
                if (userIdClaim != null)
                {
                    return int.Parse(userIdClaim.Value);
                }
            }
            throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng.");
        }
    }
}
