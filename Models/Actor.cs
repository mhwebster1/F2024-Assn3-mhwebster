using System.ComponentModel.DataAnnotations;

namespace F24_Assignment3_mwebster.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }
        [Display(Name="IMDB Link")]
        public string Imdb { get; set; }
        //image code here
        [DataType(DataType.Upload)]
        [Display(Name = "Picture")]
        public byte[]? ActorImage {get; set;}
           

    }
}
