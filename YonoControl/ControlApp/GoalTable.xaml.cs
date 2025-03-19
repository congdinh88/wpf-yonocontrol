using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

namespace YonoControl.ControlApp
{
    /// <summary>
    /// Interaction logic for GoalTable.xaml
    /// </summary>
    public partial class GoalTable : UserControl
    {
        public ObservableCollection<Goal> Goals { get; set; }
        public GoalTable()
        {
            InitializeComponent();
            Goals = new ObservableCollection<Goal>();
            dataGrid.ItemsSource = Goals;
        }
    }

    public class Goal
    {
        public string Id {  get; set; }
        public string Team { get; set; }
        public int? Goals { get; set; }
        public bool? Possition { get; set; }
    }
}
