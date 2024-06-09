using Microsoft.AspNetCore.Mvc;
using WebShopMvcUI.Models;
using WebShopMvcUI.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Http;

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
}
