using AutoMapper;
using ProjectManagement.BLL.Interface;
using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Exceptions;
using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.Entities;
using ProjectManagement.DAL.IRepositories;

namespace ProjectManagement.BLL.Service
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IProjectTeamRepository _projectTeamRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public PersonService(
            IPersonRepository personRepository,
            IProjectTeamRepository projectTeamRepository,
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IMapper mapper)
        {
            _personRepository = personRepository;
            _projectTeamRepository = projectTeamRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PersonDTO>> GetAllPersonsAsync()
        {
            var persons = await _personRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
        }

        public async Task<PersonDTO> GetPersonByIdAsync(string personId)
        {
            if (string.IsNullOrWhiteSpace(personId))
                throw new BadRequestException("Person ID is required.");

            var person = await _personRepository.GetByIdAsync(personId);
            if (person == null)
                throw new NotFoundException($"Person with ID {personId} not found.");

            return _mapper.Map<PersonDTO>(person);
        }

        public async Task<PersonDTO> CreatePersonAsync(PersonCreateDTO dto)
        {
            var nextId = await GenerateNextPersonIdAsync(dto.Role);

            var person = _mapper.Map<Person>(dto);
            person.PersonId = nextId;
            person.CreatedAt = DateTime.UtcNow;

            await _personRepository.AddAsync(person);
            await _personRepository.SaveChangesAsync();

            return _mapper.Map<PersonDTO>(person);
        }

        public async Task<List<PersonDTO>> CreatePersonsBulkAsync(
            List<PersonCreateDTO> dtoList
        )
        {
            var personsToAdd = new List<Person>();

            var groupedByRole = dtoList.GroupBy(d => d.Role);

            foreach (var group in groupedByRole)
            {
                var role = group.Key;

                var lastPerson = await _personRepository.GetLastPersonByRoleAsync(role);

                int startNumber = 1;
                char prefix = role switch
                {
                    RoleEnum.Admin => 'A',
                    RoleEnum.Manager => 'M',
                    RoleEnum.User => 'U',
                    _ => throw new BadRequestException("Invalid role")
                };

                if (lastPerson != null)
                {
                    startNumber = int.Parse(lastPerson.PersonId.Substring(1)) + 1;
                }

                foreach (var dto in group)
                {
                    var person = _mapper.Map<Person>(dto);
                    person.PersonId = $"{prefix}{startNumber:D3}";
                    person.CreatedAt = DateTime.UtcNow;

                    personsToAdd.Add(person);
                    startNumber++;
                }
            }

            await _personRepository.AddRangeAsync(personsToAdd);
            await _personRepository.SaveChangesAsync();

            return _mapper.Map<List<PersonDTO>>(personsToAdd);
        }

        private async Task<string> GenerateNextPersonIdAsync(RoleEnum role)
        {
            var lastPerson = await _personRepository.GetLastPersonByRoleAsync(role);

            char prefix = role switch
            {
                RoleEnum.Admin => 'A',
                RoleEnum.Manager => 'M',
                RoleEnum.User => 'U',
                _ => throw new BadRequestException("Invalid role")
            };

            int nextNumber = lastPerson == null
                ? 1
                : int.Parse(lastPerson.PersonId.Substring(1)) + 1;

            return $"{prefix}{nextNumber:D3}";
        }

        public async Task UpdatePersonAsync(string personId, PersonUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(personId))
                throw new BadRequestException("Person ID is required.");

            var person = await _personRepository.GetByIdAsync(personId);
            if (person == null)
                throw new NotFoundException($"Person with ID {personId} not found.");

            // ✅ Check for Role Change (Promotion/Demotion)
            if (person.Role != dto.Role)
            {
                // 1. Generate New Role-Based ID
                string newId = await GenerateNextPersonIdAsync(dto.Role);

                // 2. Prepare migration of related data
                // Update ProjectTasks
                var tasks = await _taskRepository.GetTasksByPersonAsync(personId);
                foreach (var task in tasks)
                {
                    task.AssignedToPersonId = newId;
                    _taskRepository.Update(task);
                }

                // Update Projects (if they were created by this Admin)
                var projects = await _projectRepository.GetProjectsByAdminAsync(personId);
                foreach (var project in projects)
                {
                    project.CreatedByAdminId = newId;
                    _projectRepository.Update(project);
                }

                // Update ProjectTeams memberships
                var memberships = await _projectTeamRepository.GetProjectsByPersonAsync(personId);
                foreach (var membership in memberships)
                {
                    membership.PersonId = newId;
                    _projectTeamRepository.Update(membership);
                }

                // 3. Create New Person record with new ID
                var newPerson = new Person
                {
                    PersonId = newId,
                    Name = dto.Name,
                    Role = dto.Role,
                    IsActive = dto.IsActive,
                    CreatedAt = person.CreatedAt // Maintain history
                };

                // 4. Atomic operation: Add new, delete old
                await _personRepository.AddAsync(newPerson);
                _personRepository.Remove(person);
            }
            else
            {
                // ✅ Standard update (No Role Change)
                _mapper.Map(dto, person);
                _personRepository.Update(person);
            }

            await _personRepository.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(string personId)
        {
            if (string.IsNullOrWhiteSpace(personId))
                throw new BadRequestException("Person ID is required.");

            var person = await _personRepository.GetByIdAsync(personId);
            if (person == null)
                throw new NotFoundException($"Person with ID {personId} not found.");

            _personRepository.Remove(person);
            await _personRepository.SaveChangesAsync();
        }

        public async Task<PagedResponse<PersonDTO>> GetPersonsPagedAsync(PagedRequest request)
        {
            var pagedPersons = await _personRepository.GetPersonsPagedAsync(request);

            return new PagedResponse<PersonDTO>
            {
                Data = _mapper.Map<IEnumerable<PersonDTO>>(pagedPersons.Data),
                PageNumber = pagedPersons.PageNumber,
                PageSize = pagedPersons.PageSize,
                TotalRecords = pagedPersons.TotalRecords
            };
        }

        public async Task<PersonTeamResponseDTO> GetTeamDetailsAsync(
            string requesterId,
            string projectTeamId
        )
        {
            var requester = await _personRepository.GetByIdAsync(requesterId);
            if (requester == null)
                throw new NotFoundException($"Requester with ID {requesterId} not found.");

            if (requester.Role != RoleEnum.Manager && requester.Role != RoleEnum.Admin)
                throw new UnauthorizedAccessException("Only Managers and Admins can access team details.");

            var projectTeam = await _projectTeamRepository.GetProjectTeamByIdAsync(projectTeamId);

            if (projectTeam == null)
                return null;

            var teamMembers = await _projectTeamRepository.GetTeamMembersByProjectAsync(projectTeam.ProjectId);
            var personIds = teamMembers.Select(pt => pt.PersonId).ToList();

            return new PersonTeamResponseDTO
            {
                ProjectTeamId = projectTeam.ProjectTeamId,
                PersonId = personIds
            };
        }
    }
}
