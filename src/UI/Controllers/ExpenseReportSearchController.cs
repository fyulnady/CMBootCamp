using System.Web.Mvc;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.UI.Helpers.ActionFilters;
using ClearMeasure.Bootcamp.UI.Models;

namespace ClearMeasure.Bootcamp.UI.Controllers
{
    [AddUserMetaDataToViewData]
    [Authorize]
    public class ExpenseReportSearchController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IExpenseReportRepository _expenseReportRepository;

        public ExpenseReportSearchController(IEmployeeRepository employeeRepository, IExpenseReportRepository expenseReportRepository)
        {
            _employeeRepository = employeeRepository;
            _expenseReportRepository = expenseReportRepository;
        }

        public ActionResult Index(ExpenseReportSearchModel.SearchFilters filters)
        {
            var model = new ExpenseReportSearchModel();
            if (filters != null)
                model.Filters = filters;

            var accountManager = _employeeRepository.GetByUserName(model.Filters.Submitter);
            var practiceOwner = _employeeRepository.GetByUserName(model.Filters.Approver);
            var status = !string.IsNullOrWhiteSpace(model.Filters.Status) ? ExpenseReportStatus.FromKey(model.Filters.Status) : null;

            var specification = new SearchSpecification();
            specification.MatchSubmitter(accountManager);
            specification.MatchApprover(practiceOwner);
            specification.MatchStatus(status);
            ExpenseReport[] orders = _expenseReportRepository.GetMany(specification);

            model.Results = orders;

            return View(model);
        }

    }
}
