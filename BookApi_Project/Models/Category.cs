using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Models
{
    public class Category
    {
        [Key] // primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // to give the id value from the DB . (We don't need to use it
        public int Id{ get; set; }
        [Required]
        [MaxLength(50,ErrorMessage ="Category name cann't be more than 50 charactrs")]
        public string Name{ get; set; }
        public virtual ICollection<BookCategory> BookCategories { get; set; }
    }
}
