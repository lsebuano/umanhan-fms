using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Dtos
{
    public class FarmActivityPhotoDto
    {
        public Guid PhotoId { get; set; }

        public Guid ActivityId { get; set; }

        public string PhotoUrlThumbnail { get; set; }

        public string PhotoUrlFull { get; set; }

        public string MimeType { get; set; }

        public string Notes { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
