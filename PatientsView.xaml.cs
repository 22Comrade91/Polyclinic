using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;
using HospitalManagement.Models;
using HospitalManagement;

namespace HospitalApp
{
    public partial class PatientsView : Window
    {
        private HospitalContext _context;

        public PatientsView()
        {
            InitializeComponent();
            _context = new HospitalContext();
            LoadData();
        }

        // Загружаем данные в DataGrid
        private void LoadData()
        {
            var patients = _context.Patients.ToList();
            PatientsDataGrid.ItemsSource = patients;
        }

        // Обработчик кнопки "Добавить"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var patient = new Patient
            {
                FullName = "Новый пациент",
                Age = 30,
                Gender = "М" // Сохраняем только первый символ
            };

            _context.Patients.Add(patient);
            _context.SaveChanges();
            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                selectedPatient.FullName = "Отредактированный пациент";
                selectedPatient.Age = 35;
                selectedPatient.Gender = "Ж"; // Сохраняем только первый символ
                _context.SaveChanges();
                LoadData();
            }
            else
            {
                MessageBox.Show("Выберите пациента для редактирования.");
            }
        }

        // Обработчик кнопки "Удалить"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                _context.Patients.Remove(selectedPatient);
                _context.SaveChanges();
                LoadData();
            }
            else
            {
                MessageBox.Show("Выберите пациента для удаления.");
            }
        }

        // Обработчик кнопки "Экспорт в PDF"
        private void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Список пациентов";

            // Добавляем страницу
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12);
            XFont titleFont = new XFont("Verdana", 14);

            // Заголовок страницы
            int yPoint = 40;
            gfx.DrawString("Список пациентов", titleFont, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 40;

            // Получаем данные из таблицы
            var patients = _context.Patients.ToList();

            foreach (var patient in patients)
            {
                string line = $"ID: {patient.ID}, Имя: {patient.FullName}, Возраст: {patient.Age}, Пол: {patient.Gender}";
                gfx.DrawString(line, font, XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 20;

                // Если строк больше, чем помещается на странице, добавляем новую
                if (yPoint > page.Height - 40)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = 40;
                }
            }

            // Сохраняем PDF
            string filename = "PatientsList.pdf";
            document.Save(filename);

            // Открываем PDF
            Process.Start(new ProcessStartInfo
            {
                FileName = filename,
                UseShellExecute = true
            });
        }

        // Поиск пациентов
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();
            var filteredPatients = _context.Patients
                .Where(p => p.FullName.ToLower().Contains(searchText) || p.Age.ToString().Contains(searchText) || p.Gender.ToLower().Contains(searchText))
                .ToList();
            PatientsDataGrid.ItemsSource = filteredPatients;
        }

        // Сортировка DataGrid
        private void PatientsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var direction = e.Column.SortDirection == null || e.Column.SortDirection == System.ComponentModel.ListSortDirection.Descending
                ? System.ComponentModel.ListSortDirection.Ascending
                : System.ComponentModel.ListSortDirection.Descending;
            e.Column.SortDirection = direction;

            var sortedPatients = _context.Patients
                .OrderBy(p => e.Column.SortMemberPath == "Age" ? p.Age.ToString() : p.FullName)
                .ToList();
            PatientsDataGrid.ItemsSource = sortedPatients;
        }
    }
}
