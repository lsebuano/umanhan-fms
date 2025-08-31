using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities
{
    public partial class FarmActivityPhoto : IEntity
    {
        [Column("PhotoId")]
        public Guid Id { get; set; }

        public Guid ActivityId { get; set; }

        public string PhotoUrlThumbnail { get; set; }

        public string PhotoUrlFull { get; set; }

        public string MimeType { get; set; }

        public string Notes { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual FarmActivity Activity { get; set; }
    }
}
