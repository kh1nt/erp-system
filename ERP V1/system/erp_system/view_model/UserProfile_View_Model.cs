using erp_system.model;
using erp_system.repositories;
using erp_system.view;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace erp_system.view_model
{
    public class UserProfile_View_Model : View_Model_Base
    {
        private User_Account_Model _currentUser;
        private User_Repository _userRepository;
        
        // Editing states
        private bool _isEditingName = false;
        private bool _isEditingUsername = false;
        private string _tempEmployeeName = string.Empty;
        private string _tempUsername = string.Empty;

        public User_Account_Model CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        // Editing properties
        public bool IsEditingName
        {
            get => _isEditingName;
            set
            {
                _isEditingName = value;
                OnPropertyChanged(nameof(IsEditingName));
                OnPropertyChanged(nameof(IsNotEditingName));
            }
        }

        public bool IsNotEditingName => !_isEditingName;

        public bool IsEditingUsername
        {
            get => _isEditingUsername;
            set
            {
                _isEditingUsername = value;
                OnPropertyChanged(nameof(IsEditingUsername));
                OnPropertyChanged(nameof(IsNotEditingUsername));
            }
        }

        public bool IsNotEditingUsername => !_isEditingUsername;

        public string TempEmployeeName
        {
            get => _tempEmployeeName;
            set
            {
                _tempEmployeeName = value;
                OnPropertyChanged(nameof(TempEmployeeName));
            }
        }

        public string TempUsername
        {
            get => _tempUsername;
            set
            {
                _tempUsername = value;
                OnPropertyChanged(nameof(TempUsername));
            }
        }

        // Commands
        public ICommand LogoutCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand ViewProfileCommand { get; }
        public ICommand EditNameCommand { get; }
        public ICommand SaveNameCommand { get; }
        public ICommand CancelNameEditCommand { get; }
        public ICommand EditUsernameCommand { get; }
        public ICommand SaveUsernameCommand { get; }
        public ICommand CancelUsernameEditCommand { get; }

        public UserProfile_View_Model()
        {
            _currentUser = new User_Account_Model();
            _userRepository = new User_Repository();
            
            // Initialize commands
            LogoutCommand = new View_Model_Command(Execute_LogoutCommand);
            ChangePasswordCommand = new View_Model_Command(Execute_ChangePasswordCommand);
            ViewProfileCommand = new View_Model_Command(Execute_ViewProfileCommand);
            
            // Name editing commands
            EditNameCommand = new View_Model_Command(Execute_EditNameCommand);
            SaveNameCommand = new View_Model_Command(Execute_SaveNameCommand);
            CancelNameEditCommand = new View_Model_Command(Execute_CancelNameEditCommand);
            
            // Username editing commands
            EditUsernameCommand = new View_Model_Command(Execute_EditUsernameCommand);
            SaveUsernameCommand = new View_Model_Command(Execute_SaveUsernameCommand);
            CancelUsernameEditCommand = new View_Model_Command(Execute_CancelUsernameEditCommand);
        }

        public UserProfile_View_Model(User_Account_Model user) : this()
        {
            CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
        }

        private void Execute_LogoutCommand(object? obj)
        {
            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Confirm Logout",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Clear the current user session
                    Thread.CurrentPrincipal = null;
                    
                    // Close all current windows
                    var windowsToClose = new List<Window>();
                    foreach (Window window in Application.Current.Windows)
                    {
                        windowsToClose.Add(window);
                    }
                    
                    // Close all windows
                    foreach (var window in windowsToClose)
                    {
                        try
                        {
                            window.Close();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error closing window: {ex.Message}");
                        }
                    }
                    
                    // Start a new session by creating a new login view
                    // This simulates restarting the application flow
                    var newLoginView = new Login_View();
                    newLoginView.Show();
                    Application.Current.MainWindow = newLoginView;
                    
                    // Set up the same event handler as in App.xaml.cs for new session
                    bool isHandlingClose = false;
                    newLoginView.IsVisibleChanged += (s, ev) =>
                    {
                        if (newLoginView.IsVisible == false && newLoginView.IsLoaded && !isHandlingClose)
                        {
                            isHandlingClose = true;
                            try
                            {
                                var mainWindow = new MainWindow();
                                mainWindow.Show();
                                Application.Current.MainWindow = mainWindow;
                                
                                // Only close if the window is not already closing
                                try
                                {
                                    newLoginView.Close();
                                }
                                catch (InvalidOperationException)
                                {
                                    // Window is already closing, ignore the error
                                }
                            }
                            finally
                            {
                                isHandlingClose = false;
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Execute_ChangePasswordCommand(object? obj)
        {
            try
            {
                var passwordDialog = new PasswordChangeDialog(CurrentUser.UserID);
                var result = passwordDialog.ShowDialog();
                
                if (result == true && !string.IsNullOrEmpty(passwordDialog.NewPassword))
                {
                    _userRepository.UpdatePassword(CurrentUser.UserID, passwordDialog.NewPassword);
                    MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Execute_ViewProfileCommand(object? obj)
        {
            try
            {
                MessageBox.Show(
                    $"Full Profile View for {CurrentUser.EmployeeName}\n\n" +
                    $"This feature will show detailed employee information,\n" +
                    $"including personal details, contact information,\n" +
                    $"and employment history.",
                    "Full Profile View",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Name editing methods
        private void Execute_EditNameCommand(object? obj)
        {
            TempEmployeeName = CurrentUser.EmployeeName;
            IsEditingName = true;
        }

        private void Execute_SaveNameCommand(object? obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TempEmployeeName))
                {
                    MessageBox.Show("Name cannot be empty!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Split the name into first and last name
                var nameParts = TempEmployeeName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts, 1, nameParts.Length - 1) : string.Empty;

                _userRepository.UpdateEmployeeName(CurrentUser.EmployeeID, firstName, lastName);
                CurrentUser.EmployeeName = TempEmployeeName;
                IsEditingName = false;
                
                MessageBox.Show("Name updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating name: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Execute_CancelNameEditCommand(object? obj)
        {
            TempEmployeeName = string.Empty;
            IsEditingName = false;
        }

        // Username editing methods
        private void Execute_EditUsernameCommand(object? obj)
        {
            TempUsername = CurrentUser.UserName;
            IsEditingUsername = true;
        }

        private void Execute_SaveUsernameCommand(object? obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TempUsername))
                {
                    MessageBox.Show("Username cannot be empty!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _userRepository.UpdateUsername(CurrentUser.UserID, TempUsername);
                CurrentUser.UserName = TempUsername;
                IsEditingUsername = false;
                
                MessageBox.Show("Username updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating username: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Execute_CancelUsernameEditCommand(object? obj)
        {
            TempUsername = string.Empty;
            IsEditingUsername = false;
        }
    }
}
