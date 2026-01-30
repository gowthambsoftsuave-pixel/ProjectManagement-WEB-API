using ProjectManagement.BLL.Interface;
using ProjectManagement.BLL.Service;
using ProjectManagement.DAL.Data;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.BLL.Interfaces;
using ProjectManagement.DAL.IRepositories;
using ProjectManagement.DAL.Repositories;

namespace ProjectManagement.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ProjectManagementDbContext>(options => 
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Register generic repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register entity-specific repositories
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IProjectTeamRepository, ProjectTeamRepository>();

            // Register services
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

    }
}
