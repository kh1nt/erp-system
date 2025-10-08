using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace erp_system.custom_controls
{
    /// <summary>
    /// Interaction logic for Bindable_Password_Box.xaml
    /// </summary>
    public partial class Bindable_Password_Box : UserControl
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(Bindable_Password_Box), 
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        private bool _isUpdatingPassword = false;

        public Bindable_Password_Box()
        {
            InitializeComponent();
            password_text.PasswordChanged += on_password_changed;
            password_textbox.TextChanged += on_text_changed;
        }

        private void on_password_changed(object sender, RoutedEventArgs e)
        {
            if (!_isUpdatingPassword)
            {
                _isUpdatingPassword = true;
                Password = password_text.Password;
                _isUpdatingPassword = false;
            }
        }

        private void on_text_changed(object sender, TextChangedEventArgs e)
        {
            if (!_isUpdatingPassword)
            {
                _isUpdatingPassword = true;
                Password = password_textbox.Text;
                _isUpdatingPassword = false;
            }
        }

        private void PasswordToggle_Click(object sender, RoutedEventArgs e)
        {
            if (password_text.Visibility == Visibility.Visible)
            {
                // Show password as plain text
                password_textbox.Text = password_text.Password;
                password_text.Visibility = Visibility.Collapsed;
                password_textbox.Visibility = Visibility.Visible;
                password_textbox.Focus();
                password_textbox.CaretIndex = password_textbox.Text.Length;
            }
            else
            {
                // Hide password
                password_text.Password = password_textbox.Text;
                password_textbox.Visibility = Visibility.Collapsed;
                password_text.Visibility = Visibility.Visible;
                password_text.Focus();
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            if (e.Property == PasswordProperty && !_isUpdatingPassword)
            {
                _isUpdatingPassword = true;
                password_text.Password = Password ?? string.Empty;
                password_textbox.Text = Password ?? string.Empty;
                _isUpdatingPassword = false;
            }
        }
    }
}
