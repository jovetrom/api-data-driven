using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Shop.Data;
using Shop.Models;
using Microsoft.AspNetCore.Authorization;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices]DataContext context
        )
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(
            Guid id,
            [FromServices]DataContext context
        )
        {
            var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(product);
        }

        [HttpGet]
        [Route("categories/{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(
            Guid id,
            [FromServices]DataContext context
        )
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles ="employee")]
        public async Task<ActionResult<Product>> Post(
            [FromBody]Product model,
            [FromServices]DataContext context
        )
        {       
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Has not proved possible to create product" });
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles ="manager")]
        public async Task<ActionResult<Product>> Put(
            Guid id, 
            [FromBody]Product model,
            [FromServices]DataContext context
        )
        {
            if (id != model.Id)
            return NotFound(new { message = "Product not found"});
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Product>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "This register already was update"});
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Has not proved possible to update Product"});
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles ="manager")]
        public async Task<ActionResult<Product>> Delete(
            Guid id,
            [FromServices]DataContext context
        )
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound(new { message = "Product not found"});

            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Has not proved possible to delete product"});
            }
        }
    }
}