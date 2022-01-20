using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;
using System;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> Get(
            [FromServices] DataContext context
        )
        {
            var employee = new User { Id = Guid.Parse("11465B3D-0885-42FF-8DED-EF53D88C7390"), UserName = "robin", Password = "robin", Role = "employee"};
            var manager = new User { Id = Guid.Parse("533B3354-B30E-451E-82FA-4DA74D92F1EB"), UserName = "batman", Password = "batman", Role = "manager"};
            var category = new Category { Id = Guid.Parse("757C62B3-8F11-48BA-8DEF-D2E955AD4055"), Title = "Informática" };
            var product = new Product { Id = Guid.Parse("E4EAC9B2-8665-49A9-A16C-6DA11E7F75BD"), Category = category, Title = "Mouse", Price = 99, Description = "Mouser Game"};
            context.Users.Add(employee);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new{
                message = "Data configured"
            });
        }
    }
}