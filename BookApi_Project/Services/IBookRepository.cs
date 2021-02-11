﻿using BookApi_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi_Project.Services
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);
        Book GetBook(string bookIsbn);
        bool BookExists(int bookId);
        bool BookExists(string bookIsbn);
        bool IsDuplicateISBN(int bookId,string bookIsbn);
        decimal GetBookRating(int bookId);

        bool CreateBook(List<int> authorsId , List<int> categoriesId, Book book);
        bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book);
        bool DeleteBook (Book book);
        bool Save();
    }
}
