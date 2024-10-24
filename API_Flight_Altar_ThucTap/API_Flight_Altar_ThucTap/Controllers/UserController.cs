using API_Flight_Altar.Model;
using API_Flight_Altar.Services;
using API_Flight_Altar_ThucTap.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserSevice _userService;

        public UserController(IUserSevice userService)
        {
            _userService = userService;
        }
        [HttpGet("GetUser")]
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
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message); 
            }

        }

        [HttpGet("GetMyInfo")]
        public async Task<IActionResult> GetMyInfo()
        {
            try
            {
                var user = await _userService.GetMyInfo();
                return Ok(user);
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

        [HttpPut("UpdateUser")]
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("ChangeMyPassword")]
        public async Task<IActionResult> ChangeMyPassword(string OldPassword, string NewPassword)
        {
            try
            {
                var change = await _userService.ChangePassword(OldPassword, NewPassword);
                return Ok(change);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("ChangePasswordUser")]
        public async Task<IActionResult> ChangePasswordUser(int idUser, string Password)
        {
            try
            {
                var change = await _userService.ChangePasswordUser(idUser, Password);
                return Ok(change);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
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
            catch (NotImplementedException ex)
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
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FindUserId")]
        public async Task<IActionResult> FindUserId(int idUser)
        {
            try
            {
                var u = await _userService.FindUserById(idUser);
                return Ok(u);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
