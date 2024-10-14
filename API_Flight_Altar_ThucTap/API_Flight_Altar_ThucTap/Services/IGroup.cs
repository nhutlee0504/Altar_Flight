using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
    public interface IGroup
    {
        public Task<IEnumerable<GroupModel>> GetGroups();
        public Task<GroupModel> CreateGroup(string groupName, string note);
        public Task<GroupModel> UpdateGroup(int id, string groupName, string note);
        public Task<GroupModel> DeleteGroup(int id);
        public Task<GroupModel> FindGroupByName(string name);
    }
}
