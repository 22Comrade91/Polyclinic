using System.Linq;
using System.Windows;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;
using HospitalManagement.Models;

namespace HospitalManagement
{
    public partial class VisitsView : Window
    {
        private HospitalContext _context;

        public VisitsView()
        {
            InitializeComponent();
            _context = new HospitalContext();
            LoadData();
        }

        private void LoadData()
        {
            // Загружаем данные из базы
            var visits = _context.Visits.ToList();
            VisitsDataGrid.ItemsSource = visits; // Устанавливаем источник данных для DataGrid
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var visit = new Visit
            {
                PatientID = 1,
                DoctorID = 1,
                VisitDate = DateTime.Now,
                Diagnosis = "Новый диагноз"
            };
            _context.Visits.Add(visit);
            _context.SaveChanges();
            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (VisitsDataGrid.SelectedItem is Visit selectedVisit)
            {
                selectedVisit.Diagnosis = "Обновленный диагноз";
                _context.SaveChanges();
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (VisitsDataGrid.SelectedItem is Visit selectedVisit)
            {
                _context.Visits.Remove(selectedVisit);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Список посещений";

            // Добавляем страницу
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12);
            XFont titleFont = new XFont("Verdana", 14);

            // Заголовок страницы
            int yPoint = 40;
            gfx.DrawString("Список посещений", titleFont, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 40;

            // Получаем данные из таблицы
            var visits = _context.Visits.ToList();

            foreach (var visit in visits)
            {
                string line = $"ID: {visit.ID}, Пациент ID: {visit.PatientID}, Врач ID: {visit.DoctorID}, Дата: {visit.VisitDate.ToShortDateString()}, Диагноз: {visit.Diagnosis}";
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
            string filename = "VisitsList.pdf";
            document.Save(filename);

            // Открываем PDF
            Process.Start(new ProcessStartInfo
            {
                FileName = filename,
                UseShellExecute = true
            });
        }
    }
}
