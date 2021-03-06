using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace finalGSA.Models
{
    public class Request
    {

        [Key]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Items { get; set; }
        // user can write down everything they need in textbox

        [FutureDate(ErrorMessage = "Can't be a past date")]
        [Required(ErrorMessage = "Please enter a date and time")]
        [DataType(DataType.Date)]
        public DateTime Urgency { get; set; }

        public string Notes { get; set; }
        public bool IsCompleted { get; set; }
        // default val = false;
        public int PickedUpByID { get; set; }
        public string CompletedBy { get; set; }
        // user picking up task/ getting id instead of name to connect to db
        public DateTime CreatedAt = DateTime.Now;
        public DateTime UpdatedAt = DateTime.Now;
        public int UserID { get; set; }
        public User Creator { get; set; }
    }
}