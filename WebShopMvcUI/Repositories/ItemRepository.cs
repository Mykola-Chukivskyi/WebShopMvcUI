using WebShopMvcUI.Data;
using WebShopMvcUI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Linq;

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;

    public ItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _context.Items.Include(i => i.Genre).ToList();
    }

    public Item GetItemById(int id)
    {
        return _context.Items.Include(i => i.Genre).FirstOrDefault(i => i.Id == id);
    }

    public void AddItem(Item item)
    {
        var newIdParam = new SqlParameter
        {
            ParameterName = "@NewId",
            SqlDbType = System.Data.SqlDbType.Int,
            Direction = System.Data.ParameterDirection.Output
        };

        _context.Database.ExecuteSqlRaw(
            "EXEC [dbo].[AddItem] @ItemName, @Price, @Image, @GenreId, @AuthorName, @NewId OUT",
            new SqlParameter("@ItemName", item.ItemName),
            new SqlParameter("@Price", item.Price),
            new SqlParameter("@Image", item.Image ?? (object)DBNull.Value),
            new SqlParameter("@GenreId", item.GenreId),
            new SqlParameter("@AuthorName", item.AuthorName),
            newIdParam
        );

        item.Id = (int)newIdParam.Value;
    }

    public void UpdateItem(Item item)
    {
        _context.Database.ExecuteSqlRaw(
            "EXEC [dbo].[UpdateItem] @Id, @ItemName, @Price, @Image, @GenreId, @AuthorName",
            new SqlParameter("@Id", item.Id),
            new SqlParameter("@ItemName", item.ItemName),
            new SqlParameter("@Price", item.Price),
            new SqlParameter("@Image", item.Image ?? (object)DBNull.Value),
            new SqlParameter("@GenreId", item.GenreId),
            new SqlParameter("@AuthorName", item.AuthorName)
        );
    }

    public void DeleteItem(int id)
    {
        var item = _context.Items.Find(id);
        if (item != null)
        {
            _context.Items.Remove(item);
            _context.SaveChanges();
        }
    }

    public int GetNextItemId()
    {
        return _context.Items.Any() ? _context.Items.Max(i => i.Id) + 1 : 1;
    }
}
