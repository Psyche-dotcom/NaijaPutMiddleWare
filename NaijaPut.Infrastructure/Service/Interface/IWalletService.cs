using NaijaPut.Core.DTO.Account;
using NaijaPut.Core.DTO.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaijaPut.Infrastructure.Service.Interface
{
    public interface IWalletService
    {
        Task<ResponseDto<string>> AddFund(AddFundDto addFundDto);
        Task<ResponseDto<string>> RemoveFund(RemoveFundDto deductFundDto);
        Task<ResponseDto<WalletResponseDto>> GetUserWallet(string userId);
        Task<ResponseDto<IEnumerable<WalletTransactionResponseDto>>> GetUserWalletTransaction(string userId);

    }
}
