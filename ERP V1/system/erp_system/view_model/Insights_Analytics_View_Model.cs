using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;
using erp_system.Services;
using erp_system.model;

namespace erp_system.view_model
{
    public class Insights_Analytics_View_Model : View_Model_Base
    {
        private readonly PDFReportService _pdfReportService;
        private ReportType _selectedReportType = ReportType.EmployeeDirectory;
        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;
        private bool _isGeneratingReport = false;
        private string _previewFilePath = string.Empty;

        public ObservableCollection<Status_Cards_Model> AnalyticsCards { get; set; }
        public Dictionary<string, decimal> SalesSummary { get; set; } = new Dictionary<string, decimal>();
        public ObservableCollection<ReportType> AvailableReportTypes { get; set; }

        public ReportType SelectedReportType
        {
            get => _selectedReportType;
            set
            {
                _selectedReportType = value;
                OnPropertyChanged(nameof(SelectedReportType));
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public bool IsGeneratingReport
        {
            get => _isGeneratingReport;
            set
            {
                _isGeneratingReport = value;
                OnPropertyChanged(nameof(IsGeneratingReport));
                OnPropertyChanged(nameof(CanGenerateReport));
            }
        }

        public bool CanGenerateReport => !IsGeneratingReport;

        public string PreviewFilePath
        {
            get => _previewFilePath;
            set
            {
                _previewFilePath = value;
                OnPropertyChanged(nameof(PreviewFilePath));
                OnPropertyChanged(nameof(HasPreview));
            }
        }

        public bool HasPreview => !string.IsNullOrEmpty(_previewFilePath) && File.Exists(_previewFilePath);

        public ICommand GenerateReportCommand { get; }
        public ICommand PreviewReportCommand { get; }
        public ICommand SelectReportTypeCommand { get; }
        public ICommand OpenPreviewCommand { get; }

        public Insights_Analytics_View_Model()
        {
            _pdfReportService = new PDFReportService();
            
            var data = new DataService();
            SalesSummary = data.GetSalesSummary();
            
            AnalyticsCards = new ObservableCollection<Status_Cards_Model>
            {
                new Status_Cards_Model { Title="Total Sales", Icon="💰", Value=(int)SalesSummary.GetValueOrDefault("TotalSales", 0), NavigationTarget="Sales_View" },
                new Status_Cards_Model { Title="Average Sale", Icon="📊", Value=(int)SalesSummary.GetValueOrDefault("AverageSale", 0), NavigationTarget="Sales_View" },
                new Status_Cards_Model { Title="Total Transactions", Icon="🛒", Value=(int)SalesSummary.GetValueOrDefault("TotalTransactions", 0), NavigationTarget="Sales_View" },
                new Status_Cards_Model { Title="Active Employees", Icon="👥", Value=data.GetCount("Employees"), NavigationTarget="Employees_View" }
            };

            AvailableReportTypes = new ObservableCollection<ReportType>
            {
                ReportType.EmployeeDirectory,
                ReportType.SalesSummary,
                ReportType.PayrollReport,
                ReportType.PerformanceAnalysis,
                ReportType.AttendanceReport,
                ReportType.CommissionReport
            };

            GenerateReportCommand = new View_Model_Command(_ => GenerateReport());
            PreviewReportCommand = new View_Model_Command(_ => PreviewReport());
            SelectReportTypeCommand = new View_Model_Command(SelectReportType);
            OpenPreviewCommand = new View_Model_Command(_ => OpenPreview());
        }

        private async void GenerateReport()
        {
            if (IsGeneratingReport) return;

            try
            {
                IsGeneratingReport = true;

                await Task.Run(() =>
                {
                    var filePath = _pdfReportService.GenerateReport(SelectedReportType, StartDate, EndDate);
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            MessageBox.Show($"Report generated successfully!\n\nSaved to: {filePath}", 
                                "Report Generated", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Information);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
            finally
            {
                IsGeneratingReport = false;
            }
        }

        private async void PreviewReport()
        {
            if (IsGeneratingReport) return;

            try
            {
                IsGeneratingReport = true;

                await Task.Run(() =>
                {
                    // Generate a temporary PDF for preview
                    var tempPath = Path.Combine(Path.GetTempPath(), $"Preview_{SelectedReportType}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                    var filePath = _pdfReportService.GenerateReport(SelectedReportType, StartDate, EndDate, tempPath);
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            PreviewFilePath = filePath;
                            MessageBox.Show($"Preview generated successfully!\n\nPreview file: {Path.GetFileName(filePath)}", 
                                "Preview Generated", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Information);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating preview: {ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
            finally
            {
                IsGeneratingReport = false;
            }
        }

        private void SelectReportType(object? parameter)
        {
            if (parameter is ReportType reportType)
            {
                SelectedReportType = reportType;
                // Clear previous preview when changing report type
                PreviewFilePath = string.Empty;
            }
        }

        private void OpenPreview()
        {
            if (HasPreview)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = PreviewFilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening preview: {ex.Message}", 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            }
        }
    }
}
