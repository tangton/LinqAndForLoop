using LinqAndForLoop.Library.Models;
using LinqAndForLoop.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqAndForLoop.Library.Services
{
    public class LinqSearchService : ISearchService
    {
        public Account GetFirstMatch(IList<Account> list, string name, int number)
        {
            Func<Account, bool> predicate = x => x.Name == name && x.Number == number;
            return list.FirstOrDefault(predicate);
        }
    }
}
