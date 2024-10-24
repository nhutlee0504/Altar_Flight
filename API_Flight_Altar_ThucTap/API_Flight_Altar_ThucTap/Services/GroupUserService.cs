using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
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

			if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
			{
				var groupFind = await _context.groups.FirstOrDefaultAsync(x => x.IdGroup == GroupID);
				if (groupFind != null)
				{
					if (UserID == groupFind.UserId)
					{
						throw new InvalidOperationException("You cannot add yourself to the group you created");
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
					throw new InvalidOperationException("User already exists in the group");
				}
				throw new NotImplementedException("No group found");
			}
			throw new UnauthorizedAccessException("You do not have permission to perform this action");
		}

		public async Task<Group_User> DeleteGroupUser(int id) // Xóa người dùng khỏi nhóm chỉ định
		{
			var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

			// Check if the user is Admin or GO
			if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
			{
				var groupuserFind = await _context.group_Users.FirstOrDefaultAsync(x => x.IdGU == id);
				if (groupuserFind != null)
				{
					if (userInfo.Role.ToLower().Contains("admin"))
					{
                        _context.group_Users.Remove(groupuserFind);
                        await _context.SaveChangesAsync();
                        return groupuserFind;
                    }
					var groupFind = await _context.groups.FirstOrDefaultAsync(x => x.IdGroup == groupuserFind.GroupID);
					if(groupFind.UserId == userInfo.IdUser)
					{
						_context.group_Users.Remove(groupuserFind);
						await _context.SaveChangesAsync();
						return groupuserFind;
					}
					throw new UnauthorizedAccessException("You do not have permission to perform actions with this group");
				}
				throw new NotImplementedException("User or group not found");
			}
			throw new UnauthorizedAccessException("You do not have permission to perform this action");
		}

		public async Task<IEnumerable<Group_User>> GetAllGroupUser()// lấy tất cả danh sách group
		{
			var userInfo = GetUserInfoFromClaims();
			if(userInfo.Role.ToLower().Contains("admin"))
			{
				var getGU = await _context.group_Users.ToListAsync();
				if(getGU != null)
				{
					return getGU;
				}
				throw new NotImplementedException("No user groups found");
			}

			throw new UnauthorizedAccessException("You do not have permission to perform this action");
		}

		public async Task<IEnumerable<Group_User>> GetGroupUserById(int id)//Lấy danh sách người dùng có trong group chỉ định
		{
			var userInfo = GetUserInfoFromClaims();
			var guFind = await _context.group_Users.Where(x => x.GroupID == id).ToListAsync();
			if(guFind != null)
			{
                return guFind;
            }
			throw new NotImplementedException("No user groups found");
        }

        //phương thức ngoài
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
