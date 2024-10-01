using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NextGenRobotics.Views.Product
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Meassage { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
}