using System.Collections.Generic;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.ExpenseReportWorkflow;

namespace ClearMeasure.Bootcamp.Core.Services.Impl
{
    public class WorkflowFacilitator : IWorkflowFacilitator
    {
        private readonly ICalendar _calendar;

        public WorkflowFacilitator(ICalendar calendar)
        {
            _calendar = calendar;
        }

        public IStateCommand[] GetValidStateCommands(ExpenseReport expenseReport, Employee currentUser)
        {
            var commands = new List<IStateCommand>(
                GetAllStateCommands(expenseReport, currentUser));
            commands.RemoveAll(delegate(IStateCommand obj) { return !obj.IsValid(); });

            return commands.ToArray();
        }

        public virtual IStateCommand[] GetAllStateCommands(ExpenseReport expenseReport, Employee currentUser)
        {
            var commands = new List<IStateCommand>();
            commands.Add(new DraftingCommand(expenseReport, currentUser));
            commands.Add(new DraftToSubmittedCommand(expenseReport, currentUser));
            commands.Add(new SubmittedToApprovedCommand(expenseReport, currentUser));


            return commands.ToArray();
        }
    }
}