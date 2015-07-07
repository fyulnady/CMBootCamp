using System.Linq;
using System.Web.Mvc;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.UI.Helpers;
using ClearMeasure.Bootcamp.UI.Helpers.ActionFilters;
using ClearMeasure.Bootcamp.UI.Models;
using UI.Models;

namespace ClearMeasure.Bootcamp.UI.Controllers
{
    [AddUserMetaDataToViewData]
    [Authorize]
    public class ExpenseReportController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IExpenseReportBuilder _expenseReportBuilder;
        private readonly IExpenseReportRepository _expenseReportRepository;
        private readonly IUserSession _session;
        private readonly IWorkflowFacilitator _workflowFacilitator;

        public ExpenseReportController(IEmployeeRepository employeeRepository,
            IExpenseReportRepository expenseReportRepository, IExpenseReportBuilder expenseReportBuilder, IUserSession session,
            IWorkflowFacilitator workflowFacilitator, IStateCommandVisitor stateCommandVisitor)
        {
            _employeeRepository = employeeRepository;
            _expenseReportRepository = expenseReportRepository;
            _expenseReportBuilder = expenseReportBuilder;
            _session = session;
            _workflowFacilitator = workflowFacilitator;
        }

        public ActionResult Manage(string id, EditMode mode)
        {
            Employee currentUser = _session.GetCurrentUser();

            ExpenseReport expenseReport;

            if (mode == EditMode.New)
            {
                expenseReport = _expenseReportBuilder.Build(currentUser);
                if (!string.IsNullOrEmpty(id))
                    expenseReport.Number = id;
            }
            else
            {
                expenseReport = _expenseReportRepository.GetSingle(id);
            }

            ExpenseReportManageModel model = CreateViewModel(mode, expenseReport);
            model.IsReadOnly = !_workflowFacilitator.GetValidStateCommands(expenseReport, currentUser).Any();
            ViewBag.ExpenseReport = expenseReport;
            ViewBag.CurrentUser = currentUser;

            return View("Manage", model);
        }

        [HttpPost]
        public ActionResult Manage(ExpenseReportManageModel model, string command)
        {
            Employee currentUser = _session.GetCurrentUser();

            ExpenseReport expenseReport;

            if (model.Mode == EditMode.New)
                expenseReport = _expenseReportBuilder.Build(currentUser);
            else
                expenseReport = _expenseReportRepository.GetSingle(model.ExpenseReportNumber);

            if (!ModelState.IsValid)
            {
                ViewBag.ExpenseReport = expenseReport;
                ViewBag.CurrentUser = currentUser;
                return View("Manage", model);
            }

            Employee practiceOwner = _employeeRepository.GetByUserName(model.ApproverUserName);
            Employee accountManager = _employeeRepository.GetByUserName(model.SubmitterUserName);

            expenseReport.Number = model.ExpenseReportNumber;
            expenseReport.Submitter = accountManager;
            expenseReport.Approver = practiceOwner;
            expenseReport.Title = model.Title;
            expenseReport.Description = model.Description;

            return new ExecuteCommandResult(expenseReport, command);
        }

        private ExpenseReportManageModel CreateViewModel(EditMode mode, ExpenseReport expenseReport)
        {
            var model = new ExpenseReportManageModel
            {
                ExpenseReport = expenseReport,
                Mode = mode,
                ExpenseReportNumber = expenseReport.Number,
                Status = expenseReport.FriendlyStatus,
                SubmitterUserName = expenseReport.Submitter.UserName,
                SubmitterFullName = expenseReport.Submitter.GetFullName(),
                ApproverUserName = expenseReport.Approver != null ? expenseReport.Approver.UserName : "",
                Title = expenseReport.Title,
                Description = expenseReport.Description,
                CanReassign = UserCanChangeAssignee(expenseReport),
                CreatedDate = expenseReport.CreatedDate.ToString()
            };
            return model;
        }

        public bool UserCanChangeAssignee(ExpenseReport expenseReport)
        {
            if (expenseReport.Status != ExpenseReportStatus.Draft)
            {
                return false;
            }

            return true;
        }
    }
}