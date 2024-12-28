using System.Linq;
using System.Windows;
using HospitalManagement.Models;
using System.Security.Cryptography;
using System.Text;
using HospitalManagement;

namespace HospitalApp
{
    public partial class RegisterWindow : Window
    {
        private readonly HospitalContext _context;

        public RegisterWindow()
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка, существует ли уже пользователь с таким именем
            if (_context.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = "user" // Роль по умолчанию, можно добавить администраторский доступ
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
