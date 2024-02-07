using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaijaPut.Core.Entities
{
    public class Wallet : BaseEntity
    {
        public decimal Balance { get; set; } = 100;
        public string UserId { get; set; }
        public List<WalletTransaction> WalletTransaction { get; set; }
        public ApplicationUser User { get; set; }
    }
}
