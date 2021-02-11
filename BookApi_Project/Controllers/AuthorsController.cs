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
    [Route("api/[controller]")] // controller = Authors. this is the route.
    [ApiController]
    public class AuthorsController : Controller
    {
        private IAuthorRepository _authorRepository;
        private IBookRepository _bookRepository;
        private ICountryRepository _countryRepository;




        public AuthorsController(IAuthorRepository authorRepository , IBookRepository bookRepository, ICountryRepository countryRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _countryRepository = countryRepository;
        }

        //api/authors
        [HttpGet]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthors()
        {

            var authors = _authorRepository.GetAuthors();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();

            foreach(var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id= author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }


        //api/authors/authorId
        [HttpGet("{authorId}",Name = "GetAuthor")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        public IActionResult GetAuthor(int authorId)
        {

            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();


            var author = _authorRepository.GetAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDto = new AuthorDto
            {
                Id=author.Id,
                FirstName=author.FirstName,
                LastName=author.LastName
            };
            return Ok(authorDto);
        }

        //api/authors/authorId/books
        [HttpGet("{authorId}/books")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBookByAuthor(int authorId)
        {

            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var books = _authorRepository.GetBooksByAuthor(authorId);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();

            foreach(var book in books)
            {
                booksDto.Add(
                new BookDto
                {
                    Id=book.Id,
                    Title=book.Title,
                    Isbn=book.Isbn,
                    DatePublished = book.DatePublished
                
                });
            }

            return Ok(booksDto);

        }


        //api/authors/books/bookId
        [HttpGet("books/{bookId}")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsOfABook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var authors = _authorRepository.GetAuthorsOfABook(bookId);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorsDto.Add(
                new AuthorDto
                {
                    Id=author.Id,
                    FirstName=author.FirstName,
                    LastName= author.LastName
                });
            }

            return Ok(authorsDto);

        }




        //api/authors
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Author))]
        [ProducesResponseType(400)]// for Bad Request.
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateAuthor([FromBody] Author authorToCreate) //[FromBody] means all the info comes from the body of the post request.
        {
            if (authorToCreate == null)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(authorToCreate.Country.Id))
            {
                ModelState.AddModelError("", "Country is not exist!");
                return StatusCode(404, ModelState);
            }


            authorToCreate.Country = _countryRepository.GetCountry(authorToCreate.Country.Id);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!_authorRepository.CreateAuthor(authorToCreate)) // here we try to add the new category to the DB. 
            {
                // if the create was not success.
                ModelState.AddModelError("", $"Something want wrong saving {authorToCreate.FirstName} {authorToCreate.LastName} ");
                return StatusCode(500, ModelState);
            }
            // if the create success.

            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate); // return the info of the new category. call GetCategory() in line 57.
        }



        ////api/authors/authorId
        [HttpPut("{authorId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(500)]
        public IActionResult UpdateAuthor(int authorId, [FromBody] Author authorToUpdate)
        {
            if (authorToUpdate == null)
                return BadRequest(ModelState);

            if (authorId != authorToUpdate.Id)
                return BadRequest(ModelState);

            if (!_authorRepository.AuthorExists(authorId))
                ModelState.AddModelError("", "Author is not exist!");

            if (!_countryRepository.CountryExists(authorToUpdate.Country.Id))
                ModelState.AddModelError("", "Country is not exist!");


            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            authorToUpdate.Country = _countryRepository.GetCountry(authorToUpdate.Country.Id);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_authorRepository.UpdateAuthor(authorToUpdate))
            {
                ModelState.AddModelError("", $"Somthing went wrong updating the author {authorToUpdate.FirstName} {authorToUpdate.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/authors/authorId
        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)] // no content.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(409)] // conflict.
        [ProducesResponseType(500)]
        public IActionResult DeleteAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var authorToDelete = _authorRepository.GetAuthor(authorId);

            if (_authorRepository.GetBooksByAuthor(authorId).Count() > 0)
            {
                ModelState.AddModelError("", $"{authorToDelete.FirstName} {authorToDelete.LastName} has at least one book, you can't delete");
                return StatusCode(409, ModelState); // 409 conflict.
            }


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleteing {authorToDelete.FirstName} {authorToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
