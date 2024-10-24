using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
    public interface ITypeDoc
    {
        public Task<IEnumerable<TypeDoc>> GetTypeDocs();
        public Task<IEnumerable<TypeDoc>> GetMyTypeDoc();
        public Task<TypeDoc> AddTypeDoc(string typeName, string note);
        public Task<TypeDoc> UpdateTypeDoc(int id, string typeName, string note);
        public Task<IEnumerable<TypeDoc>> FindTypeDocByName(string name);
        public Task<TypeDoc> DeleteTypeDoc(int id);
    }
}
