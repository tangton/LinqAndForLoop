using LinqAndForLoop.Library.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace LinqAndForLoop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int ListSize = 500;

        private IGenerateAccountsService _generateAccountsService = new GenerateAccountsService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            btnRun.IsEnabled = false;

            var accountsList = _generateAccountsService.GenerateAccounts(ListSize);

            var taskList = new List<Task>();

            var taskLinq = ucLinq.Run(new LinqSearchService(), accountsList);
            var taskForLoop = ucForLoop.Run(new ForLoopSearchService(), accountsList);

            taskList.Add(taskLinq);
            taskList.Add(taskForLoop);

            await Task.WhenAll(taskList);

            btnRun.IsEnabled = true;
        }
    }
}
