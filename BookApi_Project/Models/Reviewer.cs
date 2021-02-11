using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Models
{
    public class Reviewer
    {
        [Key] // primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // to give the id value from the DB . (We don't need to use it.)
        public int Id { get; set; }

        [Required]
        [MaxLength(100,ErrorMessage ="First name cannot be more than 100 characters")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "LAst name cannot be more than 100 characters")]
        public string LastName { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
