﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductRepository _repository;

        public NavController(IProductRepository repository)
        {
            _repository = repository;
        }

        public PartialViewResult Menu(string category = null, string sorting = null)
        {
            ViewBag.SelectedCategory = category;
            ViewBag.Sorting = sorting;

            IEnumerable<string> categories = _repository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);

            return PartialView("FlexMenu", categories);
        }
    }
}