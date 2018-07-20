using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace mmria.server.Controllers
{
    [Authorize(Policy = "abstractor,committee_member")]
    //[Authorize(Policy = "Over21Only")]
    //[Authorize(Policy = "BuildingEntry")]
    
    public class CaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}