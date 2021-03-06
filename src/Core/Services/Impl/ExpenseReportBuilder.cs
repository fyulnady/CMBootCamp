using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Services.Impl
{
	public class ExpenseReportBuilder : IExpenseReportBuilder
	{
		private readonly INumberGenerator _numberGenerator;
		private readonly ICalendar _calendar;

		public ExpenseReportBuilder(INumberGenerator numberGenerator, ICalendar calendar)
		{
			_numberGenerator = numberGenerator;
			_calendar = calendar;
		}

		public ExpenseReport Build(Employee creator)
		{
			ExpenseReport expenseReport = new ExpenseReport();
			expenseReport.Number = _numberGenerator.GenerateNumber();
			expenseReport.Submitter = creator;
		    expenseReport.CreatedDate = _calendar.GetCurrentTime();
			expenseReport.Status = ExpenseReportStatus.Draft;
			return expenseReport;
		}
	}
}