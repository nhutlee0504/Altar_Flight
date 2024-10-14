using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
    public interface IPermission
    {
        public Task<IEnumerable<Permission>> GetPermission();
        public Task<Permission> AddPermission(string namePermission);
        public Task<Permission> DeletePermission(int id);
    }
}
