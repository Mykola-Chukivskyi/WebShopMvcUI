using Microsoft.AspNetCore.Mvc;
using WebShopMvcUI.Models;
using WebShopMvcUI.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly IItemRepository _itemRepository;
    private readonly ApplicationDbContext _context;
    private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

    public AdminController(IItemRepository itemRepository, ApplicationDbContext context)
    {
        _itemRepository = itemRepository;
        _context = context;
    }

    public IActionResult ManageItem()
    {
        var items = _itemRepository.GetAllItems();
        return View(items);
    }

    public IActionResult CreateItem()
    {
        ViewBag.Genres = GetGenresSelectList();
        ViewBag.NewItemId = _itemRepository.GetNextItemId();
        return View(new Item { Id = ViewBag.NewItemId });
    }

    [HttpPost]
    public IActionResult CreateItem(Item item, IFormFile Image)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (Image != null && Image.Length > 0)
                {
                    var filePath = Path.Combine(_imagePath, Image.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                    item.Image = Image.FileName;
                }
                _itemRepository.AddItem(item);
                TempData["SuccessMessage"] = "Item added successfully!";
                return RedirectToAction("ManageItem");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error adding item: " + ex.Message);
            }
        }
        ViewBag.Genres = GetGenresSelectList();
        ViewBag.NewItemId = _itemRepository.GetNextItemId();
        return View(item);
    }

    public IActionResult EditItem(int id)
    {
        var item = _itemRepository.GetItemById(id);
        if (item == null)
        {
            return NotFound();
        }
        ViewBag.Genres = GetGenresSelectList();
        return View(item);
    }

    [HttpPost]
    public IActionResult EditItem(Item item, IFormFile Image)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (Image != null && Image.Length > 0)
                {
                    var filePath = Path.Combine(_imagePath, Image.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                    item.Image = Image.FileName;
                }
                else
                {
                    // Use the existing image if no new image is uploaded
                    var existingItem = _itemRepository.GetItemById(item.Id);
                    if (existingItem != null)
                    {
                        item.Image = existingItem.Image;
                    }
                }
                _itemRepository.UpdateItem(item);
                TempData["SuccessMessage"] = "Item updated successfully!";
                return RedirectToAction("ManageItem");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating item: " + ex.Message);
            }
        }
        ViewBag.Genres = GetGenresSelectList();
        return View(item);
    }

    public IActionResult DeleteItem(int id)
    {
        var item = _itemRepository.GetItemById(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost, ActionName("DeleteItem")]
    public IActionResult DeleteItemConfirmed(int id)
    {
        try
        {
            _itemRepository.DeleteItem(id);
            TempData["SuccessMessage"] = "Item deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error deleting item: " + ex.Message;
        }
        return RedirectToAction("ManageItem");
    }

    private IEnumerable<SelectListItem> GetGenresSelectList()
    {
        return _context.Genres
            .Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.GenreName
            })
            .ToList();
    }

    // GET: Admin/OrderList
    public async Task<IActionResult> OrderList()
    {
        var orders = await _context.Orders.Include(o => o.OrderStatus).ToListAsync();
        return View(orders);
    }

    // GET: Admin/EditOrderStatus/5
    public async Task<IActionResult> EditOrderStatus(int id)
    {
        var order = await _context.Orders.Include(o => o.OrderStatus).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            TempData["ErrorMessage"] = "Order not found.";
            return RedirectToAction(nameof(OrderList));
        }

        ViewBag.OrderStatuses = new SelectList(await _context.OrderStatuses.ToListAsync(), "Id", "StatusName");
        return View(order);
    }

    // POST: Admin/EditOrderStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditOrderStatus(int id, [Bind("Id,UserId,CreateDate,OrderStatusId,IsDeleted")] Order order)
    {
        if (id != order.Id)
        {
            TempData["ErrorMessage"] = "Order ID mismatch.";
            return RedirectToAction(nameof(OrderList));
        }

        var existingOrder = await _context.Orders.FindAsync(id);
        if (existingOrder == null)
        {
            TempData["ErrorMessage"] = "Order not found.";
            return RedirectToAction(nameof(OrderList));
        }

        if (ModelState.IsValid)
        {
            try
            {
                existingOrder.OrderStatusId = order.OrderStatusId;
                _context.Update(existingOrder);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order status updated successfully.";
                return RedirectToAction(nameof(OrderList));
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Error updating order status: {ex.Message}";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "Invalid data.";
        }

        ViewBag.OrderStatuses = new SelectList(await _context.OrderStatuses.ToListAsync(), "Id", "StatusName");
        return View(order);
    }

    // GET: Admin/DeleteOrder/5
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.Include(o => o.OrderStatus).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            TempData["ErrorMessage"] = "Order not found.";
            return RedirectToAction(nameof(OrderList));
        }
        return View(order);
    }

    // POST: Admin/DeleteOrder/5
    [HttpPost, ActionName("DeleteOrder")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            TempData["ErrorMessage"] = "Order not found.";
            return RedirectToAction(nameof(OrderList));
        }

        try
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Order deleted successfully.";
        }
        catch (DbUpdateException ex)
        {
            TempData["ErrorMessage"] = $"Error deleting order: {ex.Message}";
        }

        return RedirectToAction(nameof(OrderList));
    }
}
