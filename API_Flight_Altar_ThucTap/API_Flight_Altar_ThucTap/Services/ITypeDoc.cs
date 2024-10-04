using API_Flight_Altar_ThucTap.Model;

namespace API_Flight_Altar_ThucTap.Services
{
    public interface ITypeDoc
    {
        public Task<IEnumerable<TypeDoc>> GetTypeDocs();
        public Task<TypeDoc> AddTypeDoc(TypeDocDto typeDocDto);
        public Task<TypeDoc> UpdateTypeDoc(int id, TypeDocDto typeDocDto);
    }
}
