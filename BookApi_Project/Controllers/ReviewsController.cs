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
    [Route("api/[controller]")] // controller = Reviews. this is the route.
    [ApiController]
    public class ReviewsController : Controller
    {
        private IReviewRepository _reviewRepository;
        private IBookRepository _bookRepository;
        private IReviewerRepository _reviewerRepository;


        public ReviewsController(IReviewRepository reviewRepository, IBookRepository bookRepository, IReviewerRepository reviewerRepository)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _reviewerRepository = reviewerRepository;
        }

        //api/reviews
        [HttpGet]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            var reviews = _reviewRepository.GetReviews();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                reviewsDto.Add(
                    new ReviewDto
                    {
                        Id = review.Id,
                        Rating = review.Rating,
                        Headline = review.Headline,
                        ReviewText = review.ReviewText
                    });
            }
            return Ok(reviewsDto);
        }

        //api/Reviews/reviewId
        [HttpGet("{reviewId}", Name = "GetReview")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        public IActionResult GetReview(int reviewId)
        {

            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();


            var review = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewDto = new ReviewDto
            {
                Id = review.Id,
                Headline = review.Headline,
                ReviewText = review.ReviewText,
                Rating = review.Rating
            };
            return Ok(reviewDto);
        }



        //api/Reviews/reviewId/book
        [HttpGet("{reviewId}/book")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBookOfAReview(int reviewId)
        {

            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();


            var book = _reviewRepository.GetBookOfAReview(reviewId);

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




        //api/Reviews/books/bookId
        [HttpGet("books/{bookId}")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsOfABook(int bookId)
        {

            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var reviews = _reviewRepository.GetReviewsOfABook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                reviewsDto.Add(new ReviewDto
                {
                    Id = review.Id,
                    Headline = review.Headline,
                    ReviewText = review.ReviewText,
                    Rating = review.Rating
                });
            };
            return Ok(reviewsDto);
        }



        //api/reviews
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Review))]
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)]
        [ProducesResponseType(500)] //server error
        public IActionResult CreateReview([FromBody] Review reviewyToCreate) //[FromBody] means all the info comes from the body of the post request.
        {
            if (reviewyToCreate == null)
                return BadRequest(ModelState);


            if (!_reviewerRepository.ReviewerExists(reviewyToCreate.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer doesn't exist!");

            if (!_bookRepository.BookExists(reviewyToCreate.Book.Id))
                ModelState.AddModelError("", "Book doesn't exist!");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            reviewyToCreate.Book = _bookRepository.GetBook(reviewyToCreate.Book.Id); // get book info from the DB , (BC we need it to create review object).
            reviewyToCreate.Reviewer = _reviewerRepository.GetReviewer(reviewyToCreate.Reviewer.Id); // get reviewr from the DB, (BC we need it to create review object).

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!_reviewRepository.CreateReview(reviewyToCreate)) // here we try to add the new review to the DB. 
            {
                // if the create was not success.
                ModelState.AddModelError("", $"Something want wrong saving the review");
                return StatusCode(500, ModelState);
            }
            // if the create success.

            return CreatedAtRoute("GetReview", new { reviewId = reviewyToCreate.Id }, reviewyToCreate); // return the info of the new category. call GetCategory() in line 57.
        }



        ////api/reviews/reviewId
        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not fount
        [ProducesResponseType(422)] // Unprocessable Entity
        [ProducesResponseType(500)]
        public IActionResult UpdateReview(int reviewId, [FromBody] Review reviewToUpdate)
        {
            if (reviewToUpdate == null)
                return BadRequest(ModelState);

            if (reviewId != reviewToUpdate.Id)
                return BadRequest(ModelState);

            if (!_reviewRepository.ReviewExists(reviewId))
                ModelState.AddModelError("", "Review doesn't exist!");

            if (!_reviewerRepository.ReviewerExists(reviewToUpdate.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer doesn't exist!");

            if (!_bookRepository.BookExists(reviewToUpdate.Book.Id))
                ModelState.AddModelError("", "Book doesn't exist!");


            if (!ModelState.IsValid)
                return StatusCode(404,ModelState);


            reviewToUpdate.Book = _bookRepository.GetBook(reviewToUpdate.Book.Id); // get book info from the DB , (BC we need it to create review object).
            reviewToUpdate.Reviewer = _reviewerRepository.GetReviewer(reviewToUpdate.Reviewer.Id); // get reviewr from the DB, (BC we need it to create review object).

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.UpdateReview(reviewToUpdate))
            {
                ModelState.AddModelError("", $"Somthing went wrong updating the review");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/reviews/reviewId
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var reviewToDelete = _reviewRepository.GetReview(reviewId);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleteing {reviewToDelete.Headline}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
