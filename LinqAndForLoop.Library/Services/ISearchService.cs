﻿using LinqAndForLoop.Library.Models;
using System.Collections.Generic;

namespace LinqAndForLoop.Library.Services
{
    public interface ISearchService
    {
        Account GetFirstMatch(IEnumerable<Account> list, string name, int number);
    }
}
