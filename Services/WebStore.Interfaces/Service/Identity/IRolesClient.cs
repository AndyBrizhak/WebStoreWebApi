using Microsoft.AspNetCore.Identity;
using WebStore.Domain.Entities.Identity;

namespace WebStore.Interfaces.Service.Identity
{
    public interface IRolesClient : IRoleStore<Role> { }
}