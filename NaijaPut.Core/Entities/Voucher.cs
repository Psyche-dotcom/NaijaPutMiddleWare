using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaijaPut.Core.Entities
{
    public class Voucher : BaseEntity
    {
        public string Status { get; set; }
        public string ClaimById { get; set; }
        public int Code { get; set; }
        public ApplicationUser ClaimBy { get; set; }
       
        public decimal TotalValue { get; set; }
    }
}
