using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
    public interface IGroupType
    {
        public Task<IEnumerable<Group_Type>> GetAllGroupType();
        public Task<IEnumerable<Group_Type>> GetGroupTypeByIdType(int idType);
        public Task<Group_Type> AddGroupType (int idGroup, int idType, int idPermission);
        public Task<Group_Type> UpdateGroupType(int idGT, int idGroup, int idType, int idPermission);
        public Task<Group_Type> DeleteGroupType(int idGT);
    }
}
