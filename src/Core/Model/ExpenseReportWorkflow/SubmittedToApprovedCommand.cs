using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.Core.Model.ExpenseReportWorkflow
{
    public class SubmittedToApprovedCommand : StateCommandBase
    {
        public SubmittedToApprovedCommand(ExpenseReport expenseReport, Employee currentUser) : base(expenseReport, currentUser)
        {
        }

        public override string TransitionVerbPresentTense
        {
            get { return "Approve"; }
        }

        public override string TransitionVerbPastTense
        {
            get { return "Approved"; }
        }

        public override ExpenseReportStatus GetBeginStatus()
        {
            return ExpenseReportStatus.Submitted;
        }

        protected override ExpenseReportStatus GetEndStatus()
        {
            return ExpenseReportStatus.Approved;
        }

        protected override bool userCanExecute(Employee currentUser)
        {
            return currentUser == _expenseReport.Approver;
        }

        protected override void postExecute(IStateCommandVisitor commandVisitor)
        {
            commandVisitor.GoToEdit(_expenseReport);
        }
    }
}