using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities
{
    public partial class FarmGeneralExpenseReceipt : IEntity
    {
        [Column("ReceiptId")]
        public Guid Id { get; set; }

        public Guid GeneralExpenseId { get; set; }

        public string ReceiptUrlThumbnail { get; set; }

        public string ReceiptUrlFull { get; set; }

        public string MimeType { get; set; }

        public string Notes { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual FarmGeneralExpense FarmGeneralExpense { get; set; }
    }
}
