using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string TemporaryPassword { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailVerified { get; set; }

        public List<RoleDto> Roles { get; set; } = new();
    }
}
