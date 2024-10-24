using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Flight_Altar_ThucTap.Model
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idPermission { get; set; }
        [Required, Column(TypeName = "varchar(50)")]
        public string PermissionName { get; set; }
        public ICollection<Group_Type>? group_Types { get; set; }
    }
}
