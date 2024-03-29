﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NaijaPut.Core.DTO.Account;
using NaijaPut.Core.DTO.Others;
using NaijaPut.Core.Entities;
using NaijaPut.Core.Repository.Interface;
using NaijaPut.Infrastructure.Service.Interface;

namespace NaijaPut.Infrastructure.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly ILogger<AccountService> _logger;
        private readonly IGenerateJwt _generateJwt;
        private readonly INaijaPutRepository<Wallet> _walletContext;
        private readonly IEmailServices _emailServices;
        private readonly IMapper _mapper;
        public AccountService(IAccountRepo accountRepo, ILogger<AccountService> logger, IEmailServices emailServices, IGenerateJwt generateJwt, INaijaPutRepository<Wallet> walletContext, IMapper mapper)
        {
            _accountRepo = accountRepo;
            _logger = logger;
            _generateJwt = generateJwt;
            _walletContext = walletContext;
            _emailServices = emailServices;
            _mapper = mapper;
        }
        public async Task<ResponseDto<string>> RegisterUser(SignUp signUp, string Role)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByEmailAsync(signUp.Email);
                if (checkUserExist != null)
                {
                    response.ErrorMessages = new List<string>() { "User with the email already exist" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkRole = await _accountRepo.RoleExist(Role);
                if (checkRole == false)
                {
                    response.ErrorMessages = new List<string>() { "Role is not available" };
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapAccount = new ApplicationUser();
                mapAccount.Email = signUp.Email;
                mapAccount.PhoneNumber = signUp.PhoneNumber;
                mapAccount.UserName = signUp.UserName;
                mapAccount.LastName = signUp.LastName;
                mapAccount.FirstName = signUp.FirstName;

                var createUser = await _accountRepo.SignUpAsync(mapAccount, signUp.Password);
                if (createUser == null)
                {
                    response.ErrorMessages = new List<string>() { "User not created successfully" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var addRole = await _accountRepo.AddRoleAsync(createUser, Role);
                if (addRole == false)
                {
                    response.ErrorMessages = new List<string>() { "Fail to add role to user" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }

                var GenerateConfirmEmailToken = new ConfirmEmailToken()
                {
                    Token = _accountRepo.GenerateConfirmEmailToken(),
                    UserId = createUser.Id
                };
                var createwallet = new Wallet();
                createwallet.UserId = createUser.Id;
                await _walletContext.Add(createwallet);
                var walletStatus = await _walletContext.SaveChanges();
                if (walletStatus < 1)
                {
                    response.ErrorMessages = new List<string>() { "Wallet not created successfully" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var Generatetoken = await _accountRepo.SaveGenerateConfirmEmailToken(GenerateConfirmEmailToken);
                if (Generatetoken == null)
                {
                    response.ErrorMessages = new List<string>() { "Fail to generate confirm email token for user" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }

                var message = new Message(new string[] { createUser.Email }, "Confirm Email Token", $"<p>Your confirm email code is below<p><h6>{GenerateConfirmEmailToken.Token}</h6>");
                _emailServices.SendEmail(message);
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Successful";
                response.Result = "User successfully created";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in resgistering the user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> SuspendUserAsync(string useremail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(useremail);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                findUser.SuspendUser = true;
                var updateUser = await _accountRepo.UpdateUserInfo(findUser);
                if (updateUser == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in suspending user" };
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.DisplayMessage = "Error";
                    return response;
                }

                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully suspend user";
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in suspending user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> UnSuspendUserAsync(string useremail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(useremail);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                findUser.SuspendUser = false;
                var updateUser = await _accountRepo.UpdateUserInfo(findUser);
                if (updateUser == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in unsuspending user" };
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully unsuspend user";
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in unsuspending user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<LoginResultDto>> LoginUser(SignInModel signIn)
        {
            var response = new ResponseDto<LoginResultDto>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByEmailAsync(signIn.Email);
                if (checkUserExist == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                if (checkUserExist.SuspendUser == true)
                {
                    response.ErrorMessages = new List<string>() { "User is suspended, contact admin" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkPassword = await _accountRepo.CheckAccountPassword(checkUserExist, signIn.Password);
                if (checkPassword == false)
                {
                    response.ErrorMessages = new List<string>() { "Invalid Password" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkEmailConfirm = await _accountRepo.CheckEmailConfirmed(checkUserExist);
                if (checkEmailConfirm == false)
                {
                    response.ErrorMessages = new List<string>() { "Email not yet confirm, pls confirm your email" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                var generateToken = await _generateJwt.GenerateToken(checkUserExist);
                if (generateToken == null)
                {
                    response.ErrorMessages = new List<string>() { "Error in generating jwt for user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }

                var getUserRole = await _accountRepo.GetUserRoles(checkUserExist);
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Successfully login";
                response.Result = new LoginResultDto() { Jwt = generateToken, UserRole = getUserRole };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in login the user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> ForgotPassword(string UserEmail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUser = await _accountRepo.FindUserByEmailAsync(UserEmail);
                if (checkUser == null)
                {
                    response.ErrorMessages = new List<string>() { "Email is not available" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var result = await _accountRepo.ForgotPassword(checkUser);
                if (result == null)
                {
                    response.ErrorMessages = new List<string>() { "Error in generating reset token for user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var message = new Message(new string[] { checkUser.Email }, "Reset Password Code", $"<p>Your reset password code is below<p><br/><h6>{result}</h6><br/> <p>Please use it in your reset password endpoint</p>");
                _emailServices.SendEmail(message);
                response.DisplayMessage = "Token generated Successfully";
                response.Result = result;
                response.StatusCode = 200;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in generating reset token for user" };
                response.StatusCode = 501;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> ResetUserPassword(ResetPassword resetPassword)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(resetPassword.Email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var resetPasswordAsync = await _accountRepo.ResetPasswordAsync(findUser, resetPassword);
                if (resetPasswordAsync == null)
                {
                    response.ErrorMessages = new List<string>() { "Invalid token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully reset user password";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in reset user password" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<DisplayUserWithRoleDto>> GetUserFullDetails(string userid)
        {
            var response = new ResponseDto<DisplayUserWithRoleDto>();
            try
            {

                var findUser = await _accountRepo.FindUserByIdAsync(userid);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var retrieveUserDetails = await _accountRepo.GetUserWithDetails(userid);
                var getUserRole = await _accountRepo.GetUserRoles(findUser);
                if(getUserRole != null)
                {
                    retrieveUserDetails.UserRole = getUserRole[0];
                }
                
                response.Result = retrieveUserDetails;
                response.StatusCode = 200;
                response.DisplayMessage = "Successful";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in retrieving user details" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> DeleteUser(string email)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var deleteUser = await _accountRepo.DeleteUserByEmail(findUser);
                if (deleteUser == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in deleting user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully delete user";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in deleting user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> UpdateUser(string email, UpdateUserDto updateUser)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapUpdateDetails = _mapper.Map(updateUser, findUser);
                var updateUserDetails = await _accountRepo.UpdateUserInfo(mapUpdateDetails);
                if (updateUserDetails == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in updating user info" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully update user information";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in updating user info" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> ConfirmEmailAsync(int token, string email)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var retrieveToken = await _accountRepo.retrieveUserToken(findUser.Id);
                if (retrieveToken == null)
                {
                    response.ErrorMessages = new List<string>() { "Error user token token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                if (retrieveToken.Token != token)
                {
                    response.ErrorMessages = new List<string>() { "Invalid user token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                var deleteToken = await _accountRepo.DeleteUserToken(retrieveToken);
                if (deleteToken == false)
                {
                    response.ErrorMessages = new List<string>() { "Error removing user token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                findUser.EmailConfirmed = true;
                var updateUserConfirmState = await _accountRepo.UpdateUserInfo(findUser);
                if (updateUserConfirmState == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in confirming user token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully comfirm user token";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in confirming user token" };
                response.StatusCode = 501;
                response.DisplayMessage = "Error";
                return response;
            }
        }
    }
}
