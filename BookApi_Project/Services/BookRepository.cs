using BookApi_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Services
{
    public class BookRepository : IBookRepository
    {

        private BookDbContext _bookDBContext;
        

        public BookRepository(BookDbContext bookContext)
        {
            _bookDBContext = bookContext;
        }
        public bool BookExists(int bookId)
        {
            return _bookDBContext.Books.Any(b => b.Id== bookId);
        }

        public bool BookExists(string bookIsbn)
        {
            return _bookDBContext.Books.Any(b => b.Isbn == bookIsbn);

        }

        public bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book)
        {
            var authors = _bookDBContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = _bookDBContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            foreach(var author in authors)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = author,
                    Book = book
                };
                _bookDBContext.Add(bookAuthor);
            }


            foreach (var category in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = book
                };
                _bookDBContext.Add(bookCategory);
            }

            _bookDBContext.Add(book);

            return Save();
        }

        public bool DeleteBook(Book book)
        {
            _bookDBContext.Remove(book);
            return Save();
        }

        public Book GetBook(int bookId)
        {
            return _bookDBContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book GetBook(string bookIsbn)
        {
            return _bookDBContext.Books.Where(b => b.Isbn == bookIsbn).FirstOrDefault();

        }

        public decimal GetBookRating(int bookId)
        {
        var reviews = _bookDBContext.Reviews.Where(b => b.Book.Id == bookId);

            if (reviews.Count() <= 0)
                return 0;

            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());

        }

        public ICollection<Book> GetBooks()
        {
            return _bookDBContext.Books.OrderBy(b=> b.Title).ToList();
        }

        public bool IsDuplicateISBN(int bookId, string bookIsbn)
        {
           // return _bookDBContext.Books.Any(i => i.Isbn.Trim().ToUpper() == bookIsbn.Trim().ToUpper() && i.Id != bookId);
           var book = _bookDBContext.Books.Where(i => i.Isbn.Trim().ToUpper() == bookIsbn.Trim().ToUpper() && i.Id != bookId).FirstOrDefault();
            return book == null ? false : true;
        }

        public bool Save()
        {
            var saved = _bookDBContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book)
        {
            var authors = _bookDBContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = _bookDBContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            var bookAuthorsToDelete = _bookDBContext.BookAuthors.Where(b=> b.BookId == book.Id);
            var bookCategoriesToDelete = _bookDBContext.BookCategories.Where(b => b.BookId == book.Id);

            _bookDBContext.RemoveRange(bookAuthorsToDelete);
            _bookDBContext.RemoveRange(bookCategoriesToDelete);


            foreach (var author in authors)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = author,
                    Book = book
                };
                _bookDBContext.Add(bookAuthor);
            }


            foreach (var category in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = book
                };
                _bookDBContext.Add(bookCategory);
            }

            _bookDBContext.Update(book);

            return Save();
        }
    }
}
