using System;
using System.Collections.Generic;
using System.Linq;
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

namespace erp_system.view
{
    /// <summary>
    /// Interaction logic for Performance_View.xaml
    /// </summary>
    public partial class Performance_View : UserControl
    {
        public Performance_View()
        {
            InitializeComponent();
        }

        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var columnName = GetColumnNameFromHeader(element);
                if (DataContext is view_model.Performance_View_Model viewModel)
                {
                    viewModel.SortData(columnName);
                }
            }
        }

        private string GetColumnNameFromHeader(FrameworkElement header)
        {
            // Try to get the column name from the header content
            if (header is ContentControl contentControl)
            {
                var content = contentControl.Content?.ToString();
                return content switch
                {
                    "ID" => "RecordID",
                    "Employee" => "EmployeeName",
                    "Position" => "Position",
                    "Department" => "Department",
                    "Review Date" => "ReviewDate",
                    "Score" => "Score",
                    "Remarks" => "Remarks",
                    _ => "ReviewDate"
                };
            }
            return "ReviewDate";
        }
    }
}
