namespace Documentt.Models.Interface
{
    public interface IHomeRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> Trending();
        IEnumerable<Product> New();
        IEnumerable<Authour> noibat();
    }
}
