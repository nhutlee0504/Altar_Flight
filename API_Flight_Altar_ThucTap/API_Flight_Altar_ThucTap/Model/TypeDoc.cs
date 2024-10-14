using API_Flight_Altar.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace API_Flight_Altar_ThucTap.Model
{
    public class TypeDoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTypeDoc { get; set; }

        [Column(TypeName = "varchar(50)"), Required(ErrorMessage = "Vui lòng nhập tên loại tài liệu")]
        public string TypeName { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }

        // Khóa ngoại liên kết với User
        [ForeignKey("User")] // Tránh chu kỳ khi serialize
        public int UserId { get; set; }
        public User User { get; set; } // Điều này giúp lấy thông tin người dùng nếu cần
        public ICollection<Group_Type> group_Types { get; set; }
    }
}
