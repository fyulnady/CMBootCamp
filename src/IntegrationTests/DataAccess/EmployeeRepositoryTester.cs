using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.DataAccess;
using ClearMeasure.Bootcamp.DataAccess.Repositories;
using NHibernate;
using NUnit.Framework;

namespace ClearMeasure.Bootcamp.IntegrationTests.DataAccess
{
    [TestFixture]
    public class EmployeeRepositoryTester
    {
        [Test]
        public void ShouldFindEmployeeByUsername()
        {
            new DatabaseTester().Clean();

            ISession session = DataContext.GetTransactedSession();
            var one = new Employee("1", "first1", "last1", "email1");
            var two = new Employee("2", "first2", "last2", "email2");
            var three = new Employee("3", "first3", "last3", "email3");
            session.SaveOrUpdate(one);
            session.SaveOrUpdate(two);
            session.SaveOrUpdate(three);
            session.Transaction.Commit();
            session.Dispose();

            IEmployeeRepository repository = new EmployeeRepository();
            Employee employee = repository.GetByUserName("1");
            Assert.That(employee.Id, Is.EqualTo(one.Id));
        }

        [Test]
        public void ShouldGetAllEmployees()
        {
            new DatabaseTester().Clean();

            ISession session = DataContext.GetTransactedSession();
            var one = new Employee("1", "first1", "last1", "email1");
            var two = new Employee("2", "first2", "last2", "email2");
            var three = new Employee("3", "first3", "last3", "email3");
            session.SaveOrUpdate(two);
            session.SaveOrUpdate(three);
            session.SaveOrUpdate(one);
            session.Transaction.Commit();
            session.Dispose();

            IEmployeeRepository repository = new EmployeeRepository();
            Employee[] employees = repository.GetEmployees(EmployeeSpecification.All);

            Assert.That(employees.Length, Is.EqualTo(3));
            Assert.That(employees[0].UserName, Is.EqualTo("1"));
            Assert.That(employees[0].FirstName, Is.EqualTo("first1"));
            Assert.That(employees[0].LastName, Is.EqualTo("last1"));
            Assert.That(employees[0].EmailAddress, Is.EqualTo("email1"));
        }

        [Test]
        public void ShouldGetAllEmployeesForFulfillment()
        {
            new DatabaseTester().Clean();

            ISession session = DataContext.GetTransactedSession();
            var role = new Role("foo");
            var one = new Employee("1", "first1", "last1", "email1");
            one.AddRole(role);
            var two = new Employee("2", "first2", "last2", "email2");
            two.AddRole(role);
            var three = new Employee("3", "first3", "last3", "email3");
            session.SaveOrUpdate(role);
            session.SaveOrUpdate(two);
            session.SaveOrUpdate(three);
            session.SaveOrUpdate(one);
            session.Transaction.Commit();
            session.Dispose();

            IEmployeeRepository repository = new EmployeeRepository();
            Employee[] employees =
                repository.GetEmployees(new EmployeeSpecification(true));

            Assert.That(employees.Length, Is.EqualTo(2));
        }

        [Test]
        public void ShouldSaveRolesWithEmployee()
        {
            new DatabaseTester().Clean();
            var role1 = new Role("foo");
            var role2 = new Role("bar");
            var emp1 = new Employee("1", "first1", "last1", "email1");
            emp1.AddRole(role1);
            emp1.AddRole(role2);

            ISession session = DataContext.GetTransactedSession();
            session.SaveOrUpdate(role1);
            session.SaveOrUpdate(role2);
            session.SaveOrUpdate(emp1);
            session.Transaction.Commit();
            session.Dispose();

            ISession session2 = DataContext.GetTransactedSession();
            var rehydratedEmployee = session2.Load<Employee>(emp1.Id);
            Assert.That(rehydratedEmployee.GetRoles().Length,
                        Is.EqualTo(2));
        }
    }
}