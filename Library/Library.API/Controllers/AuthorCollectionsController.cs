namespace Library.API.Controllers
{
    using AutoMapper;
    using Entities;
    using Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Route("api/authorcollections")]
    public class AuthorCollectionsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection([FromBody] IEnumerable<CreateAuthorDTO> createAuthorDTOs)
        {
            if (createAuthorDTOs == null)
                return BadRequest();

            var newAuthors = Mapper.Map<IEnumerable<Author>>(createAuthorDTOs);

            foreach (var newAuthor in newAuthors)
            {
                _libraryRepository.AddAuthor(newAuthor);
            }

            if(!_libraryRepository.Save())
                throw new Exception("Creating an author collection failed on save.");

            var authorDTOs = Mapper.Map<IEnumerable<AuthorDTO>>(newAuthors);

            var idsAsString = string.Join(",", authorDTOs.Select(x => x.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString }, authorDTOs);
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            var authorIds = ids as IList<Guid> ?? ids.ToList();

            var authors = _libraryRepository.GetAuthors(authorIds);

            if (authors.Count() != authorIds.Count)
                return NotFound();

            var authorDTOs = Mapper.Map<IEnumerable<AuthorDTO>>(authors);

            return Ok(authorDTOs);
        }
    }
}