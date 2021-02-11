using BookApi_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Services
{
   public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country GetCountry(int countryId);
        Country GetCountryOfAnAuthor(int auhorId);
        ICollection<Author> GetAuthorsFromACountry(int countryId);
        bool CountryExists(int countryId);
        bool IsDuplicateCountryName(int countryId,string countryName);
        bool CreateCountry(Country country);
        bool UddateCountry(Country country);
        bool DeleteCountry(Country country);
        bool Save();
    }
}
