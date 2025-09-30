using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Data;
public class PharmacyController : Controller
{
    private readonly PharmacyDbContext _context;

    public PharmacyController(PharmacyDbContext context)
    {
        _context = context;
    }

    public IActionResult Search()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetNearest(double lat, double lon)
    {
        var pharmacies = await _context.Pharmacies.ToListAsync();

        var nearest = pharmacies.Select(p => new
        {
            id = p.Id,
            name = p.Name,
            address = p.Address,
            distance = GetDistance(lat, lon, p.Latitude, p.Longitude) // km
        })
        .OrderBy(p => p.distance)
        .Take(5);

        return Json(nearest);
    }

    private double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371; // km
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;
        lat1 = lat1 * Math.PI / 180;
        lat2 = lat2 * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}
