using NaijaPut.Core.DTO.Account;
using NaijaPut.Core.DTO.Others;

namespace NaijaPut.Infrastructure.Service.Interface
{
    public interface IAccountService
    {
        Task<ResponseDto<string>> RegisterUser(SignUp signUp, string Role);

        Task<ResponseDto<LoginResultDto>> LoginUser(SignInModel signIn);
        Task<ResponseDto<string>> ForgotPassword(string UserEmail);
        Task<ResponseDto<string>> ConfirmEmailAsync(int token, string email);

        Task<ResponseDto<string>> ResetUserPassword(ResetPassword resetPassword);
    }
}
