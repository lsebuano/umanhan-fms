using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities
{
    public partial class PricingCondition : IEntity
    {
        [Column("PricingId")]
        public Guid Id { get; set; }

        public Guid ConditionTypeId { get; set; }

        public decimal Value { get; set; }

        public string ApplyType { get; set; }

        public int Sequence { get; set; }

        public Guid ProfileId { get; set; }

        public virtual PricingProfile PricingProfile { get; set; }

        public virtual PricingConditionType ConditionType { get; set; }
    }
}
