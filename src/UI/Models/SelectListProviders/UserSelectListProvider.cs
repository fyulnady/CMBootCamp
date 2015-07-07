using System.Collections.Generic;
using System.Web.Mvc;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.UI.Models.SelectListProviders
{
    public class UserSelectListProvider
    {
        private static readonly IEmployeeRepository _repository;

        static UserSelectListProvider()
        {
            _repository = DependencyResolver.Current.GetService<IEmployeeRepository>();
        }

        public static IEnumerable<SelectListItem> GetOptions()
        {
            return GetOptions(null);
        }

        public static IEnumerable<SelectListItem> GetOptions(string selected)
        {
            var result = new List<SelectListItem>();

            var empSpec = new EmployeeSpecification();
            foreach (Employee employee in _repository.GetEmployees(empSpec))
            {
                result.Add(new SelectListItem
                {
                    Text = employee.GetFullName(),
                    Value = employee.UserName,
                    Selected = (employee.UserName == selected)
                });
            }
            _repository.GetEmployees(empSpec);

            return result;
        }

        public static IEnumerable<SelectListItem> GetOptionsWithBlank(string selected)
        {
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem {Text = "<Any>", Value = ""});

            result.AddRange(GetOptions(selected));

            return result;
        }
    }
}