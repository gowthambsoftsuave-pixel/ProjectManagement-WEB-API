using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.Interface;
using ProjectManagement.BLL.Service;
using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Pagination;

namespace ProjectManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = await _personService.GetAllPersonsAsync();
            return Ok(persons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonById(string id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] PersonCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Person data is required.");

            var created = await _personService.CreatePersonAsync(dto);
            return CreatedAtAction(nameof(GetPersonById), new { id = created.PersonId }, created);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreatePersonsBulk(
            [FromBody] List<PersonCreateDTO> dtoList
        )
        {
            if (dtoList == null || !dtoList.Any())
                return BadRequest("Person list is required.");

            var createdPersons = await _personService.CreatePersonsBulkAsync(dtoList);
            return Ok(createdPersons);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(string id, [FromBody] PersonUpdateDTO dto)
        {
            if (dto == null)
                return BadRequest("Person data is required.");

            await _personService.UpdatePersonAsync(id, dto);
            return Ok("Updated Successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(string id)
        {
            await _personService.DeletePersonAsync(id);
            return Ok("Deleted the Person Successfully");
        }

        [HttpGet("team/{projectTeamId}")]
        public async Task<IActionResult> GetTeamDetails(
            [FromHeader] string requesterId,
            string projectTeamId
        )
        {
            var teamDetails = await _personService.GetTeamDetailsAsync(requesterId, projectTeamId);
            if (teamDetails == null)
                return NotFound();
            return Ok(teamDetails);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetTasksPaged([FromQuery] PagedRequest request)
        {
            var result = await _personService.GetPersonsPagedAsync(request);
            return Ok(result);
        }
    }
}
