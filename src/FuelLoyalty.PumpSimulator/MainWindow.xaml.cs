using FuelLoyalty.PumpSimulator.ViewModels;
using System.Windows;

namespace FuelLoyalty.PumpSimulator
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}