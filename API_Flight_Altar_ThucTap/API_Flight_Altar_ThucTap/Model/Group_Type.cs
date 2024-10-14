using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_Flight_Altar_ThucTap.Model
{
    public class Group_Type
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdGT { get; set; }
        public int IdGroup { get; set; }
        [JsonIgnore] // Tránh chu kỳ khi serialize
        public GroupModel GroupModel { get; set; }
        public int IdType { get; set; }
        [JsonIgnore] // Tránh chu kỳ khi serialize
        public TypeDoc TypeDoc { get; set; }
        public int IdPermission { get; set; }
        [JsonIgnore] // Tránh chu kỳ khi serialize
        public Permission Permission { get; set; }
    }
}
