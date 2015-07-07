using System;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.ExpenseReportWorkflow;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace ClearMeasure.Bootcamp.UnitTests.Core.Services
{
    [TestFixture]
    public class WorkflowFacilitatorTester
    {
        [Test]
        public void ShouldGetNoValidStateCommandsForWrongUser()
        {
            var facilitator = new WorkflowFacilitator(new StubbedCalendar(new DateTime(2000, 1, 1)));
            var report = new ExpenseReport();
            var employee = new Employee();
            IStateCommand[] commands = facilitator.GetValidStateCommands(report, employee);

            Assert.That(commands.Length, Is.EqualTo(0));
        }


        [Test]
        public void ShouldReturnAllStateCommandsInCorrectOrder()
        {
            var facilitator = new WorkflowFacilitator(new StubbedCalendar(new DateTime(2000, 1, 1)));
            IStateCommand[] commands = facilitator.GetAllStateCommands(new ExpenseReport(), new Employee());

            Assert.That(commands.Length, Is.EqualTo(3));

            Assert.That(commands[0], Is.InstanceOf(typeof (DraftingCommand)));
            Assert.That(commands[1], Is.InstanceOf(typeof (DraftToSubmittedCommand)));
            Assert.That(commands[2], Is.InstanceOf(typeof (SubmittedToApprovedCommand)));
        }

        [Test]
        public void ShouldFilterFullListToReturnValidCommands()
        {
            var mocks = new MockRepository();
            var facilitator = mocks.PartialMock<WorkflowFacilitator>(new StubbedCalendar(new DateTime(2000, 1, 1)));
            var commandsToReturn = new IStateCommand[]
                                       {
                                           new StubbedStateCommand(true), new StubbedStateCommand(true),
                                           new StubbedStateCommand(false)
                                       };

            Expect.Call(facilitator.GetAllStateCommands(null, null)).IgnoreArguments().Return(commandsToReturn);
            mocks.ReplayAll();

            IStateCommand[] commands = facilitator.GetValidStateCommands(null, null);

            mocks.VerifyAll();
            Assert.That(commands.Length, Is.EqualTo(2));
        }

        public class StubbedStateCommand : IStateCommand
        {
            private bool _isValid;

            public StubbedStateCommand(bool isValid)
            {
                _isValid = isValid;
            }

            public bool IsValid()
            {
                return _isValid;
            }

            public void Execute(IStateCommandVisitor commandVisitor)
            {
                throw new NotImplementedException();
            }

            public string TransitionVerbPresentTense
            {
                get { throw new NotImplementedException(); }
            }

            public bool Matches(string commandName)
            {
                throw new NotImplementedException();
            }

            public ExpenseReportStatus GetBeginStatus()
            {
                throw new NotImplementedException();
            }
        }
    }
}