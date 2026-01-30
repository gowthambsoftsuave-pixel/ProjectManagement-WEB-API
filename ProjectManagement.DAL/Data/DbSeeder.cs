using ProjectManagement.Common.Enums;
using ProjectManagement.DAL.Entities;

namespace ProjectManagement.DAL.Data
{
    public static class DbSeeder
    {
        public static void SeedAdmin(ProjectManagementDbContext context)
        {
            if (!context.Persons.Any(p => p.Role == RoleEnum.Admin))
            {
                var admin = new Person
                {
                    PersonId = "A001",
                    Name = "Super Admin",
                    Role = RoleEnum.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                context.Persons.Add(admin);
                context.SaveChanges();
            }
        }
    }
}
