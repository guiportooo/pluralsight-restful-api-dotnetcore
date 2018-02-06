namespace Library.API.Controllers
{
    using AutoMapper;
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
            var authorViewModels = Mapper.Map<IEnumerable<AuthorViewModel>>(authors);
            return Ok(authorViewModels);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var author = _libraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            var authorViewModel = Mapper.Map<AuthorViewModel>(author);
            return Ok(authorViewModel);
        }
    }
}