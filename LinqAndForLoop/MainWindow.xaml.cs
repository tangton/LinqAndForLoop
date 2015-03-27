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
        private const int CollectionListSize = 1000000;
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

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource = new CancellationTokenSource();

            btnCancel.Visibility = Visibility.Visible;
            btnRun.Visibility = Visibility.Collapsed;
            btnRun.IsEnabled = false;

            var randomIntList = new List<int>();

            var random = new Random();
            for (var i = 0; i < CollectionListSize; i++)
            {
                randomIntList.Add(random.Next(1000));
            }

            lblListGenerated.Content = "Generated list: " + String.Join(", ", randomIntList);

            var taskList = new List<Task>();

            var taskLinq = PrepareAndRunFind(LinqFind, randomIntList, pbLinq, lblResultLinq, _tokenSource.Token);
            var taskFor = PrepareAndRunFind(ForLoopFind, randomIntList, pbFor, lblResultFor, _tokenSource.Token);

            taskList.Add(taskLinq);
            taskList.Add(taskFor);

            await Task.WhenAll(taskList);

            btnCancel.Visibility = Visibility.Collapsed;
            btnRun.Visibility = Visibility.Visible;
            btnRun.IsEnabled = true;
        }

        private async Task PrepareAndRunFind(Func<List<int>, int> findFunc, List<int> list, ProgressBar progressBar, Label labelResult, CancellationToken cancellationToken)
        {
            labelResult.Content = "Pending...";
            progressBar.Value = 0;
            progressBar.Maximum = CollectionListSize;

            progressBar.IsIndeterminate = true;
            var task = Task.Factory.StartNew(() => RunFind(findFunc, list), cancellationToken);

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

        private static TimeSpan RunFind(Func<List<int>, int> findFunc, List<int> listToSort)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var item = findFunc(listToSort);

            stopWatch.Stop();

            return stopWatch.Elapsed;
        }

        private int LinqFind(List<int> list)
        {
            var result = 0;
            for (var i = 0; i < 1000000; i++)
            {
                result = list.FirstOrDefault(x => x.Equals(1));
            }
            return result;
        }

        private int ForLoopFind(List<int> list)
        {
            var result = 0;
            for (var i = 0; i < 1000000; i++)
            {
                foreach (var item in list)
                {
                    if (item == 1)
                    {
                        result = item;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
