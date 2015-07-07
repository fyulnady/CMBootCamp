using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.ExpenseReportWorkflow;
using ClearMeasure.Bootcamp.Core.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace ClearMeasure.Bootcamp.UnitTests.Core.Model.ExpenseReportWorkflow
{
    [TestFixture]
    public class SubmittedToApprovedCommandTester : StateCommandBaseTester
    {
        [Test]
        public void ShouldNotBeValidInWrongStatus()
        {
            var order = new ExpenseReport();
            order.Status = ExpenseReportStatus.Submitted;
            var employee = new Employee();
            order.Submitter = employee;

            var command = new SubmittedToApprovedCommand(order, employee);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldNotBeValidWithWrongEmployee()
        {
            var order = new ExpenseReport();
            order.Status = ExpenseReportStatus.Draft;
            var employee = new Employee();
            var differentEmployee = new Employee();
            order.Approver = employee;

            var command = new SubmittedToApprovedCommand(order, differentEmployee);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldBeValid()
        {
            var order = new ExpenseReport();
            order.Status = ExpenseReportStatus.Submitted;
            var employee = new Employee();
            order.Approver = employee;

            var command = new SubmittedToApprovedCommand(order, employee);
            Assert.That(command.IsValid(), Is.True);
        }

        [Test]
        public void ShouldTransitionStateProperly()
        {
            var order = new ExpenseReport();
            order.Number = "123";
            order.Status = ExpenseReportStatus.Submitted;
            var employee = new Employee();
            order.Approver = employee;

            var mocks = new MockRepository();
            var commandVisitor = mocks.DynamicMock<IStateCommandVisitor>();
            commandVisitor.Save(order);
            commandVisitor.GoToEdit(order);
            mocks.ReplayAll();

            var command = new SubmittedToApprovedCommand(order, employee);
            command.Execute(commandVisitor);

            mocks.VerifyAll();
            Assert.That(order.Status, Is.EqualTo(ExpenseReportStatus.Approved));
        }

        protected override StateCommandBase GetStateCommand(ExpenseReport order, Employee employee)
        {
            return new SubmittedToApprovedCommand(order, employee);
        }
    }
}