using BusinessLogic;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersGamesAPI.Models;

namespace UsersGamesAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly UserLogic _userLogic;

        public UserController(UserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        [HttpPost()]
        public IActionResult Create([FromBody] UserModel userModel)
        {
            this._userLogic.AddUser(UserModel.ToDomainObject(userModel));
            return Content("Usuario agregado exitosamente");
        }

        [HttpPut("{name}")]
        public IActionResult Edit([FromRoute] string name, [FromBody] UserModel userModel)
        {
            this._userLogic.EditUser(name, UserModel.ToDomainObject(userModel));
            return Content("Usuario modificado exitosamente");
        }

        [HttpDelete("{name}")]
        public IActionResult Delete([FromRoute] string name)
        {
            this._userLogic.DeleteUser(name);
            return Content("Usuario eliminado exitosamente");
        }

        [HttpPost("{name}")]
        public IActionResult AssociateGame([FromRoute] string name, [FromBody] string gameTitle)
        {
            this._userLogic.AssociateGame(name, gameTitle);
            return Content("Juego asociado exitosamente");
        }

        [HttpDelete("{name}")]
        public IActionResult DesassociateGame([FromRoute] string name, [FromBody] string gameTitle)
        {
            this._userLogic.DesassociateGame(name, gameTitle);
            return Content("Juego desasociado exitosamente");
        }
    }
}
