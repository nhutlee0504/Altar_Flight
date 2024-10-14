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

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
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
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng.");
        }

        public async Task<GroupModel> DeleteGroup(int id)//Xóa group
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                var groupFind = await _context.groups.FindAsync(id);
                if (groupFind == null)
                {
                    throw new NotImplementedException("Không tìm thấy nhóm");
                }
                if (groupFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa nhóm này.");
                }
                _context.Remove(groupFind);
                await _context.SaveChangesAsync();
                return groupFind;
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng.");
        }

        public async Task<GroupModel> FindGroupByName(string name)//Tìm group theo tên
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                var groupFind = await _context.groups.FirstOrDefaultAsync(x => x.GroupName == name);
                if (groupFind == null)
                {
                    throw new NotImplementedException("Không tìm thấy nhóm");
                }
                if (groupFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa loại tài liệu này.");
                }
                return groupFind;
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng.");
        }

        public async Task<IEnumerable<GroupModel>> GetGroups()//Lấy danh sách group
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                return await _context.groups.ToListAsync();
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng.");
        }

        public async Task<GroupModel> UpdateGroup(int id, string groupName, string note)//Cập nhật group
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                var groupFind = await _context.groups.FindAsync(id);
                if (groupFind == null)
                {
                    throw new NotImplementedException("Không tìm thấy nhóm");
                }
                if (groupFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa nhóm này này.");
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
                var idClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier);
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role);

                if (idClaim != null && emailClaim != null && roleClaim != null)
                {
                    return (int.Parse(idClaim.Value), emailClaim.Value, roleClaim.Value);
                }
            }
            throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
        }
    }
}
