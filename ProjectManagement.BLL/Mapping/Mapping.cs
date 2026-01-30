using AutoMapper;
using ProjectManagement.DAL.Entities;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.BLL.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ===== Person mappings =====
            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<PersonCreateDTO, Person>();
            CreateMap<PersonUpdateDTO, Person>();

            // ===== Project mappings =====
            CreateMap<Project, ProjectDTO>().ReverseMap();
            CreateMap<ProjectCreateDTO, Project>();
            CreateMap<ProjectUpdateDTO, Project>();

            // ===== Task mappings =====
            CreateMap<ProjectTask, TaskDTO>().ReverseMap();
            CreateMap<TaskCreateDTO, ProjectTask>();
            CreateMap<TaskUpdateDTO, ProjectTask>();

        }
    }
}
