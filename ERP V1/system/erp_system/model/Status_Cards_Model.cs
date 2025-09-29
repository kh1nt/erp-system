using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp_system.model
{
    public class Status_Cards_Model
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty; // could be FontAwesome/Glyph or image path
        public int Value { get; set; }
        public string Description { get; set; } = string.Empty; // optional (e.g., "5 chemicals below threshold")
        public string NavigationTarget { get; set; } = string.Empty; // name of module/page to navigate
    }
}
