namespace Library.API.Controllers
{
    using AutoMapper;
    using Entities;
    using Microsoft.AspNetCore.JsonPatch;
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

            var bookDTOs = Mapper.Map<IEnumerable<BookDTO>>(books);

            return Ok(bookDTOs);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var book = _libraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
                return NotFound();

            var bookDTO = Mapper.Map<BookDTO>(book);

            return Ok(bookDTO);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] CreateBookDTO createBookDTO)
        {
            if (createBookDTO == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var newBook = Mapper.Map<Book>(createBookDTO);

            _libraryRepository.AddBookForAuthor(authorId, newBook);

            if (!_libraryRepository.Save())
                throw new Exception($"Creating a book for author {authorId} failed on save.");

            var bookDTO = Mapper.Map<BookDTO>(newBook);
            return CreatedAtRoute("GetBookForAuthor", new {authorId, id = bookDTO.Id}, bookDTO);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var book = _libraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
                return NotFound();

            _libraryRepository.DeleteBook(book);

            if (!_libraryRepository.Save())
                throw new Exception($"Deleting book {id} for author {authorId} failed on save.");

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] UpdateBookDTO updateBookDTO)
        {
            if (updateBookDTO == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var book = _libraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
            {
                var newBook = Mapper.Map<Book>(updateBookDTO);
                newBook.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, newBook);

                if (!_libraryRepository.Save())
                    throw new Exception($"Upserting book {id} for author {authorId} failed on save.");

                var bookDTO = Mapper.Map<BookDTO>(newBook);

                return CreatedAtRoute("GetBookForAuthor", new {authorId, id = bookDTO.Id}, bookDTO);
            }

            Mapper.Map(updateBookDTO, book);

            if (!_libraryRepository.Save())
                throw new Exception($"Updating book {id} for author {authorId} failed on save.");

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id,
            [FromBody] JsonPatchDocument<UpdateBookDTO> patchUpdateBookDTO)
        {
            if (patchUpdateBookDTO == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var book = _libraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
            {
                var newUpdateBookDTO = new UpdateBookDTO();
                patchUpdateBookDTO.ApplyTo(newUpdateBookDTO);

                var newBook = Mapper.Map<Book>(newUpdateBookDTO);
                newBook.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, newBook);

                if (!_libraryRepository.Save())
                    throw new Exception($"Upserting book {id} for author {authorId} failed on save.");

                var bookDTO = Mapper.Map<BookDTO>(newBook);

                return CreatedAtRoute("GetBookForAuthor", new { authorId, id = bookDTO.Id }, bookDTO);
            }

            var updateBookDTO = Mapper.Map<UpdateBookDTO>(book);

            patchUpdateBookDTO.ApplyTo(updateBookDTO);

            Mapper.Map(updateBookDTO, book);

            if (!_libraryRepository.Save())
                throw new Exception($"Patching book {id} for author {authorId} failed on save.");

            return NoContent();
        }
    }
}