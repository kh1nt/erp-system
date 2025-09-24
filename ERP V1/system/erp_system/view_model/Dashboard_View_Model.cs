using erp_system.model;
using System.Collections.ObjectModel;

namespace erp_system.view_model
{
    public class Dashboard_View_Model : View_Model_Base
    {
        public ObservableCollection<Status_Cards_Model> QuickStats { get; set; }

        public Dashboard_View_Model()
        {
            QuickStats = new ObservableCollection<Status_Cards_Model>
            {
                new Status_Cards_Model { Title="Total Chemicals in Stock", Icon="🔬", Value=120, NavigationTarget="Inventory_View" },
                new Status_Cards_Model { Title="Total Apparatus Available", Icon="⚗️", Value=80, NavigationTarget="Inventory_View" },
                new Status_Cards_Model { Title="Pending Borrower Slips", Icon="📄", Value=10, NavigationTarget="Borrower_View" }
            };
        }
    }
}
