using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.Core.Model.ExpenseReportWorkflow
{
    public class DraftToSubmittedCommand : StateCommandBase
    {

        public DraftToSubmittedCommand(ExpenseReport expenseReport, Employee currentUser)
            : base(expenseReport, currentUser)
        {
        }

        public override string TransitionVerbPresentTense
        {
            get { return "Submit"; }
        }

        public override string TransitionVerbPastTense
        {
            get { return "Submitted"; }
        }

        public override ExpenseReportStatus GetBeginStatus()
        {
            return ExpenseReportStatus.Draft;
        }

        protected override ExpenseReportStatus GetEndStatus()
        {
            return ExpenseReportStatus.Submitted;
        }

        protected override bool userCanExecute(Employee currentUser)
        {
            return currentUser == _expenseReport.Submitter;
        }

        protected override void preExecute(IStateCommandVisitor commandVisitor)
        {

        }

        protected override void postExecute(IStateCommandVisitor commandVisitor)
        {
            commandVisitor.GoToEdit(_expenseReport);
        }
    }
}