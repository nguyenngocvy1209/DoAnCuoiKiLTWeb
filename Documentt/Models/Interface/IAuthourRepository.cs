namespace Documentt.Models.Interface
{
    public interface IAuthourRepository
    {
        IEnumerable<Authour> GetAll();
        Authour GetAuthour(int aId);
    }
}
