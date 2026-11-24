
using System.Linq;
using System.Windows;

namespace test_deemm
{
    public partial class OrdersWindow : Window
    {
        catalogEntities1 db = new catalogEntities1();
        int roleId;

        public OrdersWindow(int roleId)
        {
            InitializeComponent();
            this.roleId = roleId;

            if (roleId != 1)
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }

            LoadOrders();
        }

        private void LoadOrders()
        {
            gridOrders.ItemsSource = db.orders.ToList();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = tbSearch.Text.Trim().ToLower();

            gridOrders.ItemsSource = db.orders
                .Where(x =>
                    x.full_name.ToLower().Contains(search) ||
                    x.item_number.ToLower().Contains(search) ||
                    x.status.ToLower().Contains(search))
                .ToList();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Clear();
            LoadOrders();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            orders order = new orders();

            // Следующий номер заказа
            order.Order_number = db.orders.Any()
                ? db.orders.Max(x => x.Order_number) + 1
                : 1;

            OrderEditWindow window = new OrderEditWindow(order);
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                try
                {
                    db.orders.Add(order);
                    db.SaveChanges();
                    LoadOrders();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка добавления заказа: " + ex.Message);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            orders order = gridOrders.SelectedItem as orders;

            if (order == null)
            {
                MessageBox.Show("Выберите заказ.");
                return;
            }

            OrderEditWindow window = new OrderEditWindow(order);
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                try
                {
                    db.SaveChanges();
                    LoadOrders();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка изменения заказа: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            orders order = gridOrders.SelectedItem as orders;

            if (order == null)
            {
                MessageBox.Show("Выберите заказ.");
                return;
            }

            if (MessageBox.Show("Удалить выбранный заказ?",
                "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    db.orders.Remove(order);
                    db.SaveChanges();
                    LoadOrders();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка удаления: " + ex.Message);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            db.Dispose();
            base.OnClosed(e);
        }
    }
}
