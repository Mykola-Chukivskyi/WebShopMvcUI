namespace WebShopMvcUI.Models.DTOs
{
    public class ItemDisplayModel
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
    }
}
