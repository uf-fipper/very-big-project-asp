using System.Diagnostics;
using Asp.Models;
using Microsoft.AspNetCore.Mvc;
using Models.Context;

namespace Asp.Controllers;

public class HomeController(ILogger<HomeController> logger, DatabaseContext context) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
