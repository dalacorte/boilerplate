namespace Business.Domain.Helpers
{
    public interface IViewModel<out T>
         where T : class
    {
        T Model();
    }
}
