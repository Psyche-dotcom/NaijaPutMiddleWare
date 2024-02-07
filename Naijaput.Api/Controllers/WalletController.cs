using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NaijaPut.Core.DTO.Account;
using NaijaPut.Core.DTO.Others;
using NaijaPut.Infrastructure.Service.Interface;

namespace Naijaput.Api.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }
        [HttpPost("fund/add")]
        public async Task<IActionResult> AddFundToWallet(AddFundDto AddFund)
        {
            var result = await _walletService.AddFund(AddFund);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPost("fund/deduct")]
        public async Task<IActionResult> DeductFund(RemoveFundDto RemoveFund)
        {
            var result = await _walletService.RemoveFund(RemoveFund);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("transactions/{userid}")]
        public async Task<IActionResult> WalletTransaction(string userid)
        {
            var result = await _walletService.GetUserWalletTransaction(userid);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("details/{userid}")]
        public async Task<IActionResult> GetWallet(string userid)
        {
            var result = await _walletService.GetUserWallet(userid);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
