using HospitalManagement.Models;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace HospitalManagement
{
    public partial class ReportsView : Window
    {
        private HospitalContext _context;

        public ReportsView()
        {
            InitializeComponent();
            _context = new HospitalContext();
            LoadData();
        }

        private void LoadData()
        {
            var reports = _context.Reports.ToList();
            ReportsDataGrid.ItemsSource = reports;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var report = new Report
            {
                Type = "Новый отчет",
                ReportDate = DateTime.Now,
                Description = "Описание нового отчета"
            };
            _context.Reports.Add(report);
            _context.SaveChanges();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem is Report selectedReport)
            {
                _context.Reports.Remove(selectedReport);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Список отчетов";

            // Добавляем страницу
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12);
            XFont titleFont = new XFont("Verdana", 14);

            // Заголовок страницы
            int yPoint = 40;
            gfx.DrawString("Список отчетов", titleFont, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 40;

            // Получаем данные из таблицы
            var reports = _context.Reports.ToList();

            foreach (var report in reports)
            {
                string line = $"ID: {report.ID}, Тип: {report.Type}, Дата: {report.ReportDate.ToShortDateString()}, Описание: {report.Description}";
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
            string filename = "ReportsList.pdf";
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
