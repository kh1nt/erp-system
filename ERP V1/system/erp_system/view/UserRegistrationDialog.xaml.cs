using System;
using System.Windows;

namespace erp_system
{
    public partial class UserRegistrationDialog : Window
    {
        public UserRegistrationDialog()
        {
            InitializeComponent();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
