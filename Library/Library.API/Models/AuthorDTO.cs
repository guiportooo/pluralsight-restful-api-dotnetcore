﻿namespace Library.API.Models
{
    using System;

    public class AuthorDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; }
    }
}
