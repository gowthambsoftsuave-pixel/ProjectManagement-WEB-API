using AutoMapper;
using ProjectManagement.DAL.Entities;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Person mappings
            CreateMap<Person, PersonDTO>();
            CreateMap<PersonCreateDTO, Person>();
            CreateMap<PersonUpdateDTO, Person>();

            // Project mappings
            CreateMap<Project, ProjectDTO>();
            CreateMap<ProjectCreateDTO, Project>()
                .ForMember(dest => dest.CurrentSprintCount, opt => opt.MapFrom(src => 1)) // default
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))   // default
                .ForMember(dest => dest.ProjectId, opt => opt.Ignore())                  // generated in service
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());                // generated in service

            CreateMap<ProjectUpdateDTO, Project>()
                .ForMember(dest => dest.TotalSprintCount, opt => opt.Ignore()) // PUT doesn't update total sprint
                .ForMember(dest => dest.CurrentSprintCount, opt => opt.Ignore()) // handled separately
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectId, opt => opt.Ignore());
        }
    }
}
