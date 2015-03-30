using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LinqAndForLoop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int CollectionListSize = 50;
        private const int RunLoopCount = 1000000;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource.Cancel();

            btnCancel.Visibility = Visibility.Collapsed;
            btnRun.Visibility = Visibility.Visible;
            btnRun.IsEnabled = true;
        }

        private List<Account> GenerateAccounts()
        {
            var accounts = new List<Account>();

            var random = new Random();
            for (var i = 0; i < CollectionListSize; i++)
            {
                accounts.Add(new Account
                {
                    Name = "Savings",
                    Number = random.Next(10000, 99999)
                });
            }

            return accounts;
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource = new CancellationTokenSource();

            btnCancel.Visibility = Visibility.Visible;
            btnRun.Visibility = Visibility.Collapsed;
            btnRun.IsEnabled = false;

            var accounts = GenerateAccounts();

            lblListGenerated.Content = "Generated list: " + String.Join(", ", accounts);

            var taskList = new List<Task>();

            var taskLinq = PrepareAndRunFind(LinqSearchManyTimes, accounts, pbLinq, lblResultLinq, _tokenSource.Token);
            var taskFor = PrepareAndRunFind(ForLoopSearchManyTimes, accounts, pbFor, lblResultFor, _tokenSource.Token);

            taskList.Add(taskLinq);
            taskList.Add(taskFor);

            await Task.WhenAll(taskList);

            btnCancel.Visibility = Visibility.Collapsed;
            btnRun.Visibility = Visibility.Visible;
            btnRun.IsEnabled = true;
        }

        private async Task PrepareAndRunFind(Func<List<Account>, Account> findFunc, List<Account> list, ProgressBar progressBar, Label labelResult, CancellationToken cancellationToken)
        {
            labelResult.Content = "Pending...";
            progressBar.Value = 0;
            progressBar.Maximum = CollectionListSize;

            progressBar.IsIndeterminate = true;
            var task = Task.Factory.StartNew(() => RunSearch(findFunc, list), cancellationToken);

            var failed = false;
            var cancelled = false;
            var message = string.Empty;

            var durationOfSort = new TimeSpan();

            try
            {
                await task;

                durationOfSort = task.Result;
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
            }
            catch (Exception ex)
            {
                failed = true;
                message = ex.Message;
            }

            if (cancelled)
            {
                progressBar.Value = 0;
                labelResult.Content = "Sort cancelled.";
            }
            else if (!failed)
            {
                progressBar.IsIndeterminate = false;
                progressBar.Value = CollectionListSize;
                labelResult.Content = "Done. Time taken: " + durationOfSort;
            }
            else
            {
                labelResult.Content = "Sort failed. Message: " + message;
            }
        }

        private static TimeSpan RunSearch(Func<List<Account>, Account> findFunc, List<Account> listToSearch)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var item = findFunc(listToSearch);

            stopWatch.Stop();

            return stopWatch.Elapsed;
        }

        private Account LinqSearchManyTimes(List<Account> list)
        {
            for (var i = 0; i < RunLoopCount; i++)
            {
                LinqSearch(list, "Name", 10100);
            }

            return null;
        }

        private Account ForLoopSearchManyTimes(List<Account> list)
        {
            for (var i = 0; i < RunLoopCount; i++)
            {
                ForLoopSearch(list, "Name", 10100);
            }

            return null;
        }


        private Account LinqSearch(List<Account> list, string name, int number)
        {
            return list.FirstOrDefault(x => x.Name == name && x.Number == number);
        }

        private Account LinqSearchBreakDown(List<Account> list, string name, int number)
        {
            Func<Account, bool> predicate = x => x.Name == name && x.Number == number;
            return list.FirstOrDefault(predicate);
        }

        private class Lambda1Environment
        {
            public string CapturedName;
            public int CapturedNumber;
            public bool Evaluate(Account account)
            {
                return account.Name == this.CapturedName && account.Number == this.CapturedNumber;
            }
        }

        private Account LinqSearchBreakDown2(List<Account> list, string name, int number)
        {
            var l = new Lambda1Environment() { CapturedName = name, CapturedNumber = number };
            var predicate = new Func<Account, bool>(l.Evaluate);

            return list.FirstOrDefault(predicate);
        }

        private Account LinqSearchBreakDown3(List<Account> list, string name, int number)
        {
            var l = new Lambda1Environment() { CapturedName = name, CapturedNumber = number };
            var predicate = new Func<Account, bool>(l.Evaluate);

            IEnumerable<Account> enumerable = list;
            IEnumerator<Account> enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                {
                    return enumerator.Current;
                }
            }
            return default(Account);
        }

        private Account ForLoopSearch(List<Account> list, string name, int number)
        {
            foreach (var item in list)
            {
                if (item.Name == name && item.Number == number)
                {
                    return item;
                }
            }

            return default(Account); 
        }
    }
}
