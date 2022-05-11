using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DirectoryApi.Entities;

namespace DirectoryApi.Repositories
{
    public interface IRolesRepository : IBaseRepository<Role>
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(Guid roleId);
        Task<Role> GetRoleByNameAsync(string name);
        Task<Guid> CreateRoleAsync(Role role);
        Task UpdateRoleAsync(Role dbRole, Role role);
        Task DeleteRoleAsync(Role role);
    }
}