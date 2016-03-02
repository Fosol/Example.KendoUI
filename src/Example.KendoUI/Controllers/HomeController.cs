using Microsoft.AspNet.Mvc;

namespace Example.KendoUI.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        #region Properties

        #endregion

        #region Constructors

        #endregion

        #region Methods
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        #endregion
    }
}