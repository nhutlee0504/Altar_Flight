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
        [Column(TypeName = "varchar(100)"), Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }

        [Column(TypeName = "varchar(100)"), Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public string? Role { get; set; }
        public ICollection<TypeDoc>? typeDocs { get; set; }
        public ICollection<GroupModel>? groups { get; set; }
        public ICollection<Group_User>? group_Users { get; set; }
        public ICollection<Flight>? flights { get; set; }
        public ICollection<DocFlight>? documents { get; set; }
    }
}
