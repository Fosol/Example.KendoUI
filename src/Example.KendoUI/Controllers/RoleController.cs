using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.KendoUI.Controllers
{
    [Route("[controller]")]
    public class RoleController : Controller
    {
        #region Properties

        #endregion

        #region Constructors

        #endregion

        #region Methods
        public IActionResult Find()
        {
            return View();
        }
        #endregion
    }
}
