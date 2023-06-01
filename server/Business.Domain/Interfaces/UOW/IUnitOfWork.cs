namespace Business.Domain.Interfaces.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> Commit();
    }
}
