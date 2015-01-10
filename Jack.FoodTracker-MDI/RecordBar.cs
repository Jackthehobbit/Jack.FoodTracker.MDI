using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jack.FoodTracker_MDI
{
    public partial class RecordBar : UserControl
    {
        public System.Windows.Forms.Label lbCurrent;
        public System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.Label of;
        public System.Windows.Forms.Label lbRecords;
        public RecordBar()
        {
            InitializeComponent();
        }
    }
}
