using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaijaPut.Core.DTO.Others
{
    public class AddFundDto
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Narration {  get; set; }
    }
}
