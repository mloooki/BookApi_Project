using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Dtos
{
    public class CountryDto // DTO = Data Transfer Object. this cllas to choose the property that we want to send it through API (in this example we remove authors BC it's always null).
    {
        public int Id { get; set; }
        public string Name{ get; set; }
    }
}
