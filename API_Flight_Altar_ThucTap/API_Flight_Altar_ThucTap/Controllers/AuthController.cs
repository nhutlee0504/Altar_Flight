using API_Flight_Altar.Model;
using API_Flight_Altar.Services;
using API_Flight_Altar_ThucTap.Dto;
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

        [HttpGet("user")]
        public async Task<IActionResult> GetUsers()//Lấy tất cả danh sách người dùng
        {
            try
            {
                var users = await _userService.GetUsers();
                return Ok(users);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("registeradmin")]
        public async Task<IActionResult> RegisterAdmin(UserRegisterDto userRegisterDto)//Tạo tài khoản người dùng có thể chọn quyền
        {
            try
            {
                var user = await _userService.RegisterAdminAsync(userRegisterDto);
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
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)//Tạo tài khoản người dùng có thể chọn quyền
        {
            try
            {
                var user = await _userService.RegisterAsync(userRegisterDto);
                //return Ok(new { User = user, Message = "Đăng ký thành công." });
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("loginuser")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)//Đăng nhập dành cho User
        {
            try
            {
                var token = await _userService.LoginUser(userLoginDto);
                //return Ok(new { Token = token, Message = "Đăng nhập thành công." });
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message); // Thông báo lỗi đơn giản
            }
        }

        [HttpPost("loginadmin")]
        public async Task<IActionResult> LoginAdmin([FromBody] UserLoginDto userLoginDto)//Đăng nhập dành cho Admin
        {
            try
            {
                var token = await _userService.LoginAdmin(userLoginDto);
                //return Ok(new { Token = token, Message = "Đăng nhập thành công." });
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message); // Thông báo lỗi đơn giản
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(string name, string phone)//Cập nhật thông tin người dùng
        {
            try
            {
                var user = await _userService.UpdateUser(name, phone);
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Permission")]
        public async Task<IActionResult> PermissionUser(int id, string role)//Phân quyền cho các User
        {
            try
            {
                var user = await _userService.PermissionUser(id, role);
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FindUserEmail")]
        public async Task<IActionResult> FindUserEmail(string email)//Tìm kiếm người dùng qua Email
        {
            try
            {
                var user = await _userService.FindUserByEmail(email);
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("LockUser")]
        public async Task<IActionResult> LockUser(int id)//Khóa tài khoản người dùng
        {
            try
            {
                var user = await _userService.LockUser(id);
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCastException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("UnlockUser")]
        public async Task<IActionResult> UnlockUser(int id)//Mở khóa tài khoản người dùng
        {
            try
            {
                var user = await _userService.UnlockUser(id);
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCastException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
