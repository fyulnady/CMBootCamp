using System.Web.Mvc;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.UI.Models;
using UI.Models;

namespace ClearMeasure.Bootcamp.UI.Controllers
{
    public class ToDoController : Controller
    {
        private readonly IExpenseReportRepository _repository;
        private readonly IUserSession _session;

        public ToDoController(IExpenseReportRepository repository, IUserSession session)
        {
            _repository = repository;
            _session = session;
        }

        [ChildActionOnly]
        public ActionResult Index()
        {
            var model = new ToDoModel();

            Employee currentUser = _session.GetCurrentUser();
            var assignedSpecification = new SearchSpecification();
            assignedSpecification.MatchApprover(currentUser);
            assignedSpecification.MatchStatus(ExpenseReportStatus.Submitted);
            ExpenseReport[] assigned = _repository.GetMany(assignedSpecification);
            model.Submitted = assigned;

            var inProgressSpecification = new SearchSpecification();
            inProgressSpecification.MatchApprover(currentUser);
            inProgressSpecification.MatchStatus(ExpenseReportStatus.Approved);
            ExpenseReport[] inProgress = _repository.GetMany(inProgressSpecification);
            model.Approved = inProgress;

            return PartialView(model);
        }

    }
}
