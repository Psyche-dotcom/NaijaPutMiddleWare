using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaijaPut.Core.Entities
{
    public class WalletTransaction : BaseEntity
    {
        public string Narration { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public Wallet Wallet { get; set; }
        public string WalletId { get; set; }
    }
}
