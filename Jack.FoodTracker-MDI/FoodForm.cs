using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jack.FoodTracker.Entities;
using Jack.FoodTracker.EntityDatabase;
using Jack.FoodTracker;


namespace Jack.FoodTracker_MDI
{
    public partial class FoodForm : Form
    {
        FoodTrackerService fTracker;
        IList<Food> foods;
        BindingList<Food> bfoods;
        CurrencyManager currencyManager;
        SearchService search;
        IList<FoodCategory> categories;
        public FoodForm( FoodTrackerService foodTracker)
        {
            InitializeComponent();
            fTracker = foodTracker;
            categories = fTracker.GetAllFoodCategories(false);
            cbCategory.DataSource = categories;
            cbCategory.DisplayMember = "Name";
            cbCategory.SelectedItem = null;
            titleBar1.lbRecords.Text = "food records.";
            search = new SearchService();
            bfoods = new BindingList<Food>();
            lblError.Text = "";
            BindData();
            
        }

        private void FoodForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {

                case 116:// F5 Key - Retrieve records
                    GetRecords();
                    BindList(foods);
                    currencyManager = (CurrencyManager)this.BindingContext[bfoods];
                    if (currencyManager.Count > 1)
                    {
                        scrollBar.Visible = true;
                        scrollBar.Maximum = currencyManager.Count - 1;
                        scrollBar.Minimum = 0;
                    }
                    titleBar1.lbCurrent.Text = (currencyManager.Position + 1).ToString();
                    titleBar1.lbTotal.Text = currencyManager.Count.ToString();
                    break;
                case 34: // Page Down Key-Next record
                    RecordDown();
                    break;
                case 33: // Page Up Key - Previous record
                    RecordUp();
                    break;
                case 123: // f12 key - clear screen
                    if (currencyManager != null && currencyManager.Count != 0)
                    {
                        bool changes = fTracker.CheckChanges();
                        DialogResult clear = new DialogResult(); ;
                        if (changes)
                        {
                             clear= MessageBox.Show("You have unsaved changes are you sure you want to clear ?", "Clear Screen?", MessageBoxButtons.YesNo);
                        }
                        if(!changes || clear == DialogResult.Yes )
                        {
                            if(clear == DialogResult.Yes)
                            {
                                fTracker.DiscardChanges();
                            }
                            ClearRecords();
                            scrollBar.Visible = false;
                            titleBar1.lbCurrent.Text = (currencyManager.Position + 1).ToString();
                            titleBar1.lbTotal.Text = currencyManager.Count.ToString();
                        }
                        
                    }
                    else
                    {
                        tbName.Text = "";
                        tbDesc.Text = "";
                        tbCalories.Text = "";
                        tbSugar.Text = "";
                        tbFat.Text = "";
                        tbSalt.Text = "";
                        tbSatFat.Text = "";
                        cbCategory.SelectedItem = null;
                    }
                    break;
                case 115:// f4 key quit form
                    if (fTracker.CheckChanges())
                    {
                        DialogResult quit = MessageBox.Show("Are you sure you want to quit you have unsaved changes Food ?", "Quit Food Form?", MessageBoxButtons.YesNo);
                        if (quit == DialogResult.Yes)
                        {
                            fTracker.DiscardChanges();
                            //store to store deleted food
                            this.Close();
                        }
                    }
                    else
                    {
                        this.Close();
                    }
                    
                    
                    break;
                case 117:// F6 key store
                    Store();
                    break;
                case 68: // ctrl+D - Delete Record
                    if (e.Control)
                    {
                        Food deleteFood = bfoods.ElementAt(currencyManager.Position);
                        DialogResult delete = MessageBox.Show("Are you sure you want to delete " + deleteFood.Name, "Delete Food?", MessageBoxButtons.YesNo);
                        if (delete == DialogResult.Yes && deleteFood != null)
                        {
                            fTracker.DeleteFood(deleteFood);
                            bfoods.Remove(deleteFood);
                            currencyManager.Refresh();
                            scrollBar.Maximum = scrollBar.Maximum - 1;
                            titleBar1.lbCurrent.Text = (currencyManager.Position + 1).ToString();
                            titleBar1.lbTotal.Text = currencyManager.Count.ToString();
                        }
                       
                    }
                    break;
                case 69:// ctrl + e - erase all retrived records
                    if (e.Control)
                    {
                        Food deleteFood = bfoods.ElementAt(currencyManager.Position);
                        DialogResult delete = MessageBox.Show("Are you sure you want to erase all " + currencyManager.Count.ToString() + "Foods", "Erase Food?", MessageBoxButtons.YesNo);
                        if (delete == DialogResult.Yes && deleteFood != null)
                        {
                            
                            foreach (Food item in bfoods)
                            {
                                fTracker.DeleteFood(item);
                            }
                            ClearRecords();
                        }

                    }
                    break;
                case 78: // ctrl + N add new record
                    if(e.Control)
                    {
                        Food newFood = new Food();
                        fTracker.AddBlankFood(newFood);
                        newFood.Category = categories[0];
                        BindList(newFood);
                        currencyManager = (CurrencyManager)this.BindingContext[bfoods];
                        if (currencyManager.Count > 1)
                        {
                            scrollBar.Visible = true;
                            scrollBar.Maximum = currencyManager.Count - 1;
                            scrollBar.Minimum = 0;
                        }
                        currencyManager.Position = currencyManager.Count - 1;
                        titleBar1.lbCurrent.Text = (currencyManager.Position + 1).ToString();
                        titleBar1.lbTotal.Text = currencyManager.Count.ToString();
                    }
                    break;
            }
        }

        private void ClearRecords()
        {
            bfoods.RaiseListChangedEvents = false;
            bfoods.Clear();
            bfoods.RaiseListChangedEvents = true;
            cbCategory.SelectedItem = null;
            currencyManager.Refresh();
            titleBar1.lbCurrent.Text = (currencyManager.Position + 1).ToString();
            titleBar1.lbTotal.Text = currencyManager.Count.ToString();
        }

        private void Store()
        {
            
            if (currencyManager == null || currencyManager.Count == 0)
            {
                FoodDTO dto = new FoodDTO()
                {
                    Name = tbName.Text,
                    Category = (FoodCategory)cbCategory.SelectedItem,
                    Description = tbDesc.Text,
                    Calories = tbCalories.Text,
                    Sugar = tbSugar.Text,
                    Fat = tbFat.Text,
                    Saturates = tbSatFat.Text,
                    Salt = tbSalt.Text
                };
                try
                {
                    lblError.Text = "";
                    fTracker.AddFood(dto);
                    Food newFood = fTracker.GetFoodByName(dto.Name);
                    fTracker.Store();
                    BindList(newFood);
                    currencyManager = (CurrencyManager)this.BindingContext[bfoods];
                    titleBar1.lbCurrent.Text = (currencyManager.Position + 1).ToString();
                    titleBar1.lbTotal.Text = currencyManager.Count.ToString();
                }
                catch (ArgumentException exp)
                {
                    lblError.Text = exp.Message;
                    if (exp.Message == "The Category field is required.")
                    {
                        this.ActiveControl = cbCategory;
                    }
                    else
                    {
                        this.ActiveControl = tbName;
                    }
                }

            }
            else
            {
                IList<Food> changes = fTracker.GetChanges("both");
                try
                {
                foreach(Food item in changes)
                {
                    fTracker.EditFood(item);
                }
                fTracker.Store();
                }
                catch (ArgumentException exp)
                    {

                        lblError.Text = exp.Message;
                        if (exp.Message == "The Category field is required.")
                        {
                            this.ActiveControl = cbCategory;
                        }
                        else
                        {
                            this.ActiveControl = tbName;
                        }
                    }
                
            }
        }

        private void RecordUp()
        {
            if (currencyManager.Position != 0)
            {
                currencyManager.Position--;
                titleBar1.lbCurrent.Text = (currencyManager.Position + 1).ToString();
                scrollBar.Value = currencyManager.Position;
            }
        }

        private void RecordDown()
        {
            if (currencyManager.Position + 1 < currencyManager.Count)
            {
                currencyManager.Position++;
                titleBar1.lbCurrent.Text = (currencyManager.Position + 1).ToString();
                scrollBar.Value = currencyManager.Position;
            }
        }

        private void BindList(IList<Food> food)
        {
            bfoods.RaiseListChangedEvents = false;
            bfoods.Clear();
            foreach( Food item in food)
            {
                bfoods.Add(item);
            }
            bfoods.RaiseListChangedEvents = true;
            bfoods.ResetBindings();
        }
        private void BindList(Food food)
        {
            bfoods.RaiseListChangedEvents = false;
           // bfoods.Clear();
            bfoods.Add(food);
            bfoods.RaiseListChangedEvents = true;
            bfoods.ResetBindings();
        }

        private IList<Food> GetRecords()
        {
            double dValue;
            int    iValue;
            foods = fTracker.GetAllFood();
               if (tbName.Text != "")
               {
                   foods = search.ApplySearchEquals<Food>(foods, "Name", "=", tbName.Text, true);
               }
               if (tbDesc.Text != "")
               {
                   foods = search.ApplySearchEquals<Food>(foods, "Description", "=", tbDesc.Text, true);
               }
               if (cbCategory.SelectedItem != null)
               {
                   foods = search.ApplySearchEquals<Food>(foods, "Category", "=", cbCategory.SelectedItem);
               }
               if(tbCalories.Text != "")
               {
                   if (Int32.TryParse(tbCalories.Text,out iValue))
                   {
                       foods = search.ApplySearchNumerical<Food>(foods, "Calories", "=", iValue);
                   }
               }
               if (tbSugar.Text != "")
               {
                   if (Double.TryParse(tbSugar.Text, out dValue))
                   {
                       foods = search.ApplySearchNumerical<Food>(foods, "Sugars", "=", dValue);
                   }
               }
               if (tbFat.Text != "")
               {
                   if (Double.TryParse(tbFat.Text, out dValue))
                   {
                       foods = search.ApplySearchNumerical<Food>(foods, "Fat", "=", dValue);
                   }
               }
               if (tbSatFat.Text != "")
               {
                   if (Double.TryParse(tbSatFat.Text, out dValue))
                   {
                       foods = search.ApplySearchNumerical<Food>(foods, "Saturates", "=", dValue);
                   }
               }
               if (tbSalt.Text != "")
               {
                   if (Double.TryParse(tbSalt.Text, out dValue))
                   {
                       foods = search.ApplySearchNumerical<Food>(foods, "Salt", "=", dValue);
                   }
               }
               return foods;
        }

        private void BindData()
        {
            tbName.DataBindings.Add("Text", bfoods, "Name",false,DataSourceUpdateMode.OnPropertyChanged);
            tbDesc.DataBindings.Add("Text", bfoods, "Description",false, DataSourceUpdateMode.OnPropertyChanged);
            cbCategory.DataBindings.Add("SelectedItem", bfoods,"Category",false,DataSourceUpdateMode.OnPropertyChanged);
            tbCalories.DataBindings.Add("Text", bfoods, "Calories", false, DataSourceUpdateMode.OnPropertyChanged);
            tbSugar.DataBindings.Add("Text", bfoods, "Sugars", false, DataSourceUpdateMode.OnPropertyChanged);
            tbFat.DataBindings.Add("Text", bfoods, "Fat", false, DataSourceUpdateMode.OnPropertyChanged);
            tbSatFat.DataBindings.Add("Text", bfoods, "Saturates", false, DataSourceUpdateMode.OnPropertyChanged);
            tbSalt.DataBindings.Add("Text", bfoods, "Salt", false, DataSourceUpdateMode.OnPropertyChanged);
        }


        private void scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type != ScrollEventType.EndScroll && e.Type  != ScrollEventType.First && e.Type  != ScrollEventType.Last)
            {
                if (e.NewValue < e.OldValue)
                {
                    RecordUp();
                }
                else if (e.NewValue != 0)
                {
                    RecordDown();
                }
            }
            
        }
        
    }
}
