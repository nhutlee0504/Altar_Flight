using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
    public interface IGroupUser
    {
        public Task<IEnumerable<Group_User>> GetAllGroupUser();
        public Task<IEnumerable<Group_User>> GetGroupUserById(int id);
        public Task<Group_User> AddGroupUser(int GroupID, int UserID);
        public Task<Group_User> DeleteGroupUser(int id);
    }
}
