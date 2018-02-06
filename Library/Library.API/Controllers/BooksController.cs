namespace Library.API.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using System;
    using System.Collections.Generic;

    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetBooks(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var books = _libraryRepository.GetBooksForAuthor(authorId);

            var bookViewModels = Mapper.Map<IEnumerable<BookViewModel>>(books);

            return Ok(bookViewModels);
        }

        [HttpGet("{id}")]
        public IActionResult GetBook(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var book = _libraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
                return NotFound();

            var bookViewModel = Mapper.Map<BookViewModel>(book);

            return Ok(bookViewModel);
        }
    }
}