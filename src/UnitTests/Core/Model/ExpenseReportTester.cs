using System;
using ClearMeasure.Bootcamp.Core.Model;
using NUnit.Framework;

namespace ClearMeasure.Bootcamp.UnitTests.Core.Model
{
    [TestFixture]
    public class ExpenseReportTester
    {
        [Test]
        public void PropertiesShouldInitializeToProperDefaults()
        {
            var report = new ExpenseReport();
            Assert.That(report.Id, Is.EqualTo(Guid.Empty));
            Assert.That(report.Title, Is.EqualTo(string.Empty));
            Assert.That(report.Description, Is.EqualTo(string.Empty));
            Assert.That(report.Status, Is.EqualTo(ExpenseReportStatus.Draft));
            Assert.That(report.Number, Is.EqualTo(null));
            Assert.That(report.Submitter, Is.EqualTo(null));
            Assert.That(report.Approver, Is.EqualTo(null));
            Assert.That(report.GetAuditEntries().Length, Is.EqualTo(0));
        }

        [Test]
        public void ToStringShouldReturnNumber()
        {
            var order = new ExpenseReport();
            order.Number = "456";
            Assert.That(order.ToString(), Is.EqualTo("ExpenseReport 456"));
        }

        [Test]
        public void PropertiesShouldGetAndSetValuesProperly()
        {
            var report = new ExpenseReport();
            Guid guid = Guid.NewGuid();
            var creator = new Employee();
            var assignee = new Employee();
            DateTime auditDate = new DateTime(2000, 1, 1, 8, 0, 0);
            AuditEntry testAudit = new AuditEntry(creator, auditDate, ExpenseReportStatus.Submitted, ExpenseReportStatus.Approved);

            report.Id = guid;
            report.Title = "Title";
            report.Description = "Description";
            report.Status = ExpenseReportStatus.Approved;
            report.Number = "Number";
            report.Submitter = creator;
            report.Approver = assignee;
            report.AddAuditEntry(testAudit);

            Assert.That(report.Id, Is.EqualTo(guid));
            Assert.That(report.Title, Is.EqualTo("Title"));
            Assert.That(report.Description, Is.EqualTo("Description"));
            Assert.That(report.Status, Is.EqualTo(ExpenseReportStatus.Approved));
            Assert.That(report.Number, Is.EqualTo("Number"));
            Assert.That(report.Submitter, Is.EqualTo(creator));
            Assert.That(report.Approver, Is.EqualTo(assignee));
            Assert.That(report.GetAuditEntries()[0].BeginStatus, Is.EqualTo(ExpenseReportStatus.Submitted));
            Assert.That(report.GetAuditEntries()[0].EndStatus, Is.EqualTo(ExpenseReportStatus.Approved));
            Assert.That(report.GetAuditEntries()[0].Date, Is.EqualTo(auditDate));
            Assert.That(report.GetAuditEntries()[0].ArchivedEmployeeName, Is.EqualTo(" "));
        }

        [Test]
        public void ShouldShowFriendlyStatusValuesAsStrings()
        {
            var report = new ExpenseReport();
            report.Status = ExpenseReportStatus.Submitted;

            Assert.That(report.FriendlyStatus, Is.EqualTo("Submitted"));
        }

        [Test]
        public void ShouldChangeStatus()
        {
            var report = new ExpenseReport();
            report.Status = ExpenseReportStatus.Draft;
            report.ChangeStatus(ExpenseReportStatus.Submitted);
            Assert.That(report.Status, Is.EqualTo(ExpenseReportStatus.Submitted));
        }
    }
}