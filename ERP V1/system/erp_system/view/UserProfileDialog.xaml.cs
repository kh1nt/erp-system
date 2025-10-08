using System.Windows;
using erp_system.view_model;

namespace erp_system
{
    /// <summary>
    /// Interaction logic for UserProfileDialog.xaml
    /// </summary>
    public partial class UserProfileDialog : Window
    {
        public UserProfileDialog()
        {
            InitializeComponent();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
