using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
    public interface IGroup
    {
        public Task<IEnumerable<GroupModel>> GetGroups();
        public Task<IEnumerable<GroupModel>> GetMyGroup();
        public Task<GroupModel> CreateGroup(string groupName, string note);
        public Task<GroupModel> UpdateGroup(int id, string groupName, string note);
        public Task<GroupModel> DeleteGroup(int id);
        public Task<IEnumerable<GroupModel>> FindGroupByName(string name);
        public Task<GroupModel> FindMyGroupById(int idGroup);
    }
}
