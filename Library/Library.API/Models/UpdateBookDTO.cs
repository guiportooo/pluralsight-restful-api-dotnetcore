namespace Library.API.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateBookDTO : ManipulateBookDTO
    {
        [Required(ErrorMessage = "You should fill out a description.")]

        public override string Description { get; set; }
    }
}
