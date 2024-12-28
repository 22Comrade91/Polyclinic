using HospitalApp;
using HospitalManagement.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HospitalManagement
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PatientsButton_Click(object sender, RoutedEventArgs e)
        {
            var patientsView = new PatientsView();
            patientsView.Show();
        }

        private void DoctorsButton_Click(object sender, RoutedEventArgs e)
        {
            var doctorsView = new DoctorsView();
            doctorsView.Show();
        }

        private void VisitsButton_Click(object sender, RoutedEventArgs e)
        {
            var visitsView = new VisitsView();
            visitsView.Show();
        }

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            var servicesView = new ServicesView();
            servicesView.Show();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            var reportsView = new ReportsView();
            reportsView.Show();
        }
    }
}