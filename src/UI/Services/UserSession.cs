using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;

namespace ClearMeasure.Bootcamp.UI.Services
{
    public class UserSession : IUserSession
    {
        private readonly IEmployeeRepository _employeeRepository;

        public UserSession(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public UserSession() : this(DependencyResolver.Current.GetService<IEmployeeRepository>())
        {
        }

        #region IUserSession Members

        public Employee GetCurrentUser()
        {
            IOwinContext context = HttpContext.Current.GetOwinContext();
            ClaimsPrincipal user = context.Authentication.User;
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }

            string userName = user.Claims.Single(claim => claim.Type == ClaimTypes.Name).Value;
            Employee currentUser = _employeeRepository.GetByUserName(userName);
            blowUpIfEmployeeCannotLogin(currentUser);
            return currentUser;
        }

        public void LogIn(Employee employee)
        {
            blowUpIfEmployeeCannotLogin(employee);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.UserName),
                new Claim(ClaimTypes.Email, employee.EmailAddress)
            };
            var id = new ClaimsIdentity(claims,
                                        DefaultAuthenticationTypes.ApplicationCookie);

            var context = HttpContext.Current.GetOwinContext();
            var authenticationManager = context.Authentication;
            authenticationManager.SignIn(id);
        }

        public void LogOut()
        {
            var context = HttpContext.Current.GetOwinContext();
            var authenticationManager = context.Authentication;
            authenticationManager.SignOut();
        }

        public void PushUserMessage(FlashMessage message)
        {
            ensureFlashMessagesInitialized();
            Stack<FlashMessage> flash = getFlash();
            flash.Push(message);
        }

        public FlashMessage PopUserMessage()
        {
            Stack<FlashMessage> flash = getFlash();
            if (flash.Count == 0)
            {
                return null;
            }

            return flash.Pop();
        }

        #endregion

        private void blowUpIfEmployeeCannotLogin(Employee employee)
        {
            if (employee == null)
            {
                throw new InvalidCredentialException("That user doesn't exist or is not valid.");
            }
        }

        private Stack<FlashMessage> getFlash()
        {
            ensureFlashMessagesInitialized();
            HttpSessionState session = HttpContext.Current.Session;
            return (Stack<FlashMessage>) session["flash"];
        }

        private void ensureFlashMessagesInitialized()
        {
            HttpSessionState session = HttpContext.Current.Session;
            var flash = session["flash"] as Stack<FlashMessage>;
            if (flash == null)
            {
                session["flash"] = new Stack<FlashMessage>();
            }
        }
    }
}