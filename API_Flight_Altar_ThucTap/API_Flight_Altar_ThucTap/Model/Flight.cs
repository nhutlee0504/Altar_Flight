using API_Flight_Altar.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Flight_Altar_ThucTap.Model
{
	public class Flight
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int IdFlight { get; set; }
		public string? FlightNo { get; set; }
		public DateTime DateTime { get; set; }
		public string? PointOfLoading { get; set; }
		public string? PointOfUnloading { get; set; }
		public string? TimeStart { get; set; }
		public string? TimeEnd { get; set; }
		public string? Status { get; set; }
		public bool? IsDelete { get; set; }
		public int UserId { get; set; }
		public User? User { get; set; }
		public ICollection<DocFlight> documents {  get; set; }
	}
}
