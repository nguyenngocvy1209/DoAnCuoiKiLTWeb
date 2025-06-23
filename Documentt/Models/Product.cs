namespace Documentt.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public decimal Price { get; set; }
        public bool New { get; set; }
        public bool Trending { get; set; }
        public int AuthourId { get; set; }
        public Authour? Authour { get; set; }
        public string link { get; set; }
        public string img { get; set; }
        public string? img1 { get; set; }
        public string? img2 { get; set; }
        public string? img3 { get; set; }
        public string? img4 { get; set; }
        public string? img5 { get; set; }

    }
}
