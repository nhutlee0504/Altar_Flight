using API_Flight_Altar.Services;
using API_Flight_Altar_ThucTap.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar_ThucTap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserSevice _userSevice;
        public AuthController(IUserSevice userSevice)
        {
            _userSevice = userSevice;
        }

        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(UserRegisterDto userRegisterDto)//Tạo tài khoản người dùng có thể chọn quyền
        {
            try
            {
                var user = await _userSevice.RegisterAdminAsync(userRegisterDto);
                //return Ok(new { User = user, Message = "Đăng ký thành công." });
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("LoginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromBody] UserLoginDto userLoginDto)//Đăng nhập dành cho Admin
        {
            try
            {
                var token = await _userSevice.LoginAdmin(userLoginDto);
                //return Ok(new { Token = token, Message = "Đăng nhập thành công." });
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message); // Thông báo lỗi đơn giản
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("registeruser")]
        public async Task<IActionResult> RegisterUser(UserRegisterDto userRegisterDto)//Tạo tài khoản người dùng có thể chọn quyền
        {
            try
            {
                var user = await _userSevice.RegisterAsync(userRegisterDto);
                //return Ok(new { User = user, Message = "Đăng ký thành công." });
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("loginuser")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)//Đăng nhập dành cho User
        {
            try
            {
                var token = await _userSevice.LoginUser(userLoginDto);
                //return Ok(new { Token = token, Message = "Đăng nhập thành công." });
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message); // Thông báo lỗi đơn giản
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _userSevice.Logout();
                return Ok(new { Message = "Đăng xuất thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
