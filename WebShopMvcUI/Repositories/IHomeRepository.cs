namespace WebShopMvcUI
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Item>> GetItems(string sTerm = "", int genreId = 0);
        Task<IEnumerable<Genre>> Genres();

    }
}