using Business.Domain.Models.Others;

namespace Business.Domain.Interfaces.Repositories
{
    public interface ILogRequestRepository
    {
        Task Post(LogRequest log);
    }
}
