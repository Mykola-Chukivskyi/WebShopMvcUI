using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShopMvcUI.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double UnitPrice { get; set; }
        public Item Item { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
