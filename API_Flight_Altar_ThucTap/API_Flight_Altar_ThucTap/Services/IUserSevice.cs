using API_Flight_Altar.Model;
using API_Flight_Altar_ThucTap.Dto;

namespace API_Flight_Altar.Services
{
    public interface IUserSevice
    {
        public Task<IEnumerable<User>> GetUsers();
        public Task<User> RegisterAdminAsync(UserRegisterDto userRegisterDto);
        public Task<User> RegisterAsync(UserRegisterDto userRegisterDto);
        public Task<string> LoginAdmin(UserLoginDto userLoginDto);
        public Task<string> LoginUser(UserLoginDto userLoginDto);
        public Task<User> UpdateUser(string name, string phone);
        public Task<User> FindUserByEmail(string email);
        public Task<User> PermissionUser(int idUser, string role);
        public Task<User> LockUser(int id);
        public Task<User> UnlockUser(int id);
    }
}
