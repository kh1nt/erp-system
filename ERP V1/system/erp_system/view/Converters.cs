using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using erp_system.model;

namespace erp_system.view
{
    public class FullNameConverter : IValueConverter
    {
        public static readonly FullNameConverter Instance = new FullNameConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is erp_system.model.Employee_Model employee)
            {
                return $"{employee.FirstName} {employee.LastName}";
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public static readonly StatusToColorConverter Instance = new StatusToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Pending" => new SolidColorBrush(Colors.Orange),
                    "Approved" => new SolidColorBrush(Colors.Green),
                    "Rejected" => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToVisibilityConverter : IValueConverter
    {
        public static readonly StatusToVisibilityConverter Instance = new StatusToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status && parameter is string targetStatus)
            {
                return status == targetStatus ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyToVisibilityConverter : IValueConverter
    {
        public static readonly EmptyToVisibilityConverter Instance = new EmptyToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return string.IsNullOrWhiteSpace(text) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public static readonly BoolToVisibilityConverter Instance = new BoolToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AmountToHeightConverter : IValueConverter
    {
        public static readonly AmountToHeightConverter Instance = new AmountToHeightConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                // If amount is 0 or very small, show a minimum height
                if (amount <= 0)
                    return 10.0;
                
                // Scale the amount to a reasonable height (10-120 pixels)
                var minHeight = 10.0;
                var maxHeight = 120.0;
                
                // Use logarithmic scaling for better visualization of large amounts
                // This prevents bars from being too elevated when amounts are very large
                var logAmount = Math.Log10((double)Math.Max(amount, 1));
                var maxLogAmount = Math.Log10(2000000.0); // Cap at 2M for scaling
                var ratio = Math.Min(logAmount / maxLogAmount, 1.0);
                var height = minHeight + (ratio * (maxHeight - minHeight));
                
                // Ensure minimum height for visibility
                return Math.Max(height, minHeight);
            }
            return 10.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CountToVisibilityConverter : IValueConverter
    {
        public static readonly CountToVisibilityConverter Instance = new CountToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToBoolConverter : IValueConverter
    {
        public static readonly StringToBoolConverter Instance = new StringToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string currentView && parameter is string targetView)
            {
                return currentView == targetView;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScoreToColorConverter : IValueConverter
    {
        public static readonly ScoreToColorConverter Instance = new ScoreToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal score)
            {
                if (score >= 4.0m)
                    return "High";
                else if (score >= 3.0m)
                    return "Good";
                else if (score >= 2.5m)
                    return "Average";
                else if (score >= 2.0m)
                    return "BelowAverage";
                else
                    return "Poor";
            }
            return "Average";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReportTypeToDescriptionConverter : IValueConverter
    {
        public static readonly ReportTypeToDescriptionConverter Instance = new ReportTypeToDescriptionConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReportType reportType)
            {
                return reportType switch
                {
                    ReportType.EmployeeDirectory => "Comprehensive employee directory including personal contact details, organizational information (department, position, manager), employment dates, current status, salary information, and recent performance ratings. Integrates data from Employee Management, Payroll, and Performance modules.",
                    ReportType.SalesSummary => "Detailed sales performance analysis covering total revenue, sales volume, transaction counts, product performance, customer analysis, and sales representative metrics. Includes sales trends, top performers, and commission calculations. Integrates data from Sales, Dashboard, and Employee Management modules.",
                    ReportType.PayrollReport => "Complete payroll summary showing base salaries, commissions, bonuses, deductions (taxes, benefits, loans), and net pay calculations. Includes payroll period details, payment status, and performance-based adjustments. Integrates data from Payroll, Employee Management, Sales, and Performance modules.",
                    ReportType.PerformanceAnalysis => "Comprehensive performance evaluation report including individual ratings, goal achievements, strengths, areas for improvement, review dates, and performance trends. Shows department comparisons and performance-based compensation. Integrates data from Performance, Employee Management, Dashboard, and Sales modules.",
                    ReportType.AttendanceReport => "Comprehensive leave management report covering leave requests by type, approval status, leave balances, and leave utilization patterns. Includes employee details, leave types, and approval information. Integrates data from Leave Management and Employee Management modules.",
                    ReportType.CommissionReport => "Sales commission analysis showing individual sales representative performance, commission calculations, sales targets vs. achievements, commission rates, and payment status. Includes sales transaction details and performance metrics. Integrates data from Sales, Payroll, Employee Management, and Performance modules.",
                    _ => "Custom report with selected data and date range."
                };
            }
            return "Select a report type to see its description.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public static readonly BooleanToVisibilityConverter Instance = new BooleanToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReportTypeToDisplayNameConverter : IValueConverter
    {
        public static readonly ReportTypeToDisplayNameConverter Instance = new ReportTypeToDisplayNameConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReportType reportType)
            {
                return reportType switch
                {
                    ReportType.EmployeeDirectory => "Employee Directory",
                    ReportType.SalesSummary => "Sales Summary",
                    ReportType.PayrollReport => "Payroll Report",
                    ReportType.PerformanceAnalysis => "Performance Analysis",
                    ReportType.AttendanceReport => "Attendance Report",
                    ReportType.CommissionReport => "Commission Report",
                    _ => "Custom Report"
                };
            }
            return "Select a report type";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AdminRoleToVisibilityConverter : IValueConverter
    {
        public static readonly AdminRoleToVisibilityConverter Instance = new AdminRoleToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string roleName)
            {
                return roleName?.ToLower() == "admin" ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class Converters
    {
        public static readonly FullNameConverter FullNameConverter = new FullNameConverter();
        public static readonly StatusToColorConverter StatusToColorConverter = new StatusToColorConverter();
        public static readonly StatusToVisibilityConverter StatusToVisibilityConverter = new StatusToVisibilityConverter();
        public static readonly EmptyToVisibilityConverter EmptyToVisibilityConverter = new EmptyToVisibilityConverter();
        public static readonly BoolToVisibilityConverter BoolToVisibilityConverter = new BoolToVisibilityConverter();
        public static readonly AmountToHeightConverter AmountToHeightConverter = new AmountToHeightConverter();
        public static readonly CountToVisibilityConverter CountToVisibilityConverter = new CountToVisibilityConverter();
        public static readonly StringToBoolConverter StringToBoolConverter = new StringToBoolConverter();
        public static readonly ScoreToColorConverter ScoreToColorConverter = new ScoreToColorConverter();
        public static readonly ReportTypeToDescriptionConverter ReportTypeToDescriptionConverter = new ReportTypeToDescriptionConverter();
        public static readonly BooleanToVisibilityConverter BooleanToVisibilityConverter = new BooleanToVisibilityConverter();
        public static readonly ReportTypeToDisplayNameConverter ReportTypeToDisplayNameConverter = new ReportTypeToDisplayNameConverter();
        public static readonly AdminRoleToVisibilityConverter AdminRoleToVisibilityConverter = new AdminRoleToVisibilityConverter();
    }
}
