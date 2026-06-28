using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; 
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Login");

        // Si es cajero, redirige al módulo de caja
        if (User.IsInRole("Cajero"))
            return RedirectToAction("Index", "Caja");

        var catalogoPrendas = await _context.Prendas
            .Include(p => p.Marca)
            .Include(p => p.Categoria)
            .Include(p => p.Variantes)
            .ToListAsync();

        return View(catalogoPrendas);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}