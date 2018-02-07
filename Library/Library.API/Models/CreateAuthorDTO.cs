namespace Library.API.Models
{
    using System;
    using System.Collections.Generic;

    public class CreateAuthorDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Genre { get; set; }
        public ICollection<CreateBookDTO> Books { get; set; }

        public CreateAuthorDTO()
        {
            Books = new List<CreateBookDTO>();
        }
    }
}
