using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jack.FoodTracker_MDI
{
    public partial class Menu : Form
    {
        private Form toRunForm;
        public FoodTrackerMDI MDI;
        public Menu()
        {
            InitializeComponent();
            ErrorLabel.Text = "";
        }


        private bool CheckifOpen(string name)
        {
            foreach (Form form in Application.OpenForms)
            {
                if(form.GetType().Name == name)
                {
                    form.WindowState = FormWindowState.Normal;
                    form.Focus();
                    return true;
                }
                
            }
            return false;
        }

        private void Menu_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyValue == 123 || e.KeyValue == 118)
            {
                tbOption.Text = "";
            }
            if(e.KeyValue == 115)
            {
                DialogResult quit = MessageBox.Show("Are you sure you want to quit this progam?", "Quit Foodtracker?", MessageBoxButtons.YesNo);
                if(quit == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        private void EnterPress(object sender, KeyEventArgs e)
        {
            if(tbOption.Text != "" && e.KeyValue == 13)
            {
                Run(null,null);
            }
        }

        private void Run(object sender, EventArgs e)
        {
            ErrorLabel.Text = "";
            toRunForm = null;
            if (tbOption.Text != "")
            {
                try
                {
                    switch (tbOption.Text.ToLower())
                    {
                        case "food":
                            if (!CheckifOpen("FoodForm"))
                            {
                                toRunForm = new FoodForm(MDI.fTracker);
                            }
                            break;
                        default:
                            throw new ArgumentException("No Such Option.");
                    }
                    if (toRunForm != null)
                    {
                        toRunForm.MdiParent = MDI;
                        toRunForm.Show();
                    }
                }
                catch (ArgumentException exp)
                {
                    ErrorLabel.Text = exp.Message;
                }
            }
        }
        
    }
}
