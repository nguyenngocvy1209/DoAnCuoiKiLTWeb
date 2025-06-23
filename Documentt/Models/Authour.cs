namespace Documentt.Models
{
    public class Authour
    {
        public int AuthourId { get; set; }
        public string Img { get; set; }
        public string AuthourName { get; set; }
        public string Detail { get; set; }
        public bool noibat { get; set; }
        public List<Product>? Products { get; set; }
    }
}
