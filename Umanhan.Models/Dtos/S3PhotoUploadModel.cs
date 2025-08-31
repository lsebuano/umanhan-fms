using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Dtos
{
    public class S3PhotoUploadDto
    {
        public Guid ActivityId { get; set; }
        public string S3ObjectKey { get; set; }
        public string S3ObjectContentType { get; set; }
        public string Notes { get; set; }
    }
}
