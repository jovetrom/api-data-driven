using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Shop.Data;
using Shop.Models;
using Microsoft.AspNetCore.Authorization;

namespace Shop.Controllers
{

    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] //Caso a configuração de Cache esteja na Startup, está linha desabilita o cache para este metodo 
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context
        )
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            Guid id,
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category model,
            [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Has not proved possible to create category" });
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(
            Guid id,
            [FromBody] Category model,
            [FromServices] DataContext context
        )
        {
            if (id != model.Id)
                return NotFound(new { message = "Category not found" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "This register already was update" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Has not proved possible to update category" });
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Delete(
            Guid id,
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Has not proved possible to delete category" });
            }
        }
    }
}