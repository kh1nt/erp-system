using System.Windows;
using System.Windows.Input;
using erp_system.view_model;

namespace erp_system.view
{
    public partial class PasswordVerificationDialog : Window
    {
        public PasswordVerificationDialog(string actionDescription, string currentUser)
        {
            InitializeComponent();
            DataContext = new PasswordVerificationDialogViewModel(actionDescription, currentUser);
            
            // Focus on password box when dialog opens
            Loaded += (s, e) => 
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    PasswordBox.Focus();
                    PasswordBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Input);
            };
            
            // Also try to focus when the window becomes active
            Activated += (s, e) => 
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    PasswordBox.Focus();
                    PasswordBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Input);
            };
            
            // Try one more time when content is rendered
            ContentRendered += (s, e) => 
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    PasswordBox.Focus();
                    PasswordBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Input);
            };
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                VerifyPassword();
            }
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            VerifyPassword();
        }

        private void VerifyPassword()
        {
            if (DataContext is PasswordVerificationDialogViewModel viewModel)
            {
                // Set the password from the PasswordBox
                viewModel.SetPassword(PasswordBox.Password);
                viewModel.VerifyCommand.Execute(null);
            }
        }

        public bool IsVerified => (DataContext as PasswordVerificationDialogViewModel)?.IsVerified ?? false;
    }
}
