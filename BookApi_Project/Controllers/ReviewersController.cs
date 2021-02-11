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
    [Route("api/[controller]")] // controller = Reviewer. this is the route.
    [ApiController]
    public class ReviewersController : Controller
    {

        private IReviewerRepository _reviewerRepository;
        private IReviewRepository _reviewRepository;


        public ReviewersController(IReviewerRepository reviewerRepository , IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }


        //api/reviewers
        [HttpGet]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // bad request.
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _reviewerRepository.GetReviewers();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var reviewersDto = new List<ReviewerDto>();
            foreach (var reviewer in reviewers)
            {
                reviewersDto.Add(
                    new ReviewerDto
                    {
                        Id = reviewer.Id,
                        FirstName = reviewer.FirstName,
                        LastName = reviewer.LastName
                    });
            }
            return Ok(reviewersDto);
        }


        //api/reviewers/reviewerId
        [HttpGet("{reviewerId}",Name = "GetReviewer")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // bad request.
        [ProducesResponseType(404)]// not found.
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer(int reviewerId)
        {

            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();


            var reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var reviewerDto = new ReviewerDto
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

        //api/reviewers/reviewerId/reviews
        [HttpGet("{reviewerId}/reviews")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // bad request
        [ProducesResponseType(404)]// not found.
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                reviewsDto.Add(
                    new ReviewDto
                    {
                        Id = review.Id,
                        Headline = review.Headline,
                        ReviewText = review.ReviewText,
                        Rating = review.Rating
                    }
                    );
            }

            return Ok(reviewsDto);
        }


        //api/reviewers/reviewerId/reviewer
        [HttpGet("{reviewId}/reviewer")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // bad request
        [ProducesResponseType(404)]// not found.
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerOfAReview(int reviewId)
        {
            if (_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewerOfAReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviwerDto = new ReviewerDto
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviwerDto);
        }


        //api/reviewers
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]// for Bad Request.
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody] Reviewer reviewerToCreate) //[FromBody] means all the info comes from the body of the post request.
        {
            if (reviewerToCreate == null)
                return BadRequest(ModelState);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!_reviewerRepository.CreateReviewer(reviewerToCreate)) // here we try to add the new category to the DB. 
            {
                // if the create was not success.
                ModelState.AddModelError("", $"Something want wrong saving {reviewerToCreate.FirstName} {reviewerToCreate.LastName} ");
                return StatusCode(500, ModelState);
            }
            // if the create success.

            return CreatedAtRoute("GetReviewer", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate); // return the info of the new category. call GetCategory() in line 57.
        }


        ////api/reviewers/reviewerId
        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(500)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] Reviewer reviewerToUpdate)
        {
            if (reviewerToUpdate == null)
                return BadRequest(ModelState);

            if (reviewerId != reviewerToUpdate.Id)
                return BadRequest(ModelState);

            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.UpdateReviewer(reviewerToUpdate))
            {
                ModelState.AddModelError("", $"Somthing went wrong updating {reviewerToUpdate.FirstName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }



        //api/reviewers/reviewerId
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(500)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);
            var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleteing {reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Something went wrong deleteing reviews by {reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
