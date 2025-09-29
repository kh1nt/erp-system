using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using erp_system.Services;
using erp_system.model;

namespace erp_system.view_model
{
    public class Insights_Analytics_View_Model : View_Model_Base
    {
        public ObservableCollection<Status_Cards_Model> AnalyticsCards { get; set; }
        public Dictionary<string, decimal> SalesSummary { get; set; } = new Dictionary<string, decimal>();

        public Insights_Analytics_View_Model()
        {
            var data = new DataService();
            SalesSummary = data.GetSalesSummary();
            
            AnalyticsCards = new ObservableCollection<Status_Cards_Model>
            {
                new Status_Cards_Model { Title="Total Sales", Icon="💰", Value=(int)SalesSummary.GetValueOrDefault("TotalSales", 0), NavigationTarget="Sales_View" },
                new Status_Cards_Model { Title="Average Sale", Icon="📊", Value=(int)SalesSummary.GetValueOrDefault("AverageSale", 0), NavigationTarget="Sales_View" },
                new Status_Cards_Model { Title="Total Transactions", Icon="🛒", Value=(int)SalesSummary.GetValueOrDefault("TotalTransactions", 0), NavigationTarget="Sales_View" },
                new Status_Cards_Model { Title="Active Employees", Icon="👥", Value=data.GetCount("Employees"), NavigationTarget="Employees_View" }
            };
        }
    }
}
