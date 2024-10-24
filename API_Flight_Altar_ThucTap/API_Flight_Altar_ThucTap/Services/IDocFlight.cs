using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
	public interface IDocFlight
	{
		public Task<IEnumerable<DocFlight>> GetAllDocFlight();
		public Task<IEnumerable<DocFlight>> GetDocFlightByIdFlight(int idFlight);
		public Task<DocFlight> AddDocFlight(IFormFile formFile, int Flightid, int Typeid);
		public Task<DocFlight> UpdateDocFlight(int idDocFlight, IFormFile formFile);
        public Task<DocFlightDto> DownloadDocFlight(int idDocument);
        public Task<IEnumerable<DocFlight>> SearchDocFlight(string searchTerm);
		public Task<DocFlight> RemoveFlight(int idFlight);
        public Task<DocFlight> ConfirmDocFlight(int idDocument);
    }
}
