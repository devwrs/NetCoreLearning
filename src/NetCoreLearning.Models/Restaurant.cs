using System.ComponentModel.DataAnnotations;

namespace NetCoreLearning.Models
{
    public enum CuisineType
    {
        None,
        Italian,
        French,
        Japanese,
        American
    }

    public class Restaurant
    {
        public int Id { get; set; }
        [Required, MaxLength(80), Display(Name ="Restaurant Name")]
        public string Name { get; set; }
        [Required, Display(Name ="Cuisine Type")]
        public CuisineType Cuisine { get; set; }
        public Restaurant()
        {
        }
    }
}
