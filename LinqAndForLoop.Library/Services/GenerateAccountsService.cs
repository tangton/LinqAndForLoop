using LinqAndForLoop.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqAndForLoop.Library.Services
{
    public interface IGenerateAccountsService
    {
        IList<Account> GenerateAccounts(int numberOfAccounts);
    }

    public class GenerateAccountsService : IGenerateAccountsService
    {
        public IList<Account> GenerateAccounts(int numberOfAccounts)
        {
            var accounts = new List<Account>();

            var random = new Random();
            for (var i = 0; i < numberOfAccounts; i++)
            {
                accounts.Add(new Account
                {
                    Name = "Savings",
                    Number = random.Next(10000, 99999)
                });
            }

            return accounts;
        }
    }
}
