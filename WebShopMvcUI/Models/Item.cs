using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShopMvcUI.Models
{
    [Table("Item")]
    public class Item
    {
        public Item()
        {
            OrderDetail = new List<OrderDetail>();
            CartDetail = new List<CartDetail>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? ItemName { get; set; }
        
        [Required]
        [MaxLength(40)]
        public string? AuthorName { get; set; }
        
        [Required]
        public double Price { get; set; }
        public string? Image { get; set; }

        [Required]
        public int GenreId { get; set; }

        public Genre? Genre { get; set; }
        
        public List<OrderDetail> OrderDetail { get; set; }
        
        public List<CartDetail> CartDetail { get; set; }

        [NotMapped]
        public string? GenreName { get; set; }
    }
}
