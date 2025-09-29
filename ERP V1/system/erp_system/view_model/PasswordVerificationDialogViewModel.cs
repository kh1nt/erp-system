using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using erp_system.Services;
using erp_system.view;

namespace erp_system.view_model
{
    public class PasswordVerificationDialogViewModel : View_Model_Base
    {
        private readonly string _actionDescription;
        private readonly string _currentUser;
        private readonly DataService _dataService;

        private string _password = string.Empty;
        
        public void SetPassword(string password)
        {
            _password = password;
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public string ActionDescription => _actionDescription;
        public string CurrentUser => _currentUser;
        public bool IsVerified { get; private set; }

        public PasswordVerificationDialogViewModel(string actionDescription, string currentUser)
        {
            _actionDescription = actionDescription;
            _currentUser = currentUser;
            _dataService = new DataService();
        }

        public System.Windows.Input.ICommand VerifyCommand => new View_Model_Command(_ => VerifyPassword());
        public System.Windows.Input.ICommand CancelCommand => new View_Model_Command(_ => Cancel());

        private void VerifyPassword()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_password))
                {
                    ShowError("Please enter your password.");
                    return;
                }

                // Get user from database
                var user = _dataService.GetUserByUsername(_currentUser);
                if (user == null)
                {
                    ShowError("User not found.");
                    return;
                }

                // Check if user has required role (Admin or HR Officer)
                if (user.RoleName != "Admin" && user.RoleName != "HR Officer")
                {
                    ShowError("You don't have permission to perform this action.");
                    return;
                }

                // Verify password
                var hashedPassword = HashPassword(_password);
                if (hashedPassword != user.PasswordHash)
                {
                    ShowError("Invalid password.");
                    return;
                }

                // Password verified successfully
                IsVerified = true;
                ClearError();
                
                // Close the dialog
                if (Application.Current.MainWindow is Window mainWindow)
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is PasswordVerificationDialog dialog && dialog.DataContext == this)
                        {
                            dialog.DialogResult = true;
                            dialog.Close();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error verifying password: {ex.Message}");
            }
        }

        private void Cancel()
        {
            IsVerified = false;
            if (Application.Current.MainWindow is Window mainWindow)
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is PasswordVerificationDialog dialog && dialog.DataContext == this)
                    {
                        dialog.DialogResult = false;
                        dialog.Close();
                        break;
                    }
                }
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        private void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
