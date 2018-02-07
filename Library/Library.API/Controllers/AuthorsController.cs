namespace Library.API.Controllers
{
    using AutoMapper;
    using Entities;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using System;
    using System.Collections.Generic;

    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {
            var authors = _libraryRepository.GetAuthors();
            var authorDTOs = Mapper.Map<IEnumerable<AuthorDTO>>(authors);
            return Ok(authorDTOs);
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {
            var author = _libraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            var authorDTO = Mapper.Map<AuthorDTO>(author);
            return Ok(authorDTO);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] CreateAuthorDTO createAuthorDTO)
        {
            if (createAuthorDTO == null)
                return BadRequest();

            var newAuthor = Mapper.Map<Author>(createAuthorDTO);

            _libraryRepository.AddAuthor(newAuthor);

            if (!_libraryRepository.Save())
                throw new Exception("Creating an author failed on save.");

            var authorDTO = Mapper.Map<AuthorDTO>(newAuthor);
            return CreatedAtRoute("GetAuthor", new { id = authorDTO.Id }, authorDTO);
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id) 
            => _libraryRepository.AuthorExists(id) ? new StatusCodeResult(StatusCodes.Status409Conflict) : NotFound();

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var author = _libraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            _libraryRepository.DeleteAuthor(author);

            if(!_libraryRepository.Save())
                throw new Exception($"Deleting author {id} failed on save.");

            return NoContent();
        }
    }
}