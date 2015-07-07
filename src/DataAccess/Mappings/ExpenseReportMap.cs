using System.Collections.Generic;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.DataAccess.Repositories;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ClearMeasure.Bootcamp.DataAccess.Mappings
{
    public class ExpenseReportMap : ClassMap<ExpenseReport>
    {
        public ExpenseReportMap()
        {
            Not.LazyLoad();
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Number).Not.Nullable().Length(5);
            Map(x => x.Title).Not.Nullable().Length(200);
            Map(x => x.Description).Not.Nullable().Length(4000);
            Map(x => x.CreatedDate);
            Map(x => x.Status).Not.Nullable().CustomType<ExpenseReportStatusType>();
            References(x => x.Submitter).Not.Nullable().Column("SubmitterId");
            References(x => x.Approver).Column("ApproverId");

            HasMany(Reveal.Member<ExpenseReport, IEnumerable<AuditEntry>>("_auditEntries"))
                .AsList(part =>
                {
                    part.Column("Sequence");
                    part.Type<int>();
                })
                .Table("AuditEntry")
                .Cascade.AllDeleteOrphan()
                .KeyColumn("ExpenseReportId")
                .Component(part =>
                {
                    part.References(x => x.Employee).Column("EmployeeId");
                    part.Map(x => x.ArchivedEmployeeName);
                    part.Map(x => x.Date);
                    part.Map(x => x.BeginStatus).CustomType<ExpenseReportStatusType>();
                    part.Map(x => x.EndStatus).CustomType<ExpenseReportStatusType>();
                })
                .Access.CamelCaseField()
                .Not.LazyLoad();
        }
    }
}