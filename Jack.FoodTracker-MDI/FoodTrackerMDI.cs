using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jack.FoodTracker.EntityDatabase;
using Jack.FoodTracker.Entities;
using Jack.FoodTracker;

namespace Jack.FoodTracker_MDI
{
    public partial class FoodTrackerMDI : Form
    {
        FoodContext context;
        FoodCategoryRepository foodCategoryRepo;
        FoodRepository foodRepo;
        UnitOfWork UnitOfWork;
        PresetMealRepository presetMealRepo;
        public FoodTrackerService fTracker;
        public FoodTrackerMDI()
        {
            InitializeComponent();
            context = new FoodContext();
            foodRepo = new FoodRepository(context);
            foodCategoryRepo = new FoodCategoryRepository(context);
            presetMealRepo = new PresetMealRepository(context);
            UnitOfWork = new UnitOfWork(context,foodRepo, foodCategoryRepo, presetMealRepo);
            fTracker = new FoodTrackerService(UnitOfWork);
            Menu menu = new Menu();
            menu.MdiParent = this;
            menu.MDI = this;
            menu.Show();
        }
    }
}
