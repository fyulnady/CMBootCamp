using System;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.DataAccess;
using ClearMeasure.Bootcamp.IntegrationTests.DataAccess;
using NHibernate;
using NUnit.Framework;

namespace ClearMeasure.Bootcamp.IntegrationTests
{
    [TestFixture, Explicit]
    public class ZDataLoader
    {
        [Test, Category("DataLoader")]
        public void PopulateDatabase()
        {
            new DatabaseTester().Clean();
            ISession session = DataContext.GetTransactedSession();

            var lead = new Role("Facility Lead");
            var fulfillment = new Role("Fulfillment");
            session.SaveOrUpdate(lead);
            session.SaveOrUpdate(fulfillment);

            //Trainer1
            var jpalermo = new Employee("jpalermo", "Jeffrey", "Palermo", "jeffrey@clear-measure.com");
            jpalermo.AddRole(lead);
            jpalermo.AddRole(fulfillment);
            session.SaveOrUpdate(jpalermo);

            //Person 1
            
            //Person 2
            
            //Person 3
            
            //Person 4
            
            //Person 5
            
            //Person 6
            
            //Person 7
            
            //Person 8
            
            //Person 9

            //Person 10

            //Person 11

            //Person 12

            //Person 13

            var hsimpson = new Employee("hsimpson", "Homer", "Simpson", "homer@simpson.com");
            hsimpson.AddRole(fulfillment);
            session.SaveOrUpdate(hsimpson);

            foreach (ExpenseReportStatus status in ExpenseReportStatus.GetAllItems())
            {
                var order = new ExpenseReport();
                order.Number = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
                order.Submitter = jpalermo;
                order.Approver = jpalermo;
                order.Status = status;
                order.Title = "Work Order starting in status " + status;
                order.Description = "Foo, foo, foo, foo " + status;
                order.CreatedDate = new DateTime(2000, 1, 1, 8, 0, 0);
                new DateTime(2000, 1, 1, 8, 0, 0);
                order.ChangeStatus(ExpenseReportStatus.Draft);
                order.ChangeStatus(ExpenseReportStatus.Submitted);
                order.ChangeStatus(ExpenseReportStatus.Approved);

                session.SaveOrUpdate(order);
            }

            var order2 = new ExpenseReport();
            order2.Number = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            order2.Submitter = jpalermo;
            order2.Approver = jpalermo;
            order2.Status = ExpenseReportStatus.Approved;
            order2.Title = "Work Order starting in status ";
            order2.Description = "Foo, foo, foo, foo ";
            order2.CreatedDate = new DateTime(2000, 1, 1, 8, 0, 0);
            new DateTime(2000, 1, 1, 8, 0, 0);
            session.SaveOrUpdate(order2);

            session.Transaction.Commit();
            session.Dispose();
        }
    }
}