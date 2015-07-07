using System;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.DataAccess;
using ClearMeasure.Bootcamp.DataAccess.Repositories;
using NHibernate;
using NUnit.Framework;

namespace ClearMeasure.Bootcamp.IntegrationTests.DataAccess
{
    [TestFixture]
    public class ExpenseReportRepositoryTester
    {
        [Test]
        public void ShouldGetByNumber()
        {
            new DatabaseTester().Clean();

            var creator = new Employee("1", "1", "1", "1");
            var order1 = new ExpenseReport();
            order1.Submitter = creator;
            order1.Number = "123";
            var order2 = new ExpenseReport();
            order2.Submitter = creator;
            order2.Number = "456";


            ISession session = DataContext.GetTransactedSession();
            session.SaveOrUpdate(creator);
            session.SaveOrUpdate(order1);
            session.SaveOrUpdate(order2);
            session.Transaction.Commit();
            session.Dispose();

            var repository = new ExpenseReportRepository();
            ExpenseReport order123 = repository.GetSingle("123");
            ExpenseReport order456 = repository.GetSingle("456");

            Assert.That(order123.Id, Is.EqualTo(order1.Id));
            Assert.That(order456.Id, Is.EqualTo(order2.Id));
        }

        [Test]
        public void ShouldSave()
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
            ISession session = DataContext.GetTransactedSession();
            session.SaveOrUpdate(creator);
            session.SaveOrUpdate(assignee);
            session.Transaction.Commit();

            var repository = new ExpenseReportRepository();
            repository.Save(order);

            session.Dispose();

            ISession session2 = DataContext.GetTransactedSession();
            var rehydratedReport = session2.Load<ExpenseReport>(order.Id);
            Assert.That(rehydratedReport.Id, Is.EqualTo(order.Id));
            Assert.That(rehydratedReport.Submitter.Id, Is.EqualTo(order.Submitter.Id));
            Assert.That(rehydratedReport.Approver.Id, Is.EqualTo(order.Approver.Id));
            Assert.That(rehydratedReport.Title, Is.EqualTo(order.Title));
            Assert.That(rehydratedReport.Description, Is.EqualTo(order.Description));
            Assert.That(rehydratedReport.Status, Is.EqualTo(order.Status));
            
            rehydratedReport.Number.ShouldEqual(order.Number);
        }

        [Test]
        public void ShouldSearchBySpecificationWithAssignee()
        {
            new DatabaseTester().Clean();

            var employee1 = new Employee("1", "1", "1", "1");
            var employee2 = new Employee("2", "2", "2", "2");
            var order1 = new ExpenseReport();
            order1.Submitter = employee2;
            order1.Approver = employee1;
            order1.Number = "123";
            var order2 = new ExpenseReport();
            order2.Submitter = employee1;
            order2.Approver = employee2;
            order2.Number = "456";

            ISession session = DataContext.GetTransactedSession();
            session.SaveOrUpdate(employee1);
            session.SaveOrUpdate(employee2);
            session.SaveOrUpdate(order1);
            session.SaveOrUpdate(order2);
            session.Transaction.Commit();
            session.Dispose();

            var repository = new ExpenseReportRepository();
            var specification = new SearchSpecification();
            specification.MatchApprover(employee1);
            ExpenseReport[] orders = repository.GetMany(specification);

            Assert.That(orders.Length, Is.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(order1.Id));
        }

        [Test]
        public void ShouldSearchBySpecificationWithCreator()
        {
            new DatabaseTester().Clean();

            var creator1 = new Employee("1", "1", "1", "1");
            var creator2 = new Employee("2", "2", "2", "2");
            var order1 = new ExpenseReport();
            order1.Submitter = creator1;
            order1.Number = "123";
            var order2 = new ExpenseReport();
            order2.Submitter = creator2;
            order2.Number = "456";

            ISession session = DataContext.GetTransactedSession();
            session.SaveOrUpdate(creator1);
            session.SaveOrUpdate(creator2);
            session.SaveOrUpdate(order1);
            session.SaveOrUpdate(order2);
            session.Transaction.Commit();
            session.Dispose();

            var repository = new ExpenseReportRepository();
            var specification = new SearchSpecification();
            specification.MatchSubmitter(creator1);
            ExpenseReport[] orders = repository.GetMany(specification);

            Assert.That(orders.Length, Is.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(order1.Id));
        }

        [Test]
        public void ShouldSearchBySpecificationWithFullSpecification()
        {
            new DatabaseTester().Clean();

            var employee1 = new Employee("1", "1", "1", "1");
            var employee2 = new Employee("2", "2", "2", "2");
            var order1 = new ExpenseReport();
            order1.Submitter = employee2;
            order1.Approver = employee1;
            order1.Number = "123";
            order1.Status = ExpenseReportStatus.Submitted;
            var order2 = new ExpenseReport();
            order2.Submitter = employee1;
            order2.Approver = employee2;
            order2.Number = "456";
            order2.Status = ExpenseReportStatus.Draft;

            ISession session = DataContext.GetTransactedSession();
            session.SaveOrUpdate(employee1);
            session.SaveOrUpdate(employee2);
            session.SaveOrUpdate(order1);
            session.SaveOrUpdate(order2);
            session.Transaction.Commit();
            session.Dispose();

            var repository = new ExpenseReportRepository();
            var specification = new SearchSpecification();
            specification.MatchStatus(ExpenseReportStatus.Submitted);
            specification.MatchSubmitter(employee2);
            specification.MatchApprover(employee1);
            ExpenseReport[] orders = repository.GetMany(specification);

            Assert.That(orders.Length, Is.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(order1.Id));
        }

        [Test]
        public void ShouldSearchBySpecificationWithStatus()
        {
            new DatabaseTester().Clean();

            var employee1 = new Employee("1", "1", "1", "1");
            var employee2 = new Employee("2", "2", "2", "2");
            var order1 = new ExpenseReport();
            order1.Submitter = employee2;
            order1.Approver = employee1;
            order1.Number = "123";
            order1.Status = ExpenseReportStatus.Submitted;
            var order2 = new ExpenseReport();
            order2.Submitter = employee1;
            order2.Approver = employee2;
            order2.Number = "456";
            order2.Status = ExpenseReportStatus.Draft;

            ISession session = DataContext.GetTransactedSession();
            session.SaveOrUpdate(employee1);
            session.SaveOrUpdate(employee2);
            session.SaveOrUpdate(order1);
            session.SaveOrUpdate(order2);
            session.Transaction.Commit();
            session.Dispose();

            var repository = new ExpenseReportRepository();
            var specification = new SearchSpecification();
            specification.MatchStatus(ExpenseReportStatus.Submitted);
            ExpenseReport[] orders = repository.GetMany(specification);

            Assert.That(orders.Length, Is.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(order1.Id));
        }

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

            ISession session = DataContext.GetTransactedSession();
            session.SaveOrUpdate(creator);
            session.SaveOrUpdate(assignee);
            session.Transaction.Commit();

            var repository = new ExpenseReportRepository();
            repository.Save(order);

            session.Dispose();

            ISession session2 = DataContext.GetTransactedSession();
            var rehydratedReport = session2.Load<ExpenseReport>(order.Id);
            var x = order.GetAuditEntries()[0];
            var y = rehydratedReport.GetAuditEntries()[0];
            Assert.That(x.BeginStatus, Is.EqualTo(y.BeginStatus));
        }

    }
}