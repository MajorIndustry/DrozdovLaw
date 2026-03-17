using Microsoft.AspNetCore.Mvc;

namespace DrozdovLaw.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("List", "Case");
}