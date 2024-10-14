using API_Flight_Altar.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_Flight_Altar_ThucTap.Model
{
    public class Group_User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdGU { get; set; }
        public int UserID { get; set; }
        [JsonIgnore] // Tránh chu kỳ khi serialize
        public User User { get; set; }
        public int GroupID { get; set; }
        [JsonIgnore] // Tránh chu kỳ khi serialize
        public GroupModel Group { get; set; }
    }
}
