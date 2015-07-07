using System.Web.Mvc;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using UI.Models;

namespace ClearMeasure.Bootcamp.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeRepository _repository;
        private readonly IUserSession _session;

        public AccountController(IEmployeeRepository repository, IUserSession session)
        {
            _repository = repository;
            _session = session;
        }

        public ActionResult Login()
        {
            _session.LogOut();
            return View();
        }

        [HttpPost]
        public void Login(LoginModel model)
        {
            Employee employee = _repository.GetByUserName(model.UserName);
            _session.LogIn(employee);
        }

        public ActionResult Logout()
        {
            _session.LogOut();
            return Redirect("~/");
        }
    }
}
