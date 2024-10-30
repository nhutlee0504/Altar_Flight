using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace API_Flight_Altar_ThucTap.Services
{
    public class GroupService : IGroup
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
        public GroupService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<GroupModel>> GetGroups()//Lấy danh sách nhóm người dùng
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var gtgroup = await _context.groups.ToListAsync();
                if (gtgroup != null)
                {
                    return gtgroup;
                }
                throw new NotImplementedException("No group found");
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }
        public async Task<IEnumerable<GroupModel>> GetMyGroup() //Lấy danh sách nhóm do người dùng tạo
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var groupFind = await _context.groups.Where(x => x.UserId == userInfo.IdUser).ToListAsync();
                if (groupFind != null)
                {
                    return groupFind;
                }
                throw new NotImplementedException("No group found");
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }
        public async Task<GroupModel> CreateGroup(string groupName, string note)//Tạo nhóm
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    throw new ArgumentException("Group name cannot be empty.");
                }
                var newGr = new GroupModel
                {
                    GroupName = groupName,
                    Note = note,
                    CreatedDate = DateTime.UtcNow,
                    UserId = userInfo.IdUser,
                };

                await _context.groups.AddAsync(newGr);
                await _context.SaveChangesAsync();
                return newGr;
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }
        public async Task<GroupModel> DeleteGroup(int id)//Xóa nhóm
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var groupFind = await _context.groups.FindAsync(id);
                if (groupFind == null)
                {
                    throw new NotImplementedException("No group found");
                }
                if(userInfo.Role.ToLower().Contains("admin"))
                {
                    _context.Remove(groupFind);
                    await _context.SaveChangesAsync();
                    return groupFind;
                }
                if (groupFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("You do not have permission to modify this group");
                }
                _context.Remove(groupFind);
                await _context.SaveChangesAsync();
                return groupFind;
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }
        public async Task<IEnumerable<GroupModel>> FindGroupByName(string name)//Tìm nhóm theo tên
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var groupFind = await _context.groups.Where(x => x.GroupName.Contains(name)).ToListAsync();
                if (groupFind == null)
                {
                    throw new NotImplementedException("No group found");
                }
                return groupFind;
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }
        public async Task<GroupModel> FindMyGroupById(int idGroup)//Tìm nhóm theo Id của Group
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var groupFind = await _context.groups.FirstOrDefaultAsync(x => x.IdGroup == idGroup);
                if (groupFind == null)
                {
                    throw new NotImplementedException("No group found");
                }
                return groupFind;
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }
        public async Task<GroupModel> UpdateGroup(int id, string groupName, string note)//Cập nhật nhóm
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("Group name cannot be empty.");
            }
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var groupFind = await _context.groups.FindAsync(id);
                if (groupFind == null)
                {
                    throw new NotImplementedException("No group found");
                }
                if (userInfo.Role.ToLower().Contains("admin"))
                {
                    groupFind.GroupName = groupName;
                    groupFind.Note = note;
                    await _context.SaveChangesAsync();
                    return groupFind;
                }
                if (groupFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("You do not have access permission");
                }
                groupFind.GroupName = groupName;
                groupFind.Note = note;
                await _context.SaveChangesAsync();
                return groupFind;
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        //Phương thức ngoài
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
