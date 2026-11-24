using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace test_deemm
{
    public partial class ProductsWindow : Window
    {
        catalogEntities1 db = new catalogEntities1();
        int roleId;

        public ProductsWindow(int roleId)
        {
            InitializeComponent();
            this.roleId = roleId;

            // Настройка прав
            if (roleId == 0 || roleId == 3)
            {
                btnOrders.Visibility = Visibility.Collapsed;
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;

                tbSearch.IsEnabled = false;
                cbCategory.IsEnabled = false;
                cbSort.IsEnabled = false;
                btnReset.IsEnabled = false;
            }
            else if (roleId == 2)
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }

            cbCategory.Items.Add("Все категории");

            foreach (var item in db.Tovar.Select(x => x.Product_category).Distinct())
            {
                if (!string.IsNullOrWhiteSpace(item))
                    cbCategory.Items.Add(item);
            }

            cbCategory.SelectedIndex = 0;
            cbSort.SelectedIndex = 0;

            LoadProducts();
        }

        private string ResolvePhotoPath(string rawPhoto)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = string.IsNullOrWhiteSpace(rawPhoto) ? "picture.png" : rawPhoto.Trim();

            string[] candidates =
            {
                Path.Combine(baseDir, fileName),
                Path.Combine(baseDir, "Images", fileName),
                Path.Combine(baseDir, "Resources", fileName),
                Path.Combine(baseDir, "picture.png")
            };

            foreach (string path in candidates)
            {
                if (File.Exists(path))
                    return path;
            }

            return null;
        }

        private List<Tovar> GetPreparedProducts()
        {
            var products = db.Tovar.ToList();
            foreach (var p in products)
            {
                p.photo = ResolvePhotoPath(p.photo);
            }
            return products;
        }

        private void LoadProducts()
        {
            var products = GetPreparedProducts();
            gridProducts.ItemsSource = products;
            txtCount.Text = "Товаров: " + products.Count;
        }

        private void ApplyFilters()
        {
            var products = GetPreparedProducts();
            string search = tbSearch.Text.Trim().ToLower();

            if (search != "")
            {
                products = products.Where(x =>
                    (x.article ?? "").ToLower().Contains(search) ||
                    (x.Product_name ?? "").ToLower().Contains(search) ||
                    (x.Product_category ?? "").ToLower().Contains(search))
                    .ToList();
            }

            if (cbCategory.SelectedIndex > 0)
            {
                string category = cbCategory.SelectedItem.ToString();
                products = products.Where(x => x.Product_category == category).ToList();
            }

            if (cbSort.SelectedIndex == 1)
                products = products.OrderBy(x => x.Price).ToList();

            if (cbSort.SelectedIndex == 2)
                products = products.OrderByDescending(x => x.Price).ToList();

            gridProducts.ItemsSource = products;
            txtCount.Text = "Товаров: " + products.Count;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (db != null && cbCategory.SelectedItem != null)
                ApplyFilters();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (db != null && cbSort.SelectedItem != null)
                ApplyFilters();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Clear();
            cbCategory.SelectedIndex = 0;
            cbSort.SelectedIndex = 0;
            LoadProducts();
        }

        // Скидка больше 15 процентов
        private void gridProducts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Tovar product = e.Row.Item as Tovar;

            if (product != null && product.discount > 15)
                e.Row.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#2E8B57"));
            else
                e.Row.ClearValue(DataGridRow.BackgroundProperty);
        }

        // Добавление
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductEditWindow window = new ProductEditWindow();
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                db.Tovar.Add(window.Product);
                db.SaveChanges();
                LoadCategories();
                ApplyFilters();
            }
        }

        // Изменение
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Tovar product = gridProducts.SelectedItem as Tovar;

            if (product == null)
            {
                MessageBox.Show("Выберите товар.");
                return;
            }

            ProductEditWindow window = new ProductEditWindow(product);
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                db.SaveChanges();
                LoadCategories();
                ApplyFilters();
            }
        }

        // Обновление списка категорий
        private void LoadCategories()
        {
            cbCategory.Items.Clear();
            cbCategory.Items.Add("Все категории");

            foreach (var item in db.Tovar.Select(x => x.Product_category).Distinct())
            {
                if (!string.IsNullOrWhiteSpace(item))
                    cbCategory.Items.Add(item);
            }

            cbCategory.SelectedIndex = 0;
        }

        // Удаление
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Tovar product = gridProducts.SelectedItem as Tovar;

            if (product == null)
            {
                MessageBox.Show("Выберите товар.");
                return;
            }

            if (MessageBox.Show("Удалить товар " + product.Product_name + "?",
                "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    db.Tovar.Remove(product);
                    db.SaveChanges();
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось удалить товар: " + ex.Message);
                }
            }
        }

        // Заказы
        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow window = new OrdersWindow(roleId);
            window.Owner = this;
            window.ShowDialog();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            db.Dispose();
            base.OnClosed(e);
        }
    }
}