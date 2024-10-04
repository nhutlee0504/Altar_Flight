using API_Flight_Altar.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace API_Flight_Altar_ThucTap.Model
{
    public class TypeDoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTypeDoc { get; set; }

        [Column(TypeName = "varchar(50)"), Required(ErrorMessage = "Vui lòng nhập tên loại tài liệu")]
        public string TypeName { get; set; }

        public DateTime CreatedDate { get; set; }

        // Khóa ngoại liên kết với User
        public int UserId { get; set; }
        public User User { get; set; } // Điều này giúp lấy thông tin người dùng nếu cần
    }
}
