﻿using BookApi_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Services
{
    public class CountryRepository : ICountryRepository
    {
        private BookDbContext _countryContext;

        public CountryRepository(BookDbContext countryContext) // this is called Inject DbContext into Repository.
        {
            _countryContext = countryContext;
        }

        public bool CountryExists(int countryId)
        {
            return _countryContext.Countries.Any(c => c.Id == countryId);
        }

        public bool CreateCountry(Country country)
        {
            _countryContext.Add(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            _countryContext.Remove(country);
            return Save();
        }

        public ICollection<Author> GetAuthorsFromACountry(int countryId)
        {
            return _countryContext.Authors.Where(c => c.Country.Id == countryId).ToList();

        }

        public ICollection<Country> GetCountries()
        {
            return _countryContext.Countries.OrderBy(c => c.Name).ToList(); // order by name.
        }

        public Country GetCountry(int countryId)
        {
            return _countryContext.Countries.Where(c => c.Id == countryId).FirstOrDefault(); 

        }

        public Country GetCountryOfAnAuthor(int auhorId)
        {
            return _countryContext.Authors.Where(a => a.Id == auhorId).Select(c => c.Country).FirstOrDefault();
        }

        public bool IsDuplicateCountryName(int countryId, string countryName)
        {
            var country = _countryContext.Countries.Where(c => c.Name.Trim().ToUpper() == countryName.Trim().ToUpper() && c.Id != countryId).FirstOrDefault();
            return country == null ? false : true;
        }

        public bool Save()
        {
            var saved = _countryContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UddateCountry(Country country)
        {
            _countryContext.Update(country);
            return Save();
        }
    }
}
