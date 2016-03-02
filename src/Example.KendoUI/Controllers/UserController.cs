using Example.KendoUI.Data;
using Example.KendoUI.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Example.KendoUI.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        #region Properties

        #endregion

        #region Constructors

        #endregion

        #region Methods
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "User Manager";
            return View();
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            if (id <= 0)
                return new BadRequestResult();

            var result = Data.DataSource.Users.FirstOrDefault(u => u.Id == id);
            return new ObjectResult(result);
        }

        [HttpGet("/users")]
        public IActionResult GetUsers()
        {
            var results = Data.DataSource.Users;
            return new ObjectResult(results);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
                return new BadRequestResult();

            Data.DataSource.Users.Create(user);

            return new ObjectResult(user);
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
                return new BadRequestResult();

            Data.DataSource.Users.Update(user);

            return new ObjectResult(user);
        }

        [HttpDelete]
        public IActionResult DeleteUser([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
                return new BadRequestResult();

            var result = Data.DataSource.Users.Delete(user);

            return new ObjectResult(result);
        }
        #endregion
    }
}