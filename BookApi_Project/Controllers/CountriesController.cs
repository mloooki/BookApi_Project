using BookApi_Project.Dtos;
using BookApi_Project.Models;
using BookApi_Project.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Controllers
{
    [Route("api/[controller]")] // controller = countries. this is the route.
    [ApiController]
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;
        private IAuthorRepository _authorRepository;

        public CountriesController(ICountryRepository countryRepository , IAuthorRepository authorRepository)
        {
            _countryRepository = countryRepository;
            _authorRepository = authorRepository;
        }
        //api/countries
        [HttpGet]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _countryRepository.GetCountries().ToList();

            if (!ModelState.IsValid) // if the call was invalid, then return bad request. (if the list was empty that doesn't mean it's invalid result).
            {
                return BadRequest(ModelState);
            }
            var countriesDto = new List<CountryDto>();
            foreach (var country in countries)
            {
                countriesDto.Add(new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name
                });
            }
            return Ok(countriesDto);
        }

        //api/countries/countryId
        [HttpGet("{countryId}",Name = "GetCountry")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountry(int countryId)
        {

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();


            var country = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid) // if the call was invalid, then return bad request. (if the list was empty that doesn't mean it's invalid result).
            {
                return BadRequest(ModelState);
            }

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }


        //api/countries/authors/authorId
        [HttpGet("authors/{authorId}")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountryOfAnAuthor(int authorId)
        {

            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var country = _countryRepository.GetCountryOfAnAuthor(authorId);


            if (!ModelState.IsValid) // if the call was invalid, then return bad request. (if the list was empty that doesn't mean it's invalid result).
                return BadRequest(ModelState);

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);

        }

        // TO DO - GetAuthorsFromACountry()
        //api/countries/countryId/authors
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(400)]// for Bad Request.
        [ProducesResponseType(404)] 
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsFromCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var authors = _countryRepository.GetAuthorsFromACountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }
        //api/countries
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Country))]
        [ProducesResponseType(400)]// for Bad Request.
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCountry([FromBody]Country countryToCreate) //[FromBody] means all the info comes from the body of the post request.
        {
            if (countryToCreate == null)
                return BadRequest(ModelState);

            var country = _countryRepository.GetCountries().Where(c => c.Name.Trim().ToUpper() == countryToCreate.Name.Trim().ToUpper()).FirstOrDefault(); //check if the country already exists or not.

            if(country != null)
            {
                ModelState.AddModelError("", $"Country {countryToCreate.Name} already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!_countryRepository.CreateCountry(countryToCreate)) // here we try to add the new country to the DB. 
            {
                // if the create was not success.
                ModelState.AddModelError("", $"Something want wrong saving {countryToCreate.Name} ");
                return StatusCode(500, ModelState);
            }
            // if the create success.

            return CreatedAtRoute("GetCountry", new { countryId = countryToCreate.Id }, countryToCreate); // return the info of the new country. call GetCountry() in line 55.
        }


        //api/countries/countryId
        [HttpPut("{countryId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not fount
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCountry(int countryId, [FromBody]Country updateCountryInfo)
        {
            if (updateCountryInfo == null)
                return BadRequest(ModelState);

            if (countryId != updateCountryInfo.Id)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            //if(_countryRepository.IsDuplicateCountryName(countryId,updateCountryInfo.Name))
            //     ModelState.AddModelError("", $"Country {updateCountryInfo.Name} already exists!");
            //    return StatusCode(422, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.UddateCountry(updateCountryInfo))
            {
                ModelState.AddModelError("", $"Somthing went wrong updating {updateCountryInfo.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/countries/countryId
        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(409)] // conflict
        [ProducesResponseType(500)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var countryToDelete = _countryRepository.GetCountry(countryId);

            if (_countryRepository.GetAuthorsFromACountry(countryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Country {countryToDelete.Name} cannot be deleted BC it's used at least on author.");
                return StatusCode(409,ModelState); // 409 conflict.
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.DeleteCountry(countryToDelete)) 
            {
                ModelState.AddModelError("", $"Something went wrong deleteing {countryToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();


        }
    }

}
