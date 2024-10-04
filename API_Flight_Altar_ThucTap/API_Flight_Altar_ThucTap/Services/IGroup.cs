using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
    public interface IGroup
    {
        public Task<IEnumerable<GroupModel>> GetGroups();
        public Task<GroupModel> CreateGroup(GroupDto group);
        public Task<GroupModel> UpdateGroup(int id, GroupDto group);
    }
}
