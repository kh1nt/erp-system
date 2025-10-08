using System.Windows;
using System;
using System.Threading.Tasks;
using erp_system.view;

namespace erp_system
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool _shownGlobalUiError;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Global exception handlers to avoid silent minimize/close
            this.DispatcherUnhandledException += (s, ev) =>
            {
                System.Diagnostics.Debug.WriteLine($"UI exception: {ev.Exception}");
                if (!_shownGlobalUiError)
                {
                    _shownGlobalUiError = true;
                    MessageBox.Show("An unexpected error occurred. The application will try to continue.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                ev.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
            {
                System.Diagnostics.Debug.WriteLine($"Domain exception: {ev.ExceptionObject}");
            };

            TaskScheduler.UnobservedTaskException += (s, ev) =>
            {
                System.Diagnostics.Debug.WriteLine($"Task exception: {ev.Exception}");
                ev.SetObserved();
            };

            // Uncomment the line below to test database connection first
            // var Test_View = new DatabaseTest_View();
            // Test_View.Show();
            
            // Start with Login View
            var Login_View = new Login_View();
            Login_View.Show();
            
            bool isHandlingClose = false;
            Login_View.IsVisibleChanged += (s, ev) =>
            {
                if (Login_View.IsVisible == false && Login_View.IsLoaded && !isHandlingClose)
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
                            Login_View.Close();
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

}
