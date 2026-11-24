
using System;
using System.Windows;

namespace test_deemm
{
    public partial class ProductEditWindow : Window
    {
        public Tovar Product { get; set; }

        public ProductEditWindow()
        {
            InitializeComponent();
            Product = new Tovar();
        }

        public ProductEditWindow(Tovar product)
        {
            InitializeComponent();
            Product = product;

            tbArticle.Text = product.article;
            tbName.Text = product.Product_name;
            tbUnit.Text = product.Unit_measurement;
            tbPrice.Text = product.Price.ToString();
            tbSupplier.Text = product.Supplier;
            tbManufacturer.Text = product.Manufacturer;
            tbDiscount.Text = product.discount.ToString();
            tbCount.Text = product.Kol_vo_na_sklade.ToString();
            tbDescription.Text = product.Product_description;
            tbPhoto.Text = product.photo;

            cbCategory.Text = product.Product_category;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            short price;
            byte discount;
            byte count;

            if (tbArticle.Text.Trim() == "" || tbName.Text.Trim() == "")
            {
                MessageBox.Show("Заполните артикул и название.");
                return;
            }

            if (!short.TryParse(tbPrice.Text, out price) || price < 0)
            {
                MessageBox.Show("Введите правильную цену.");
                return;
            }

            if (!byte.TryParse(tbDiscount.Text, out discount))
            {
                MessageBox.Show("Введите правильную скидку.");
                return;
            }

            if (!byte.TryParse(tbCount.Text, out count))
            {
                MessageBox.Show("Введите правильное количество.");
                return;
            }

            Product.article = tbArticle.Text.Trim();
            Product.Product_name = tbName.Text.Trim();
            Product.Unit_measurement = tbUnit.Text;
            Product.Price = price;
            Product.Supplier = tbSupplier.Text;
            Product.Manufacturer = tbManufacturer.Text;
            Product.Product_category = cbCategory.Text;
            Product.discount = discount;
            Product.Kol_vo_na_sklade = count;
            Product.Product_description = tbDescription.Text;
            Product.photo = tbPhoto.Text;

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
