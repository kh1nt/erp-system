﻿using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Windows.Input;
using erp_system.model;
using erp_system.repositories;


namespace erp_system.view_model
{
    public class Login_View_Model : View_Model_Base
    {
        //Fields
        private string _username = string.Empty;
        private SecureString? _password;
        private string _error_message = string.Empty;
        private bool _is_view_visible = true;

        private readonly IUser_Repository User_Repositroy;

        //Properties
        public string Username
        {
            get => _username;

            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public SecureString? Password
        {
            get => _password;

            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string Error_Message
        {
            get => _error_message;

            set
            {
                _error_message = value;
                OnPropertyChanged(nameof(Error_Message));
            }
        }
        public bool Is_View_Visible
        {
            get => _is_view_visible;

            set
            {
                _is_view_visible = value;
                OnPropertyChanged(nameof(Is_View_Visible));
            }
        }

        // -> Commands
        public ICommand Login_Command { get; }
        public ICommand Logout_Command { get; } = new View_Model_Command(_ => { });
        public ICommand Recover_Password_Command { get; } = new View_Model_Command(_ => { });
        public ICommand Show_Password_Command { get; } = new View_Model_Command(_ => { });
        public ICommand Remember_Password_Command { get; } = new View_Model_Command(_ => { });

        //Constructors
        public Login_View_Model()
        {
            User_Repositroy = new User_Repository();
            Login_Command = new View_Model_Command(Execute_Login_Command, Can_Execute_Login_Command);
            Recover_Password_Command = new View_Model_Command(p => Execute_Recovery_Password("",""));
        }


        private bool Can_Execute_Login_Command(object? obj)
        {
            bool valid_data;
            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3 || Password == null || Password.Length < 3)
                valid_data = false;
            else
                valid_data = true;
            return valid_data;

        }

        private void Execute_Login_Command(object? obj)
        {
            try
            {
                var Is_Valid_User = User_Repositroy.Authenticate_User(new NetworkCredential(Username, Password));
                if (Is_Valid_User)
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(Username), null);
                    Is_View_Visible = false;
                    Error_Message = string.Empty; // Clear any previous errors
                }
                else
                {
                    Error_Message = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                Error_Message = "Database connection error. Please check your connection.";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }
        }

        private void Execute_Recovery_Password(string username, string email)
        {
            throw new NotImplementedException();
        }
    }
}
