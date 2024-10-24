using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
	public interface IFlight
	{
		public Task<IEnumerable<Flight>> GetAllFlight();
		public Task<IEnumerable<Flight>> GetMyFlight();
		public Task<Flight> AddFlight(FlightInfo flightInfo);
		public Task<Flight> UpdateFlight(int idFlight, FlightInfo flightInfo);
		public Task<Flight> RemoveFlight(int idFlight);
	}
}
