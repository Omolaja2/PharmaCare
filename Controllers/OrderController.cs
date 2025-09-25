using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class OrderController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }

}
