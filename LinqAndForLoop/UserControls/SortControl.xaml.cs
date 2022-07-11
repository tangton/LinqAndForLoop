using LinqAndForLoop.Library.Models;
using LinqAndForLoop.Library.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LinqAndForLoop.UserControls
{
    /// <summary>
    /// Interaction logic for SortControl.xaml
    /// </summary>
    public partial class SortControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SortControl));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public SortControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public async Task Run(ISearchService searchService, IEnumerable<Account> list)
        {
            lblResult.Content = "Pending...";
            pbStatus.Value = 0;
            pbStatus.Maximum = list.Count();

            pbStatus.IsIndeterminate = true;
            var task = Task.Factory.StartNew(() =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                for (var i = 0; i < 1000000; i++)
                {
                    searchService.GetFirstMatch(list, "Name", 10100);
                }

                stopWatch.Stop();

                return stopWatch.Elapsed;
            });

            var failed = false;
            var message = string.Empty;

            var durationOfSort = new TimeSpan();

            try
            {
                await task;

                durationOfSort = task.Result;
            }
            catch (Exception ex)
            {
                failed = true;
                message = ex.Message;
            }

            if (!failed)
            {
                pbStatus.IsIndeterminate = false;
                pbStatus.Value = list.Count();
                lblResult.Content = "Done. Time taken: " + durationOfSort;
            }
            else
            {
                lblResult.Content = "Sort failed. Message: " + message;
            }
        }
    }
}
