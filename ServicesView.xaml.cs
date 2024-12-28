using HospitalManagement.Models;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace HospitalManagement
{
    public partial class ServicesView : Window
    {
        private HospitalContext _context;

        public ServicesView()
        {
            InitializeComponent();
            _context = new HospitalContext();
            LoadData();
        }

        private void LoadData()
        {
            var services = _context.Services.ToList();
            ServicesDataGrid.ItemsSource = services;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var service = new Service
            {
                Name = "Новая услуга",
                Price = 1000.00m // Примерная цена
            };
            _context.Services.Add(service);
            _context.SaveChanges();
            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesDataGrid.SelectedItem is Service selectedService)
            {
                selectedService.Name = "Обновленная услуга";
                selectedService.Price = 1500.00m;
                _context.SaveChanges();
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesDataGrid.SelectedItem is Service selectedService)
            {
                _context.Services.Remove(selectedService);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Список услуг";

            // Добавляем страницу
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12);
            XFont titleFont = new XFont("Verdana", 14);

            // Заголовок страницы
            int yPoint = 40;
            gfx.DrawString("Список услуг", titleFont, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 40;

            // Получаем данные из таблицы
            var services = _context.Services.ToList();

            foreach (var service in services)
            {
                string line = $"ID: {service.ID}, Услуга: {service.Name}, Цена: {service.Price} руб.";
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
            string filename = "ServicesList.pdf";
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
