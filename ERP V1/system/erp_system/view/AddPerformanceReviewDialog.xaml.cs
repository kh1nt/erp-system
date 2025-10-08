using System;
using System.Windows;
using erp_system.view_model;

namespace erp_system.view
{
    public partial class AddPerformanceReviewDialog : Window
    {
        private readonly Performance_View_Model _viewModel;

        public AddPerformanceReviewDialog(Performance_View_Model viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = new AddPerformanceReviewViewModel();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as AddPerformanceReviewViewModel;
            if (vm != null && vm.SavePerformanceReview())
            {
                _viewModel.LoadData(); // Refresh the performance data
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
