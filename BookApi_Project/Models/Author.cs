using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Models
{
    public class Author
    {
        [Key] // primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // to give the id value from the DB . (We don't need to use it.)
        public int Id { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="First Name cannot be more than 100 characters")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(200, ErrorMessage = "First Name cannot be more than 200 characters")]
        public string LastName { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }


    }
}
