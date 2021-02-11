using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Models
{
    public class Country
    {
        [Key] // primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // to give the id value from the DB . (We don't need to use it
        public int Id { get; set; }
        [Required]
        [MaxLength(50,ErrorMessage ="County name cann't be more than 50 characters")]
        public string Name{ get; set; }
        public virtual ICollection<Author> Authors { get; set; } // virtual
    }
}
