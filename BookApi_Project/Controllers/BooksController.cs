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
    [Route("api/[controller]")] // controller = Books. this is the route.
    [ApiController]
    public class BooksController : Controller
    {

        private IBookRepository _bookRepository;
        private IAuthorRepository _authorRepository;
        private ICategoryRepository _categoryRepository;
        private IReviewRepository _reviewRepository;

        public BooksController(IBookRepository bookRepository, IAuthorRepository authorRepository, ICategoryRepository categoryRepository, IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
        }


        //api/books
        [HttpGet]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooks()
        {

            var books = _bookRepository.GetBooks();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var BooksDto = new List<BookDto>();

            foreach (var book in books)
            {
                BooksDto.Add(new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    DatePublished = book.DatePublished
                });
            }

            return Ok(BooksDto);
        }


        //api/books/bookId
        [HttpGet("{bookId}",Name = "GetBook")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(int bookId)
        {

            if (!_bookRepository.BookExists(bookId))
                return NotFound();


            var book = _bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                DatePublished = book.DatePublished
            };
            return Ok(bookDto);
        }


        //api/books/isbn/bookIsbn
        [HttpGet("ISBN/{bookIsbn}")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(string bookIsbn)
        {

            if (!_bookRepository.BookExists(bookIsbn))
                return NotFound();


            var book = _bookRepository.GetBook(bookIsbn);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                DatePublished = book.DatePublished
            };
            return Ok(bookDto);
        }

        //api/books/bookId/rating
        [HttpGet("{bookId}/rating")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var rating = _bookRepository.GetBookRating(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rating);

        }

        //api/books?authId=1&authId=2&catId=1&catId=2
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Book))]
        [ProducesResponseType(400)]// for Bad Request.
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateBook([FromQuery] List<int> authId, [FromQuery] List<int> catId, [FromBody] Book bookToCreate)
        {
            var statscode = ValidateBook(authId, catId, bookToCreate);

            if (!ModelState.IsValid)
                return StatusCode(statscode.StatusCode);

            if (!_bookRepository.CreateBook(authId, catId, bookToCreate))
            {
                ModelState.AddModelError("", $"Somthing went wrong saving book {bookToCreate.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetBook",new { bookId =bookToCreate.Id}, bookToCreate);
        }


        //api/books/bookId?authId=1&authId=2&catId=1&catId=2
        [HttpPut("{bookId}")]
        [ProducesResponseType(204)] // no content.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateBook(int bookId,[FromQuery] List<int> authId, [FromQuery] List<int> catId, [FromBody] Book bookToUpdate)
        {

            if (bookId != bookToUpdate.Id)
                return BadRequest();

            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var statscode = ValidateBook(authId, catId, bookToUpdate);

            if (!ModelState.IsValid)
                return StatusCode(statscode.StatusCode);

            if (!_bookRepository.UpdateBook(authId, catId, bookToUpdate))
            {
                ModelState.AddModelError("", $"Somthing went wrong updating book {bookToUpdate.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/books/bookId
        [HttpDelete("{bookId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(500)]
        public IActionResult DeleteBooke(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var reviewsToDelete = _reviewRepository.GetReviewsOfABook(bookId);
            var bookToldelete = _bookRepository.GetBook(bookId);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Something went wrong deleteing reviews");
                return StatusCode(500, ModelState);
            }

            if (!_bookRepository.DeleteBook(bookToldelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleteing book {bookToldelete.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private StatusCodeResult ValidateBook(List<int> authId , List<int> catId , Book book)
        {
            if(book == null || authId.Count() <= 0 || catId.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing book, author, or category");
                return BadRequest();
            }

            if(_bookRepository.IsDuplicateISBN(book.Id, book.Isbn))
            {
                ModelState.AddModelError("", "Duplicate ISBN");
                return StatusCode(422); //enprocceple entity.
            }


            foreach (var id in authId)
            {
                if (!_authorRepository.AuthorExists(id))
                {
                    ModelState.AddModelError("", "Author is not found");
                    return StatusCode(404);
                }
            }

            foreach (var id in catId)
            {
                if (!_categoryRepository.CategoryExists(id))
                {
                    ModelState.AddModelError("", "Category is not found");
                    return StatusCode(404);
                }
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical Error");
                return BadRequest();
            }
            return NoContent(); // valid status.
        }

        }
}
