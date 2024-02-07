using AutoMapper;
using NaijaPut.Core.DTO.Others;
using NaijaPut.Core.Entities;

namespace Naijaput.Api.ProjectProfileMapping
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<WalletTransaction,AddFundDto>().ReverseMap();
            CreateMap<WalletTransaction, RemoveFundDto>().ReverseMap();
            CreateMap<WalletTransaction, WalletTransactionResponseDto>().ReverseMap();
        }
    }
}
