using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NextGenRobotics.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, Display(Name = "Category Name")]
        public string   Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        // Navigation property to link products to categories
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}
