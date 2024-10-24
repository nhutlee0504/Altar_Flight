using API_Flight_Altar.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Flight_Altar_ThucTap.Model
{
    public class GroupModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdGroup { get; set; }
        public string? GroupName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Note { get; set; }
        public int UserId { get; set; }
        [JsonIgnore] // Tránh chu kỳ khi serialize
        public User? User { get; set; }
        public ICollection<Group_User>? group_Users { get; set; }
        public ICollection<Group_Type>? group_Types { get; set; }
    }
}
