using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUser { get; set; }
        [Column(TypeName = "varchar(100)"), Required(ErrorMessage = "Vui lòng nhập email"),
            EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Column(TypeName = "varchar(100)"), Required(ErrorMessage = "Vui lòng nhập mật khẩu"),
         RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,16}$",
                          ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ cái, một số và một ký tự đặc biệt.")]
        public string Password { get; set; }
        public string? Name { get; set; }

        public string? Phone { get; set; }
        public string? Status { get; set; }
        public string? Role { get; set; }
        public List<TypeDoc> typeDocs { get; set; }
        public List<GroupModel> groups { get; set; }
    }
}
