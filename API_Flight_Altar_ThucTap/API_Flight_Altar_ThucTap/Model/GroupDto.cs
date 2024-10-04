using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_Flight_Altar_ThucTap.Model
{
    public class GroupDto
    {
        public string GroupName { get; set; }
        public string Note { get; set; }
    }
}
