using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace WebShopMvcUI.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Item>> GetItems(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Item> items = await (from item in _db.Items
                                             join genre in _db.Genres
                                             on item.GenreId equals genre.Id
                                             where string.IsNullOrWhiteSpace(sTerm) || (item != null && item.ItemName.ToLower().StartsWith(sTerm))
                                             select new Item
                                             {
                                                 Id = item.Id,
                                                 Image = item.Image,
                                                 AuthorName = item.AuthorName,
                                                 ItemName = item.ItemName,
                                                 GenreId = item.GenreId,
                                                 Price = item.Price,
                                                 GenreName = genre.GenreName
                                             }
                                             ).ToListAsync();
            if (genreId > 0)
            {

                items = items.Where(a => a.GenreId == genreId).ToList();
            }
            return items;

        }
    }
}
