using System;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.DataAccess;
using ClearMeasure.Bootcamp.DataAccess.Repositories;
using NHibernate;
using NUnit.Framework;

namespace ClearMeasure.Bootcamp.IntegrationTests.DataAccess.Mappings
{
    [TestFixture]
    public class ExpenseReportMappingTester
    {
        [Test]
        public void ShouldSaveAuditEntries()
        {
            new DatabaseTester().Clean();

            var creator = new Employee("1", "1", "1", "1");
            var assignee = new Employee("2", "2", "2", "2");
            var order = new ExpenseReport();
            order.Submitter = creator;
            order.Approver = assignee;
            order.Title = "foo";
            order.Description = "bar";
            order.ChangeStatus(ExpenseReportStatus.Approved);
            order.Number = "123";
            order.AddAuditEntry(new AuditEntry(creator, DateTime.Now, ExpenseReportStatus.Submitted,
                                                  ExpenseReportStatus.Approved));

            using (ISession session = DataContext.GetTransactedSession())
            {
                session.SaveOrUpdate(creator);
                session.SaveOrUpdate(assignee);
                session.Transaction.Commit();
            }

            var repository = new ExpenseReportRepository();
            repository.Save(order);

            ExpenseReport rehydratedExpenseReport;
            using (ISession session2 = DataContext.GetTransactedSession())
            {
                rehydratedExpenseReport = session2.Load<ExpenseReport>(order.Id);
            }

            var x = order.GetAuditEntries()[0];
            var y = rehydratedExpenseReport.GetAuditEntries()[0];
            Assert.That(x.BeginStatus, Is.EqualTo(y.BeginStatus));
        }

    }
}