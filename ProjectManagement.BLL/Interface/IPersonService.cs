using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Pagination;

namespace ProjectManagement.BLL.Interface
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonDTO>> GetAllPersonsAsync();
        Task<PersonDTO> GetPersonByIdAsync(string personId);

        Task<PersonDTO> CreatePersonAsync(PersonCreateDTO dto);

        Task<List<PersonDTO>> CreatePersonsBulkAsync(List<PersonCreateDTO> dtoList);

        Task UpdatePersonAsync(string personId, PersonUpdateDTO dto);
        Task DeletePersonAsync(string personId);
        Task<PagedResponse<PersonDTO>> GetPersonsPagedAsync(PagedRequest request);

        Task<PersonTeamResponseDTO> GetTeamDetailsAsync(
            string requesterId,
            string projectTeamId
        );
    }
}
