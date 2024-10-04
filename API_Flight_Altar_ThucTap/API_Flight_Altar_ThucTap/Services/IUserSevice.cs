using API_Flight_Altar.Model;

namespace API_Flight_Altar.Services
{
    public interface IUserSevice
    {
        public Task<IEnumerable<User>> GetUsers();
        public Task<User> RegisterAsync(UserRegisterDto userRegisterDto);
        public Task<string> LoginAsync(UserLoginDto userLoginDto);
    }
}
