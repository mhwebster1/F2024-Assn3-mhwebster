using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace F24_Assignment3_mwebster.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Display(Name = "IMDB Link")]
        public string? Imdb { get; set; }
        public string? Genre { get; set; }

        //image code here
        [DataType(DataType.Upload)]
        [Display(Name = "Poster")]
        public byte[]? MovieImage { get; set; }

        private int _releaseDate;

        [Display(Name = "Year Released")]
        public int ReleaseDate
        {
            get { return _releaseDate; }
            set
            {
                if (value >= 1000 && value <= 9999) // Restrict to a valid four-digit year
                {
                    _releaseDate = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("ReleaseDate must be a valid four-digit year.");
                }
            }
        }

        //[Display(Name ="Year Released")]
        //public int ReleaseDate { get; set; }

        //details link here
        

    }
}
