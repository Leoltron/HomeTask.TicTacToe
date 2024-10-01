using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HomeTask.TicTacToe.Models;
using HomeTask.TicTacToe.Models.Games;

namespace HomeTask.TicTacToe.Controllers;

public class IndexController(IGameListManager gameListManager) : Controller
{
    public IActionResult Index()
    {
        var game = gameListManager.FindOrCreateActiveGame();
        return View(game);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}