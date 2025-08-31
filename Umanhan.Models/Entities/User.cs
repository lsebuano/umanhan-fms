using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities
{
    public class User : IEntity
    {
        [Column("UserId")]
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}
