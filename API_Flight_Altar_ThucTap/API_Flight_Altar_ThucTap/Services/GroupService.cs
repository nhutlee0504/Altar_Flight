using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Dto;
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

        public async Task<GroupModel> CreateGroup(string groupName, string note)//Tạo group
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    throw new ArgumentException("Group name cannot be empty.");
                }
                var findGroup = await _context.groups.FirstOrDefaultAsync(x => x.GroupName.ToLower().Contains(groupName));
                if (findGroup != null) 
                {
                    throw new InvalidOperationException("Group name already exists");
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

        public async Task<GroupModel> DeleteGroup(int id)//Xóa group
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

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

        public async Task<IEnumerable<GroupModel>> FindGroupByName(string name)//Tìm group theo tên
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

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

        public async Task<IEnumerable<GroupModel>> GetGroups()//Lấy danh sách group
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var gtgroup = await _context.groups.ToListAsync();
                if(gtgroup != null)
                {
                    return gtgroup;
                }
                throw new NotImplementedException("No group found");
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        public async Task<IEnumerable<GroupModel>> GetMyGroup() //Lấy nhóm của mình
        {
            var userInfo = GetUserInfoFromClaims();
            if(userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var groupFind = await _context.groups.Where(x => x.UserId == userInfo.IdUser).ToListAsync();
                if(groupFind != null)
                {
                    return groupFind;
                }
                throw new NotImplementedException("No group found");
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        public async Task<GroupModel> UpdateGroup(int id, string groupName, string note)//Cập nhật group
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("Group name cannot be empty.");
            }
            var findGroup = await _context.groups.FirstOrDefaultAsync(x => x.GroupName.ToLower().Contains(groupName));
            if (findGroup != null)
            {
                throw new InvalidOperationException("Group name already exists");
            }
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

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
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng.");
        }

        //Phương thức ngoài

        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;

            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var expClaim = userClaim.FindFirst("exp");
                if (expClaim != null && long.TryParse(expClaim.Value, out long exp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                    if (expirationTime < DateTime.UtcNow)
                    {
                        throw new UnauthorizedAccessException("Token has expired. Please log in again");
                    }
                }

                var idClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier);
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role);

                if (idClaim != null && emailClaim != null && roleClaim != null)
                {
                    return (int.Parse(idClaim.Value), emailClaim.Value, roleClaim.Value);
                }
            }

            throw new UnauthorizedAccessException("Please log in to the system");
        }
    }
}
