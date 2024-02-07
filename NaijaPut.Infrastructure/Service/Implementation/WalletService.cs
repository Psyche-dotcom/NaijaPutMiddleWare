using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NaijaPut.Core.DTO.Others;
using NaijaPut.Core.Entities;
using NaijaPut.Core.Enum;
using NaijaPut.Core.Repository.Interface;
using NaijaPut.Infrastructure.Service.Interface;

namespace NaijaPut.Infrastructure.Service.Implementation
{
    public class WalletService : IWalletService
    {
        private readonly INaijaPutRepository<Wallet> _walletRepository;
        private readonly INaijaPutRepository<WalletTransaction> _walletTransactionRepository;
        private readonly ILogger<WalletService> _logger;
        private readonly IMapper _mapper;

        public WalletService(INaijaPutRepository<Wallet> walletRepository, ILogger<WalletService> logger, IMapper mapper, INaijaPutRepository<WalletTransaction> walletTransactionRepository)
        {
            _walletRepository = walletRepository;
            _logger = logger;
            _mapper = mapper;
            _walletTransactionRepository = walletTransactionRepository;
        }

        public async Task<ResponseDto<string>> AddFund(AddFundDto addFundDto)
        {
            var response = new ResponseDto<string>();
            using (var transaction = await _walletRepository.BeginTransaction())
            {
                try
                {
                    var wallet = await _walletRepository.GetQueryable().FirstOrDefaultAsync(u => u.UserId == addFundDto.UserId);
                    if (wallet == null)
                    {
                        response.ErrorMessages = new List<string>() { "Invalid wallet" };
                        response.StatusCode = 404;
                        response.DisplayMessage = "Error";
                        return response;
                    }

                    wallet.Balance += addFundDto.Amount;
                    _walletRepository.Update(wallet);

                    var walletTransaction = _mapper.Map<WalletTransaction>(addFundDto);
                    walletTransaction.TransactionType = TransactionStatus.Credit.ToString();
                    await _walletTransactionRepository.Add(walletTransaction);
                    await _walletRepository.SaveChanges();
                    await transaction.CommitAsync();
                    response.StatusCode = StatusCodes.Status200OK;
                    response.DisplayMessage = "Success";
                    response.Result = "Fund Added Successfully";
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    _logger.LogError(ex, "Error adding fund to wallet");
                    response.ErrorMessages = new List<string>() { "Error adding fund to wallet" };
                    response.StatusCode = 500;
                    response.DisplayMessage = "Error";
                    return response;
                }
            }
        }

        public async Task<ResponseDto<WalletResponseDto>> GetUserWallet(string userId)
        {

            var response = new ResponseDto<WalletResponseDto>();
            try
            {
                var wallet = await _walletRepository.GetQueryable().Include(u => u.User)
               .Select(u => new WalletResponseDto()
               {
                   Balance = u.Balance,
                   FirstName = u.User.FirstName,
                   LastName = u.User.LastName,
                   UserId = u.User.Id,
               }).
                FirstOrDefaultAsync(u => u.UserId == userId);
                if (wallet == null)
                {
                    response.ErrorMessages = new List<string>() { "Invalid wallet" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.Result = wallet;
                response.StatusCode = 200;
                response.DisplayMessage = "Success";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error retrieving user wallet" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }

        }

        public async Task<ResponseDto<IEnumerable<WalletTransactionResponseDto>>> GetUserWalletTransaction(string userId)
        {
            var response = new ResponseDto<IEnumerable<WalletTransactionResponseDto>>();
            try
            {
                var wallet = await _walletRepository.GetQueryable().FirstOrDefaultAsync(u => u.UserId == userId);
                if (wallet == null)
                {
                    response.ErrorMessages = new List<string>() { "Invalid wallet" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var getAllTransaction = await _walletTransactionRepository.GetQueryable().
                    Where(u => u.WalletId == wallet.Id).Select(t => new WalletTransactionResponseDto
                    {
                        Amount = t.Amount,
                        Created = t.Created,
                        Id = t.Id,
                        Narration = t.Narration,
                        TransactionType = t.TransactionType
                    }).
                    ToListAsync();
                response.Result = getAllTransaction;
                response.StatusCode = 200;
                response.DisplayMessage = "Succesful";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error retrieving user wallet transactions" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }

        }

        public async Task<ResponseDto<string>> RemoveFund(RemoveFundDto deductFundDto)
        {
            var response = new ResponseDto<string>();
            using (var transaction = await _walletRepository.BeginTransaction())
            {
                try
                {
                    var wallet = await _walletRepository.GetQueryable().FirstOrDefaultAsync(u => u.UserId == deductFundDto.UserId);
                    if (wallet == null)
                    {
                        response.ErrorMessages = new List<string>() { "Invalid wallet" };
                        response.StatusCode = 404;
                        response.DisplayMessage = "Error";
                        return response;
                    }
                    if (wallet.Balance < deductFundDto.Amount)
                    {
                        response.ErrorMessages = new List<string>() { "Withdrawal Amount exceeds Wallet Balance" };
                        response.StatusCode = 400;
                        response.DisplayMessage = "Error";
                        return response;
                    }

                    wallet.Balance -= deductFundDto.Amount;
                    _walletRepository.Update(wallet);

                    var walletTransaction = _mapper.Map<WalletTransaction>(deductFundDto);
                    walletTransaction.TransactionType = TransactionStatus.Debit.ToString();
                    await _walletTransactionRepository.Add(walletTransaction);

                    await _walletRepository.SaveChanges();
                    await transaction.CommitAsync();

                    response.StatusCode = StatusCodes.Status200OK;
                    response.DisplayMessage = "Success";
                    response.Result = "Fund Deducted Successfully";
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex.Message, ex);
                    response.ErrorMessages = new List<string>() { "Error deducting fund from wallet" };
                    response.StatusCode = 500;
                    response.DisplayMessage = "Error";
                    return response;
                }
            }
        }
    }

}
