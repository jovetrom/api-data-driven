using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices]DataContext context
        )
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles ="manager")]
        public async Task<ActionResult<User>> Post(
            [FromBody]User model,
            [FromServices]DataContext context
        )
        {       
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            try
            {
                //Força o usuário a ser criado sempre com "funcionário"
                model.Role = "employee";
                context.Users.Add(model);
                await context.SaveChangesAsync();

                //Esconde a Senha
                model.Password = "";

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Has not proved possible to create user" });
            }
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Autheticate(
            [FromBody]User model,
            [FromServices]DataContext context)
        {
            var user = await context.Users.AsNoTracking().Where(x => x.UserName == model.UserName && x.Password == model.Password).FirstOrDefaultAsync();

            if(user == null)
                return NotFound(new { message = "User or password invalid"});
            
            var token = TokenService.GenerateToken(user);

            //Esconde a senha
            user.Password = "";

            return new
            {
                user = user,
                token = token
            };
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles ="manager")]
        public async Task<ActionResult<User>> Put(
            Guid id, 
            [FromBody]User model,
            [FromServices]DataContext context
        )
        {
            if (id != model.Id)
            return NotFound(new { message = "User not found"});
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "This register already was update"});
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Has not proved possible to update user"});
            }
        }
    }
}