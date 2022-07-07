using LinqAndForLoop.Library.Models;
using LinqAndForLoop.Library.Services;
using System.Collections.Generic;

namespace LinqAndForLoop.Library.Services
{
    public class ForLoopSearchService : ISearchService
    {
        public Account GetFirstMatch(IList<Account> list, string name, int number)
        {
            foreach (var item in list)
            {
                if (item.Name == name && item.Number == number)
                {
                    return item;
                }
            }

            return default;
        }
    }
}
