
using System;
using System.Linq;
using System.Windows;

namespace test_deemm
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Вход по логину и паролю
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "";

            string login = tblog.Text.Trim();
            string password = pBox.Password;

            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password))
            {
                txtStatus.Text = "Введите логин и пароль";
                return;
            }

            try
            {
                using (var db = new catalogEntities1())
                {
                    var userObj = db.user_import.FirstOrDefault(x =>
                        x.login == login &&
                        x.password == password);

                    if (userObj == null)
                    {
                        txtStatus.Text = "Неверный логин или пароль";
                        return;
                    }

                    int roleId;

                    if (userObj.Employee_role == "Администратор")
                        roleId = 1;
                    else if (userObj.Employee_role == "Менеджер")
                        roleId = 2;
                    else if (userObj.Employee_role == "Авторизированный клиент")
                        roleId = 3;
                    else
                    {
                        txtStatus.Text = "Неизвестная роль пользователя";
                        return;
                    }

                    ProductsWindow window = new ProductsWindow(roleId);
                    window.Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при авторизации:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Вход гостем
        private void btnGuest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProductsWindow window = new ProductsWindow(0);
                window.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось открыть каталог:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
