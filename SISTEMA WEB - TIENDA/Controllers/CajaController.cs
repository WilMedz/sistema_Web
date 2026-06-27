using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Cajero,Administrador")]
    public class CajaController : Controller
    {
        public IActionResult Index() => View();
    }
}