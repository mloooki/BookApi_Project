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
    [Route("api/[controller]")] // controller = categories. this is the route.
    [ApiController]
    public class CategoriesController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private IBookRepository _bookRepository;


        public CategoriesController(ICategoryRepository categoryRepository , IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }

        //api/categories
        [HttpGet]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var categoriesDto = new List<CategoryDto>();
            foreach(var category in categories)
            {
                categoriesDto.Add(
                    new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name
                    });
            }
            return Ok(categoriesDto);
        }


        //api/categories/categoryId
        [HttpGet("{categoryId}",Name = "GetCategory")]
        // ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // bad request.
        [ProducesResponseType(404)] // not found.
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        public IActionResult GetCategory(int categoryId)
        {

            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDto = new CategoryDto(){
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }


        //api/categories/books/bookId
        [HttpGet("books/{bookId}")]
         //ProducesResponseType (to type the expected result from the API, it's not nessesary to write.
        [ProducesResponseType(400)] // bad request.
        [ProducesResponseType(404)] //not found.
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetAllCategoriesForABook(int bookId)
        {

            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var categories = _categoryRepository.GetAllCategoriesForABook(bookId);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDtos = new List<CategoryDto>();
            foreach(var category in categories)
            {
                categoryDtos.Add( new CategoryDto {
                    Id = category.Id,
                    Name= category.Name
                });
            }

            return Ok(categoryDtos);
        }



        //api/categories/categoryId/books
        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // bad request.
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GettAllBooksForCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var books = _categoryRepository.GetAllBooksForCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();

            foreach (var book in books)
            {
                booksDto.Add(new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    DatePublished = book.DatePublished
                });
            }

            return Ok(booksDto);
        }


        //api/categories
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]// for Bad Request.
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody] Category categoryToCreate) //[FromBody] means all the info comes from the body of the post request.
        {
            if (categoryToCreate == null)
                return BadRequest(ModelState);

            var category = _categoryRepository.GetCategories().Where(c => c.Name.Trim().ToUpper() == categoryToCreate.Name.Trim().ToUpper()).FirstOrDefault(); //check if the category already exists or not.

            if (category != null)
            {
                ModelState.AddModelError("", $"Category {categoryToCreate.Name} already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!_categoryRepository.CreateCategory(categoryToCreate)) // here we try to add the new category to the DB. 
            {
                // if the create was not success.
                ModelState.AddModelError("", $"Something want wrong saving {categoryToCreate.Name} ");
                return StatusCode(500, ModelState);
            }
            // if the create success.

            return CreatedAtRoute("GetCategory", new { categoryId = categoryToCreate.Id }, categoryToCreate); // return the info of the new category. call GetCategory() in line 57.
        }



        ////api/categories/categoryId
        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not fount
        [ProducesResponseType(422)] // Unprocessable Entity
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] Category updateCategoryInfo)
        {
            if (updateCategoryInfo == null)
                return BadRequest(ModelState);

            if (categoryId != updateCategoryInfo.Id)
                return BadRequest(ModelState);

            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            //if(_countryRepository.IsDuplicateCountryName(countryId,updateCountryInfo.Name))
            //     ModelState.AddModelError("", $"Country {updateCountryInfo.Name} already exists!");
            //    return StatusCode(422, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.UpdateCategory(updateCategoryInfo))
            {
                ModelState.AddModelError("", $"Somthing went wrong updating {updateCategoryInfo.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/countries/countryId
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)] // for Bad Request.
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(409)] // conflict
        [ProducesResponseType(500)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if (_categoryRepository.GetAllBooksForCategory(categoryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Category {categoryToDelete.Name} cannot be deleted BC it's used by at least one book.");
                return StatusCode(409, ModelState); // 409 conflict.
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleteing {categoryToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();


        }
    }
}
