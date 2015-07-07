using System;
using System.Collections.Generic;
using System.Linq;

namespace ClearMeasure.Bootcamp.Core.Model
{
    public class ExpenseReport
    {
        public IList<AuditEntry> _auditEntries = new List<AuditEntry>(); 
        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual ExpenseReportStatus Status { get; set; }
        public virtual Employee Submitter { get; set; }
        public virtual Employee Approver { get; set; }
        public virtual string Number { get; set; }
        public DateTime? CreatedDate { get; set; }

        public ExpenseReport()
        {
            Status = ExpenseReportStatus.Draft;
            Description = "";
            Title = "";
        }

        public virtual string FriendlyStatus
        {
            get { return GetTextForStatus(); }
        }

        protected virtual string GetTextForStatus()
        {
            return Status.ToString();
        }

        public override string ToString()
        {
            return "ExpenseReport " + Number;
        }

        public virtual void ChangeStatus(ExpenseReportStatus status)
        {
            Status = status;
        }

        public virtual void ChangeStatus(Employee employee, DateTime date, ExpenseReportStatus status)
        {
            var auditItem = new AuditEntry(employee, date, Status, status);
            _auditEntries.Add(auditItem);
            Status = status;
        }

        public AuditEntry[] GetAuditEntries()
        {
            return _auditEntries.ToArray();
        }

        public void AddAuditEntry(AuditEntry auditEntry)
        {
            _auditEntries.Add(auditEntry);
        }
    }
}