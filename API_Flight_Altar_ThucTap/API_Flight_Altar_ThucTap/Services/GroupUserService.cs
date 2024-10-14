using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Flight_Altar_ThucTap.Services
{
    public class GroupUserService : IGroupUser
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GroupUserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Group_User> AddGroupUser(int GroupID, int UserID)//Thêm người dùng vào nhóm
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                var groupFind = await _context.groups.FirstOrDefaultAsync(x => x.IdGroup == GroupID);
                if (groupFind != null)
                {
                    // Kiểm tra xem UserID có phải là người tạo nhóm không
                    if (UserID == groupFind.UserId)
                    {
                        throw new InvalidOperationException("Người dùng không thể thêm chính họ vào nhóm mà họ đã tạo.");
                    }

                    var groupuserFind = await _context.group_Users.FirstOrDefaultAsync(x => x.GroupID == GroupID && x.UserID == UserID);
                    if (groupuserFind == null)
                    {
                        var newGU = new Group_User
                        {
                            GroupID = GroupID,
                            UserID = UserID
                        };
                        await _context.group_Users.AddAsync(newGU);
                        await _context.SaveChangesAsync();
                        return newGU;
                    }
                    throw new InvalidOperationException("Người dùng đã tồn tại trong nhóm");
                }
            throw new NotImplementedException("Không tìm thấy nhóm này");
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng này");
        }

        public async Task<Group_User> DeleteGroupUser(int GroupID, int UserID) // Xóa người dùng khỏi nhóm chỉ định
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            // Check if the user is Admin or GO
            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                var groupFind = await _context.groups.FirstOrDefaultAsync(x => x.IdGroup == GroupID);
                if (groupFind != null)
                {
                    // Check if the current user is the creator of the group
                    if (groupFind.UserId != userInfo.IdUser)
                    {
                        throw new UnauthorizedAccessException("Chỉ người tạo nhóm mới có quyền xóa người dùng khỏi nhóm.");
                    }

                    var groupuserFind = await _context.group_Users.FirstOrDefaultAsync(x => x.GroupID == GroupID && x.UserID == UserID);
                    if (groupuserFind != null)
                    {
                        _context.group_Users.Remove(groupuserFind);
                        await _context.SaveChangesAsync();
                        return groupuserFind;
                    }
                    throw new NotImplementedException("Không tìm thấy người dùng hoặc nhóm");
                }
                throw new NotImplementedException("Không tìm thấy nhóm này");
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng này");
        }


        public async Task<IEnumerable<Group_User>> GetGroupUserById(int id)//Lấy danh sách người dùng có trong group chỉ định
        {
            var guFind = await _context.group_Users.Where(x => x.GroupID == id).ToListAsync();
            return guFind;
        }

        //phương thức ngoài
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
