using System.Windows;

namespace erp_system.view
{
    public partial class StatusUpdateDialog : Window
    {
        public StatusUpdateDialog()
        {
            InitializeComponent();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
