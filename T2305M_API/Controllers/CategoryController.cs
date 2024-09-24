using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using T2305M_API.Entities;
using T2305M_API.DTO.ResponseModel;
using T2305M_API.DTO.RequestModel.Category;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace T2305M_API.Controllers
{
    [ApiController]
    [Route("/api/category")]
    [Authorize(Policy = "AUTH")]
    public class CategoryController : Controller
    {
        private readonly T2305mApiContext _context;
        public CategoryController(T2305mApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "ValidYearOld")]
        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            List<CategoryResponse> data = new List<CategoryResponse>();
            foreach(var item in categories)
            {
                data.Add(new CategoryResponse(item));
            }
            return Ok(data);
        }

        [HttpGet]
        [Route("find")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Get(int id)
        {
            try
            {
                Category category = _context.Categories.Find(id);
                if (category == null)
                    throw new Exception("Category not found");
                return Ok(new CategoryResponse(category));
            }catch(Exception e)
            {
                return NotFound(e.Message);
            }
           
        }


        [HttpPost]
        public IActionResult Create(CreateCategory model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception(ModelState.Values.SelectMany(v => v.Errors)
                        .Select(v => v.ErrorMessage).First());
                Category data = new Category { Name = model.name };
                _context.Categories.Add(data);
                _context.SaveChanges();
                return Created($"find?id={data.Id}", new CategoryResponse(data));
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(EditCategory model)
        {
            try
            {
                Category category = _context.Categories.Find(model.id);
                if (category == null)
                    throw new Exception("Category not found!");
                category.Name = model.name;
                _context.Categories.Update(category);
                return NoContent();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                Category category = _context.Categories.Find(id);
                if (category == null)
                    throw new Exception("Category not found!");
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return NoContent();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

