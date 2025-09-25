using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyApp.Controllers
{
    [Authorize(Roles = "Pharmacist")] 
    public class PharmacyController : Controller
    {
        // Pharmacist dashboard
        public IActionResult MyPharmacy()
        {
            return View();
        }

        public IActionResult Manage()
        {
            return View();
        }
    }
}