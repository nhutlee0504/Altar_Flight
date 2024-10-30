using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace API_Flight_Altar_ThucTap.Services
{
    public class GroupTypeService : IGroupType
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
        public GroupTypeService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<Group_Type>> GetAllGroupType()//Lấy tất cả danh sách nhóm đang liên kết với loại tài liệu
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin"))
            {
                var getGU = await _context.group_Types.ToListAsync();
                if (getGU != null)
                {
                    return getGU;
                }
                throw new NotImplementedException("No group found for this document type");
            }
            throw new UnauthorizedAccessException("You do not have permission to perform this action");
        }
        public async Task<IEnumerable<Group_Type>> GetGroupTypeByIdType(int idType)//Lấy nhóm tài liệu theo Id của loại tài liệu
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var GtFind = await _context.group_Types.Where(x => x.IdType == idType).ToListAsync();
                if (GtFind.Count > 0)
                {
                    return GtFind;
                }
                throw new NotImplementedException("No group found for this type or the type does not exist");
            }
            throw new UnauthorizedAccessException("You do not have permission to perform this action");
        }
        public async Task<Group_Type> AddGroupType(int idGroup, int idType, int idPermission)//Thêm nhóm tài liệu
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var newGT = new Group_Type
                {
                    IdGroup = idGroup,
                    IdType = idType,
                    IdPermission = idPermission
                };
                await _context.group_Types.AddAsync(newGT);
                await _context.SaveChangesAsync();
                return newGT;
            }
            throw new UnauthorizedAccessException("You do not have permission to perform this action");
        }
        public async Task<Group_Type> DeleteGroupType(int idGT)//Xóa nhóm tài liệu
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var gtFind = await _context.group_Types.FirstOrDefaultAsync(x => x.IdGT == idGT);
                if (gtFind != null)
                {
                    if (userInfo.Role.ToLower().Contains("admin"))
                    {
                        _context.group_Types.Remove(gtFind);
                        await _context.SaveChangesAsync();
                        return gtFind;
                    }
                    var typeFind = await _context.typeDocs.FirstOrDefaultAsync(x => x.IdTypeDoc == gtFind.IdType);
                    if (typeFind.UserId == userInfo.IdUser)
                    {
                        _context.group_Types.Remove(gtFind);
                        await _context.SaveChangesAsync();
                        return gtFind;
                    }
                    throw new UnauthorizedAccessException("You do not have permission to perform actions with this document group");
                }
                throw new NotImplementedException("No group found for this document type");
            }
            throw new UnauthorizedAccessException("You do not have permission to perform this action");
        }
        public async Task<Group_Type> UpdateGroupType(int idGT, int idGroup, int idType, int idPermission)//Cập nhật nhóm tài liệu
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var gtFind = await _context.group_Types.FirstOrDefaultAsync(x => x.IdGT == idGT);
                if (gtFind != null)
                {
                    var typeFind = await _context.typeDocs.FirstOrDefaultAsync(x => x.IdTypeDoc == idType);
                    if (typeFind != null && typeFind.UserId == userInfo.IdUser)
                    {
                        gtFind.IdGroup = idGroup;
                        gtFind.IdType = idType;
                        gtFind.IdPermission = idPermission;
                        await _context.SaveChangesAsync();
                        return gtFind;
                    }
                    throw new UnauthorizedAccessException("You do not have permission to update this document group.");
                }
                throw new NotImplementedException("No document type group found");
            }
            throw new UnauthorizedAccessException("You do not have permission to perform this action");
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
