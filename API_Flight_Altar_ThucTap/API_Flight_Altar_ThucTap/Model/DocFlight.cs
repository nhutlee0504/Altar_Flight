using API_Flight_Altar.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Flight_Altar_ThucTap.Model
{
	public class DocFlight
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int IdDocument { get; set; }
		public string? DocumentName { get; set; }
		public byte[]? FileContent { get; set; }
		public double? Version { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime UpdatedTime { get; set;}
		public string? Status { get; set; }
		public string? Signature { get; set; }
		public bool? IsDeleted { get; set; }
		public int UserId { get; set; }
		public User? User { get; set; }
		public int FlightId { get; set; }
		public Flight? Flight { get; set; }
		public int TypeId { get; set; }
		public TypeDoc TypeDoc { get; set; }
	}
}
