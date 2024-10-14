using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Flight_Altar_ThucTap.Services
{
    public class GroupTypeService : IGroupType
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupTypeService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Group_Type> AddGroupType(int idGroup, int idType, int idPermission)
        {
            var user = GetUserInfoFromClaims();
            if (user.Role != "Admin" || user.Role == "GO")
            {
                throw new UnauthorizedAccessException("Chỉ admin mới có quyền");
            }
            var newGT = new Group_Type
            {
                IdGroup = idGroup,
                IdType = idType,
                IdPermission = idPermission
            };
            try
            {
                await _context.group_Types.AddAsync(newGT);
                await _context.SaveChangesAsync();
                return newGT;
            }
            catch (Exception)
            {
                throw new InvalidCastException("Có lỗi xảy ra. Hãy kiểm tra dữ liệu đã nhập");
            }
        }

        public Task<Group_Type> DeleteGroupType(int idGT)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Group_Type>> GetGroupTypeByIdType(int idType)
        {
            var user = GetUserInfoFromClaims();
            if (user.Role != "Admin" || user.Role == "GO")
            {
                throw new UnauthorizedAccessException("Chỉ admin mới có quyền");
            }
            var GtFind = await _context.group_Types.Where(x => x.IdType == idType).ToListAsync();
            if (GtFind.Count > 0)
            {
                return GtFind;
            }
            throw new NotImplementedException("Không tìm thấy nhóm có trong loại này hoặc loại không tồn tại");
        }

        public Task<Group_Type> UpdateGroupType(int idGT, int idGroup, int idType, int idPermission)
        {
            throw new NotImplementedException();
        }

        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()//Lấy dữ liệu thông qua token
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
