using System;
using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.Core.Model.ExpenseReportWorkflow
{
    public abstract class StateCommandBase : IStateCommand
    {
        protected Employee _currentUser;
        protected ExpenseReport _expenseReport;

        protected StateCommandBase(ExpenseReport expenseReport, Employee currentUser)
        {
            _expenseReport = expenseReport;
            _currentUser = currentUser;
        }

        public abstract string TransitionVerbPastTense { get; }

        public abstract ExpenseReportStatus GetBeginStatus();
        public abstract string TransitionVerbPresentTense { get; }

        public bool IsValid()
        {
            bool beginStatusMatches = _expenseReport.Status == GetBeginStatus();
            bool currentUserIsCorrectRole = userCanExecute(_currentUser);
            return beginStatusMatches && currentUserIsCorrectRole;
        }

        public void Execute(IStateCommandVisitor commandVisitor)
        {
//            Log.Info(this, "Executing");
            preExecute(commandVisitor);
            string currentUserFullName = _currentUser.GetFullName();
            _expenseReport.ChangeStatus(_currentUser, DateTime.Now, GetEndStatus());

            commandVisitor.Save(_expenseReport);

            string loweredTransitionVerb = TransitionVerbPastTense.ToLower();
            string reportNumber = _expenseReport.Number;
            string message = string.Format("You have {0} work order {1}", loweredTransitionVerb, reportNumber);
            commandVisitor.SendMessage(message);
            string debugMessage = string.Format("{0} has {1} work order {2}", currentUserFullName, loweredTransitionVerb,
                reportNumber);
//            Log.Debug(this, debugMessage);
            postExecute(commandVisitor);

//            Log.Info(this, "Executed");
        }

        public bool Matches(string commandName)
        {
            return TransitionVerbPresentTense == commandName;
        }

        protected abstract ExpenseReportStatus GetEndStatus();
        protected abstract bool userCanExecute(Employee currentUser);

        protected virtual void preExecute(IStateCommandVisitor commandVisitor)
        {
        }

        protected abstract void postExecute(IStateCommandVisitor commandVisitor);


        protected virtual void sendChangeStateNotification(INotifier notifier)
        {
            notifier.SendChangeStateNotification(string.Format("Work order {0} changed from {1} to {2}",
                _expenseReport.Number, GetBeginStatus(), GetEndStatus()));
        }

        protected virtual void sendAssignedNotification(INotifier notifier)
        {
        }

        public bool ShouldSendAssignmentNotification()
        {
            return _expenseReport.Status == ExpenseReportStatus.Submitted;
        }
    }
}