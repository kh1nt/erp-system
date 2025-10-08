using System;
using System.Windows;
using erp_system.repositories;

namespace erp_system
{
    public partial class PasswordChangeDialog : Window
    {
        public string NewPassword { get; private set; } = string.Empty;
        private readonly User_Repository _userRepository;
        private readonly int _currentUserId;

        public PasswordChangeDialog(int userId)
        {
            InitializeComponent();
            _userRepository = new User_Repository();
            _currentUserId = userId;
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== SAVE BUTTON CLICKED ===");
                System.Diagnostics.Debug.WriteLine($"UserID: {_currentUserId}");
                
                
                // Clear previous error message
                ErrorMessage.Visibility = Visibility.Collapsed;
                ErrorMessage.Text = string.Empty;

                // Validate current password - get from whichever control has content
                var currentPassword = string.Empty;
                if (CurrentPasswordBox.Visibility == Visibility.Visible && !string.IsNullOrEmpty(CurrentPasswordBox.Password))
                {
                    currentPassword = CurrentPasswordBox.Password;
                }
                else if (CurrentPasswordTextBox.Visibility == Visibility.Visible && !string.IsNullOrEmpty(CurrentPasswordTextBox.Text))
                {
                    currentPassword = CurrentPasswordTextBox.Text;
                }
                else
                {
                    // Try both controls regardless of visibility
                    currentPassword = !string.IsNullOrEmpty(CurrentPasswordBox.Password) ? 
                        CurrentPasswordBox.Password : CurrentPasswordTextBox.Text;
                }
                    
                System.Diagnostics.Debug.WriteLine($"Current password length: {currentPassword?.Length ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Current password box visible: {CurrentPasswordBox.Visibility}");
                System.Diagnostics.Debug.WriteLine($"Current password text box visible: {CurrentPasswordTextBox.Visibility}");
                System.Diagnostics.Debug.WriteLine($"Current password box content: '{CurrentPasswordBox.Password}'");
                System.Diagnostics.Debug.WriteLine($"Current password text box content: '{CurrentPasswordTextBox.Text}'");
                System.Diagnostics.Debug.WriteLine($"Current password final value: '{currentPassword}'");
                    
                if (string.IsNullOrWhiteSpace(currentPassword))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Current password is empty");
                    ShowError("Current password is required!");
                    return;
                }

                // Verify current password
                System.Diagnostics.Debug.WriteLine("Getting user from database...");
                var currentUser = _userRepository.GetbyId(_currentUserId);
                if (currentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: User not found in database");
                    ShowError("User not found!");
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"User found: {currentUser.Username}");
                System.Diagnostics.Debug.WriteLine("Verifying current password...");
                var isPasswordValid = _userRepository.VerifyPassword(currentUser.Username, currentPassword);
                System.Diagnostics.Debug.WriteLine($"Password verification result: {isPasswordValid}");
                
                if (!isPasswordValid)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Current password is incorrect");
                    ShowError("Current password is incorrect!");
                    return;
                }

                // Validate new password - get from whichever control has content
                var newPassword = string.Empty;
                if (NewPasswordBox.Visibility == Visibility.Visible && !string.IsNullOrEmpty(NewPasswordBox.Password))
                {
                    newPassword = NewPasswordBox.Password;
                }
                else if (NewPasswordTextBox.Visibility == Visibility.Visible && !string.IsNullOrEmpty(NewPasswordTextBox.Text))
                {
                    newPassword = NewPasswordTextBox.Text;
                }
                else
                {
                    // Try both controls regardless of visibility
                    newPassword = !string.IsNullOrEmpty(NewPasswordBox.Password) ? 
                        NewPasswordBox.Password : NewPasswordTextBox.Text;
                }
                
                var confirmPassword = string.Empty;
                if (ConfirmPasswordBox.Visibility == Visibility.Visible && !string.IsNullOrEmpty(ConfirmPasswordBox.Password))
                {
                    confirmPassword = ConfirmPasswordBox.Password;
                }
                else if (ConfirmPasswordTextBox.Visibility == Visibility.Visible && !string.IsNullOrEmpty(ConfirmPasswordTextBox.Text))
                {
                    confirmPassword = ConfirmPasswordTextBox.Text;
                }
                else
                {
                    // Try both controls regardless of visibility
                    confirmPassword = !string.IsNullOrEmpty(ConfirmPasswordBox.Password) ? 
                        ConfirmPasswordBox.Password : ConfirmPasswordTextBox.Text;
                }
                    
                System.Diagnostics.Debug.WriteLine($"New password length: {newPassword?.Length ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Confirm password length: {confirmPassword?.Length ?? 0}");
                System.Diagnostics.Debug.WriteLine($"New password box visible: {NewPasswordBox.Visibility}");
                System.Diagnostics.Debug.WriteLine($"Confirm password box visible: {ConfirmPasswordBox.Visibility}");
                System.Diagnostics.Debug.WriteLine($"New password box content: '{NewPasswordBox.Password}'");
                System.Diagnostics.Debug.WriteLine($"New password text box content: '{NewPasswordTextBox.Text}'");
                System.Diagnostics.Debug.WriteLine($"Confirm password box content: '{ConfirmPasswordBox.Password}'");
                System.Diagnostics.Debug.WriteLine($"Confirm password text box content: '{ConfirmPasswordTextBox.Text}'");
                System.Diagnostics.Debug.WriteLine($"New password final value: '{newPassword}'");
                System.Diagnostics.Debug.WriteLine($"Confirm password final value: '{confirmPassword}'");
                    
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: New password is empty");
                    ShowError("New password cannot be empty!");
                    return;
                }

                if (newPassword.Length < 8)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: New password too short ({newPassword.Length} chars)");
                    ShowError("New password must be at least 8 characters long!");
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: New password and confirmation do not match");
                    ShowError("New password and confirmation do not match!");
                    return;
                }

                // Check if new password is different from current password
                if (currentPassword == newPassword)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: New password is same as current password");
                    ShowError("New password must be different from current password!");
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine("All validations passed!");

                // Set the new password and close dialog
                System.Diagnostics.Debug.WriteLine("Setting new password and closing dialog...");
                NewPassword = newPassword;
                DialogResult = true;
                System.Diagnostics.Debug.WriteLine("Dialog closed successfully");
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowError($"Error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            System.Diagnostics.Debug.WriteLine($"SHOWING ERROR: {message}");
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
            System.Diagnostics.Debug.WriteLine($"Error message visibility: {ErrorMessage.Visibility}");
            System.Diagnostics.Debug.WriteLine($"Error message text: '{ErrorMessage.Text}'");
        }

        private void CurrentPasswordToggle_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPasswordBox.Visibility == Visibility.Visible)
            {
                CurrentPasswordTextBox.Text = CurrentPasswordBox.Password;
                CurrentPasswordBox.Visibility = Visibility.Collapsed;
                CurrentPasswordTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                CurrentPasswordBox.Password = CurrentPasswordTextBox.Text;
                CurrentPasswordTextBox.Visibility = Visibility.Collapsed;
                CurrentPasswordBox.Visibility = Visibility.Visible;
            }
        }

        private void NewPasswordToggle_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox.Visibility == Visibility.Visible)
            {
                NewPasswordTextBox.Text = NewPasswordBox.Password;
                NewPasswordBox.Visibility = Visibility.Collapsed;
                NewPasswordTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                NewPasswordBox.Password = NewPasswordTextBox.Text;
                NewPasswordTextBox.Visibility = Visibility.Collapsed;
                NewPasswordBox.Visibility = Visibility.Visible;
            }
        }

        private void ConfirmPasswordToggle_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmPasswordBox.Visibility == Visibility.Visible)
            {
                ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
                ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;
                ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordBox.Visibility = Visibility.Visible;
            }
        }
    }
}
