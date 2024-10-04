using API_Flight_Altar.Model;
using API_Flight_Altar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserSevice _userService;

        public AuthController(IUserSevice userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userService.GetUsers();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                var user = await _userService.RegisterAsync(userRegisterDto);
                return Ok(new { User = user, Message = "Đăng ký thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                var token = await _userService.LoginAsync(userLoginDto);
                return Ok(new { Token = token, Message = "Đăng nhập thành công." });
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest("Email hoặc mật khẩu không đúng."); // Thông báo lỗi đơn giản
            }
        }


    }
}
