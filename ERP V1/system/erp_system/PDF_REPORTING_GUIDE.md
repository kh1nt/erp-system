# PDF Reporting System Guide

## Overview
Your ERP system now includes a comprehensive PDF reporting system that allows you to generate professional PDF reports for various business data.

## Features

### Available Report Types
1. **Employee Directory** - Complete list of all employees with details
2. **Sales Summary** - Sales performance and revenue analysis
3. **Payroll Report** - Salary, commission and deduction details
4. **Performance Analysis** - Employee performance metrics and KPIs
5. **Attendance Report** - Employee attendance and leave summary
6. **Commission Report** - Sales consultant commission breakdown

### Key Features
- **Professional PDF Layout** - Clean, formatted reports with headers and tables
- **Date Range Filtering** - Optional date range selection for time-based reports
- **Summary Statistics** - Key metrics and totals at the top of each report
- **Detailed Data Tables** - Comprehensive data in well-formatted tables
- **Automatic File Naming** - Reports are saved with descriptive names including dates
- **Desktop Integration** - Reports are automatically saved to your Desktop

## How to Use

### 1. Access Reports
- Navigate to the **Insights & Analytics** section in your ERP system
- The report generation interface will be displayed on the right side

### 2. Generate a Report
1. **Select Report Type**: Choose from the dropdown menu
2. **Set Date Range** (Optional): Use the date pickers to filter data
3. **Click "Generate PDF Report"**: The system will create and save the PDF
4. **View Results**: A success message will show the file location

### 3. Report Structure
Each PDF report includes:
- **Header**: Company name, report title, and generation date
- **Summary Section**: Key statistics and totals
- **Detailed Data**: Comprehensive tables with all relevant information
- **Footer**: Professional closing

## Technical Implementation

### Dependencies Added
- `itext7` (v8.0.2) - Professional PDF generation library
- `System.Drawing.Common` (v8.0.0) - Graphics support

### Files Created/Modified
- `model/Report_Models.cs` - Data models for different report types
- `Services/PDFReportService.cs` - Core PDF generation service
- `view_model/Insights_Analytics_View_Model.cs` - Updated with report functionality
- `view/Insights_Analytics_View.xaml` - Updated UI with report controls
- `view/Converters.cs` - Added report-specific converters

### Report Generation Process
1. User selects report type and optional date range
2. System queries database for relevant data
3. Data is formatted into report structure
4. PDF is generated using iText7 library
5. File is saved to Desktop with descriptive name
6. User receives confirmation with file path

## Customization Options

### Adding New Report Types
1. Add new enum value to `ReportType` in `Report_Models.cs`
2. Implement data retrieval logic in `PDFReportService.GetReportData()`
3. Add table generation method in `PDFReportService`
4. Update the converter for report descriptions

### Modifying Report Layout
- Edit the PDF generation methods in `PDFReportService.cs`
- Customize headers, tables, and styling
- Add company logos or branding elements

### Changing File Location
- Modify the `GenerateFileName()` method in `PDFReportService.cs`
- Update the file path in `GenerateReport()` method

## Example Usage

```csharp
// Generate an employee directory report
var pdfService = new PDFReportService();
var filePath = pdfService.GenerateReport(ReportType.EmployeeDirectory);

// Generate a sales report for a specific date range
var startDate = new DateTime(2024, 1, 1);
var endDate = new DateTime(2024, 12, 31);
var salesReportPath = pdfService.GenerateReport(ReportType.SalesSummary, startDate, endDate);
```

## Troubleshooting

### Common Issues
1. **PDF Generation Fails**: Check database connectivity and data availability
2. **File Not Found**: Verify Desktop access permissions
3. **Empty Reports**: Ensure database contains data for selected date range
4. **Formatting Issues**: Check iText7 library installation

### Performance Tips
- Use date ranges to limit data volume for large datasets
- Reports are generated asynchronously to prevent UI blocking
- Large reports may take longer to generate

## Future Enhancements
- Report scheduling and automation
- Email integration for report distribution
- Custom report templates
- Interactive report preview
- Export to other formats (Excel, CSV)
- Report caching and history

---

*This PDF reporting system provides a solid foundation for business intelligence and data analysis within your ERP system.*
