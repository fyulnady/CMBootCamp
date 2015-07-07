using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.Core.Model.ExpenseReportWorkflow
{
    public class DraftingCommand : StateCommandBase
    {
        public DraftingCommand(ExpenseReport expenseReport, Employee currentUser)
            : base(expenseReport, currentUser)
        {
        }

        public override string TransitionVerbPresentTense
        {
            get { return "Save"; }
        }

        public override string TransitionVerbPastTense
        {
            get { return "Saved"; }
        }

        public override ExpenseReportStatus GetBeginStatus()
        {
            return ExpenseReportStatus.Draft;
        }

        protected override ExpenseReportStatus GetEndStatus()
        {
            return ExpenseReportStatus.Draft;
        }

        protected override bool userCanExecute(Employee currentUser)
        {
            return currentUser == _expenseReport.Submitter;
        }

        protected override void postExecute(IStateCommandVisitor commandVisitor)
        {
            commandVisitor.GoToEdit(_expenseReport);
        }
    }
}