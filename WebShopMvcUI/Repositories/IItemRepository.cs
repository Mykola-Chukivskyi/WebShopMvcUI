using WebShopMvcUI.Models;

public interface IItemRepository
{
    IEnumerable<Item> GetAllItems();
    Item GetItemById(int id);
    void AddItem(Item item);
    void UpdateItem(Item item);
    void DeleteItem(int id);
    int GetNextItemId();
}
