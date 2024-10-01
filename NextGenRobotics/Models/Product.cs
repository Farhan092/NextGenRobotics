using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NextGenRobotics.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(20, ErrorMessage = "Product Name cannot exceed 20 characters.")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be a positive value.")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Stock Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
        public int? UnitInStock { get; set; }

        [Required(ErrorMessage = "Stock Status is required.")]
        [Display(Name = "Stock Status")]
        public StockStatus StockStatus { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 200 characters.")]
        [Display(Name = "Description")]
        
        public string Description { get; set; }

        [Display(Name = "Image")]
        [DataType(DataType.ImageUrl, ErrorMessage = "Please provide a valid image URL.")]
        public string PicturePath { get; set; }

        [Required]
        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        // Navigation properties for relationships
        [JsonIgnore]
        public virtual Category Category { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; }
    }

    public enum StockStatus
    {
        InStock = 1,
        OutOfStock = 0
    }
}
