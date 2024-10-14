using System.ComponentModel.DataAnnotations;

namespace API_Flight_Altar_ThucTap.Dto
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vai trò của tài khoản")]
        public string Role { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
