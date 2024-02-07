using NaijaPut.Core.Entities;

namespace NaijaPut.Infrastructure.Service.Interface
{
    public interface IGenerateJwt
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}
