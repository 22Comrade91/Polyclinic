using System.Linq;
using System.Windows;
using HospitalManagement.Models;
using System.Security.Cryptography;
using System.Text;
using HospitalManagement;

namespace HospitalApp
{
    public partial class LoginWindow : Window
    {
        private readonly HospitalContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new HospitalContext();
        }

        // Хеширование пароля
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = HashPassword(PasswordBox.Password);

            // Поиск пользователя в базе данных
            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.PasswordHash == password);

            if (user != null)
            {
                MessageBox.Show("Вход успешен!");
                // Открытие основного окна
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно регистрации
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
