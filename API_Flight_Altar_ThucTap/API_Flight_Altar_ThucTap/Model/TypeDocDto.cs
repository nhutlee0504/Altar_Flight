using System.ComponentModel.DataAnnotations;

namespace API_Flight_Altar_ThucTap.Model
{
    public class TypeDocDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên loại tài liệu")]
        public string TypeDocName { get; set; }
    }
}
