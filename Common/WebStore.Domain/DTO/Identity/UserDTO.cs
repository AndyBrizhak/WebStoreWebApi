using Microsoft.AspNetCore.Identity;
using WebStore.Domain.Entities.Identity;

namespace WebStore.Domain.DTO.Identity
{
    public abstract class UserDTO
    {
        public User User { get; set; }
    }
}