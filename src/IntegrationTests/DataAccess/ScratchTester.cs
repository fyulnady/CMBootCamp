using System;
using System.Collections.Generic;
using System.Data;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.DataAccess;
using ClearMeasure.Bootcamp.UI.DependencyResolution;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;
using StructureMap;

namespace ClearMeasure.Bootcamp.IntegrationTests.DataAccess
{
    [TestFixture, Explicit]
    public class ScratchTester
    {
        [Test]
        public void Foo1()
        {
            ISession session = DataContext.GetTransactedSession();
            ICriteria criteria = session.CreateCriteria(typeof (ExpenseReport));
            criteria.CreateAlias("Submitter", "crtor");
            criteria.Add(Expression.Eq("crtor.FirstName", "P-Mo"));
            criteria.AddOrder(Order.Asc("Number"));

            IList<ExpenseReport> list = criteria.List<ExpenseReport>();

            foreach (ExpenseReport order in list)
            {
                Console.WriteLine(order);
            }
        }

        [Test]
        public void Foo3()
        {
            ISession session = DataContext.GetTransactedSession();
            IQuery query =
                session.CreateQuery(@"from ExpenseReport wo
                where wo.Submitter.FirstName = :nameparam");
            query.SetString("nameparam", "P-Mo");
            IList<ExpenseReport> list = query.List<ExpenseReport>();


            foreach (ExpenseReport order in list)
            {
                Console.WriteLine(order);
                Console.WriteLine(order.Submitter.EmailAddress);
                order.Submitter.EmailAddress = "foo@bar.com";
                Console.WriteLine(order.Submitter.EmailAddress);
            }
        }

        [Test]
        public void Foo4()
        {
            ISession session = DataContext.GetTransactedSession();
            IQuery query =
                session.CreateQuery(@"from ExpenseReport wo
                join fetch wo.AuditEntries");
            query.SetResultTransformer(new DistinctRootEntityResultTransformer());
            IList<ExpenseReport> list = query.List<ExpenseReport>();


            foreach (ExpenseReport order in list)
            {
                Console.WriteLine(order);
            }
        }

        [Test]
        public void Foo5()
        {
            ISession session = DataContext.GetTransactedSession();
            IQuery query =
                session.CreateQuery(@"select count(wo) from ExpenseReport wo
                ");

            object somefin = query.UniqueResult();


            Console.WriteLine(somefin);
        }

        [Test]
        public void Foo6()
        {
            ISession session = DataContext.GetTransactedSession();
            Role role = new Role("");
            Employee employee = new Employee("1", "1", "1", "1");
            employee.AddRole(role);

            session.SaveOrUpdate(employee);
            session.Transaction.Commit();
        }

        [Test]
        public void Foo8()
        {
            ISession session = DataContext.GetTransactedSession();
            ICriteria criteria = session.CreateCriteria(typeof(Employee));
            IList<Employee> list = criteria.List<Employee>();
            foreach (Employee employee in list)
            {
                Console.WriteLine(employee.GetType().Name + " - " + employee);
            }
        }

        [Test]
        public void Foo11()
        {
            ISession session = DataContext.GetTransactedSession();
            IDbCommand command = session.Connection.CreateCommand();
            command.CommandText = "print 'howdy yall'";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            ICriteria criteria = session.CreateCriteria(typeof(Employee));
            IList<Employee> list = criteria.List<Employee>();
            foreach (Employee employee in list)
            {
                Console.WriteLine(employee.GetType().Name + " - " + employee);
            }
        }

        [Test]
        public void TestStructureMap()
        {
            var container = new Container(new StructureMapRegistry());
            var whatDoIHave = container.WhatDoIHave();
//            Console.WriteLine(whatDoIHave);
            
            var session = container.GetInstance<IUserSession>();
            var currentUser = session.GetCurrentUser();
            var fullName = currentUser.GetFullName();
            Console.WriteLine(fullName);
        }
    }
}