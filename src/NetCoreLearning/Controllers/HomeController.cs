using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreLearning.Models;
using NetCoreLearning.ViewModels;
using NetCoreLearning.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreLearning.Controllers
{
    public class HomeController : Controller
    {
        private IGreeter _greeter;
        private IRestaurantData _restaurantData;

        public HomeController(IGreeter greeter, IRestaurantData restaurantData)
        {
            _greeter = greeter;
            _restaurantData = restaurantData;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            HomePageViewModel vm = new HomePageViewModel
            {
                CurrentMessage = _greeter.GetGreetString(),
                Restaurants = _restaurantData.GetAll()
            };
            return View(vm);
        }

        public IActionResult IndexWithPartialView()
        {
            HomePageViewModel vm = new HomePageViewModel
            {
                CurrentMessage = _greeter.GetGreetString(),
                Restaurants = _restaurantData.GetAll()
            };
            return View(vm);
        }

        public IActionResult Detail(int id)
        {
            Restaurant model = _restaurantData.Get(id);
            if (model == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RestaurantEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newRestaurant = new Restaurant
                {
                    Name = model.Name,
                    Cuisine = model.Cuisine
                };
                newRestaurant = _restaurantData.Add(newRestaurant);
                _restaurantData.Commit();

                return RedirectToAction(nameof(Detail), new { id = newRestaurant.Id });
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Restaurant model = _restaurantData.Get(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RestaurantEditViewModel model)
        {
            var restaurant = _restaurantData.Get(id);
            restaurant.Name = model.Name;
            restaurant.Cuisine = model.Cuisine;

            if (ModelState.IsValid)
            {                
                _restaurantData.Commit();
                
                return RedirectToAction(nameof(Detail), new { id = restaurant.Id });
            }
            else
            {
                return View(restaurant);
            }
        }
    }
}
