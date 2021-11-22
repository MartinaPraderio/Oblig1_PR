using BusinessLogic;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersGamesAPI.Models;

namespace UsersGamesAPI.Controllers
{
    [ApiController]
    [Route("[games]")]
    public class GameController : ControllerBase
    {
        private readonly GameLogic _gameLogic;

        public GameController(GameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        [HttpPost()]
        public IActionResult Create([FromBody] GameModel gameModel)
        {
            this._gameLogic.AddGame(GameModel.ToDomainObject(gameModel));
            return Content("Juego agregado exitosamente");
        }

        [HttpPut("{title}")]
        public IActionResult Edit([FromRoute] string title, [FromBody] GameModel newGameModel)
        {
            this._gameLogic.EditGame(title, GameModel.ToDomainObject(newGameModel));
            return Content("Juego modificado exitosamente");
        }

        [HttpDelete("{title}")]
        public IActionResult Delete([FromRoute] string title)
        {
            this._gameLogic.DeleteGame(title);
            return Content("Juego eliminado exitosamente");
        }
    }
}
