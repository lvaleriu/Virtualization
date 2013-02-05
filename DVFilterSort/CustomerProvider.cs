#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DataVirtualization;

#endregion

namespace DVFilterSort
{
    public class CustomerProvider : IItemsProvider<Customer>
    {
        private readonly List<Customer> _customers;
        private readonly DateTime? _dateFrom;
        private readonly DateTime? _dateTo;
        private readonly string _sortField;
        private int _count;

        public CustomerProvider()
        {
            _dateFrom = DateTime.Today.AddYears(-100);
            _dateTo = DateTime.Today.AddYears(100);
            _sortField = "CustomerSince DESC";

            _customers = new List<Customer>();
            for (int i = 0; i < 10000; i++)
            {
                _customers.Add(new Customer
                    {
                        AmountPaidInternationalCalls = i%100,
                        AmountPaidLocalCalls = i%100,
                        AmountPaidNationalCalls = i%100,
                        CustomerSince = _dateFrom.Value.AddDays(i),
                        FirstName = string.Format("Customer {0}", i),
                        Id = i,
                        LastName = string.Format("LastName {0}", i),
                        JoinedPreferredProgram = i%2 == 0,
                        NumberFamilyMembersInPlan = i%4,
                        Region = i%100,
                    });
            }
        }

        public CustomerProvider(DateTime? dateFrom, DateTime? dateTo, string sortField)
            : this()
        {
            if (dateFrom != null)
            {
                _dateFrom = dateFrom;
            }
            if (dateTo != null)
            {
                _dateTo = dateTo;
            }

            _sortField = sortField;
        }

        public int FetchCount()
        {
            Thread.Sleep(1000);
            _count = _customers.Count(e => e.CustomerSince >= _dateFrom && e.CustomerSince <= _dateTo);
            return _count;
        }

        public IList<Customer> FetchRange(int startIndex, int pageCount, out int overallCount)
        {
            Thread.Sleep(500);

            overallCount = _count; // In this case it's ok not to get the count again because we're assuming the data in the database is not changing.

            if (_sortField.Contains("DESC"))

                return _customers.Where(e => e.CustomerSince >= _dateFrom && e.CustomerSince <= _dateTo)
                                 .OrderBy(e => e.FirstName)
                                 .Skip(startIndex)
                                 .Take(pageCount).ToList();
            else
                return _customers.Where(e => e.CustomerSince >= _dateFrom && e.CustomerSince <= _dateTo)
                                 .OrderByDescending(e => e.FirstName)
                                 .Skip(startIndex)
                                 .Take(pageCount).ToList();
        }
    }
}