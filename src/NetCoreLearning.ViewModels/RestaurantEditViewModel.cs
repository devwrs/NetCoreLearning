using System.ComponentModel.DataAnnotations;
using NetCoreLearning.Models;

namespace NetCoreLearning.ViewModels
{
    public class RestaurantEditViewModel
    {
        [Required, MaxLength(80), Display(Name = "Restaurant Name")]
        public string Name { get; set; }
        [Required, Display(Name = "Cuisine Type")]
        public CuisineType Cuisine { get; set; }
    }
}
