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

        public GroupTypeService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Group_Type> AddGroupType(int idGroup, int idType, int idPermission)
        {
            var user = GetUserInfoFromClaims();
            if (user.Role == "Admin" || user.Role == "GO")
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

        public async Task<Group_Type> DeleteGroupType(int idGT)
        {
            var user = GetUserInfoFromClaims();
            if (user.Role == "Admin" || user.Role == "GO")
            {
                var gtFind = await _context.group_Types.FirstOrDefaultAsync(x => x.IdGT == idGT);
                if (gtFind != null)
                {
                    var typeFind = await _context.typeDocs.FirstOrDefaultAsync(x => x.IdTypeDoc == gtFind.IdType);
                    if (typeFind.UserId == user.IdUser)
                    {
                        _context.typeDocs.Remove(typeFind);
                        await _context.SaveChangesAsync();
                        return gtFind;
                    }
                    throw new UnauthorizedAccessException("You do not have permission to perform actions with this document group");
                }
                throw new NotImplementedException("No group found for this document type");
            }
            throw new UnauthorizedAccessException("You do not have permission to perform this action");
        }

        public async Task<IEnumerable<Group_Type>> GetAllGroupType()
        {
            var user = GetUserInfoFromClaims();
            if (user.Role == "Admin" || user.Role == "GO")
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

        public async Task<IEnumerable<Group_Type>> GetGroupTypeByIdType(int idType)
        {
            var user = GetUserInfoFromClaims();
            if (user.Role == "Admin" || user.Role == "GO")
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

        public async Task<Group_Type> UpdateGroupType(int idGT, int idGroup, int idType, int idPermission)
        {
            var user = GetUserInfoFromClaims();
            if (user.Role == "Admin" || user.Role == "GO")
            {
                var gtFind = await _context.group_Types.FirstOrDefaultAsync(x => x.IdGT == idGT);
                if (gtFind != null)
                {
                    var typeFind = await _context.typeDocs.FirstOrDefaultAsync(x => x.IdTypeDoc == idType);
                    if (typeFind != null && typeFind.UserId == user.IdUser)
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
