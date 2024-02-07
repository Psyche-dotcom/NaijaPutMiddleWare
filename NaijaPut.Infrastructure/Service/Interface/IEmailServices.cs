using NaijaPut.Core.DTO.Others;

namespace NaijaPut.Infrastructure.Service.Interface
{
    public interface IEmailServices
    {
        void SendEmail(Message message);
    }
}
