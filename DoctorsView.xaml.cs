using System;
using System.Linq;
using System.Windows;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;
using HospitalManagement.Models;

namespace HospitalManagement.Views
{
    public partial class DoctorsView : Window
    {
        private HospitalContext _context;

        public DoctorsView()
        {
            InitializeComponent();
            _context = new HospitalContext();
            LoadData();
        }

        private void LoadData()
        {
            DoctorsDataGrid.ItemsSource = _context.Doctors.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var doctor = new Doctor { FullName = "Новый врач", Specialty = "Общая практика", Contact = "000-000" };
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorsDataGrid.SelectedItem is Doctor selectedDoctor)
            {
                selectedDoctor.FullName = "Отредактированный врач";
                _context.SaveChanges();
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorsDataGrid.SelectedItem is Doctor selectedDoctor)
            {
                _context.Doctors.Remove(selectedDoctor);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Список врачей";

            // Добавляем страницу
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Определяем шрифты
            XFont font = new XFont("Verdana", 12); // Обычный шрифт
            XFont titleFont = new XFont("Verdana", 14); // Жирный шрифт для заголовков

            // Заголовок страницы
            int yPoint = 40;
            gfx.DrawString("Список врачей", titleFont, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 40;

            // Получаем данные из таблицы
            var doctors = _context.Doctors.ToList();

            foreach (var doctor in doctors)
            {
                string line = $"ID: {doctor.ID}, Имя: {doctor.FullName}, Специальность: {doctor.Specialty}, Контакт: {doctor.Contact}";
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
            string filename = "DoctorsList.pdf";
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
