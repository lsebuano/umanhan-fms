using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities
{
    public partial class ChangeLog : IEntity
    {
        [Column("ChangeId")]
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Username { get; set; }

        public string Type { get; set; }

        public string Entity { get; set; }

        public string EntityId { get; set; }

        public string Field { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}
