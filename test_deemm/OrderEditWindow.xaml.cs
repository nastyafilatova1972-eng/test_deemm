
using System;
using System.Windows;
using System.Windows.Controls;

namespace test_deemm
{
    public partial class OrderEditWindow : Window
    {
        private orders order;

        public OrderEditWindow(orders selectedOrder)
        {
            InitializeComponent();
            order = selectedOrder;

            tbNumber.Text = order.Order_number.ToString();
            tbItems.Text = order.item_number;
            dpOrderDate.SelectedDate = order.Order_date;
            dpDeliveryDate.SelectedDate = order.delivery_date;
            tbPvz.Text = order.address_pvz.ToString();
            tbClient.Text = order.full_name;
            tbCode.Text = order.code.ToString();

            if (!string.IsNullOrWhiteSpace(order.status))
            {
                cbStatus.Items.Add(order.status);
                cbStatus.SelectedIndex = 0;
            }
            else
            {
                cbStatus.SelectedIndex = 0;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int pvz;
            int code;

            if (!int.TryParse(tbPvz.Text, out pvz))
            {
                MessageBox.Show("Введите номер пункта выдачи.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(tbCode.Text) &&
                !int.TryParse(tbCode.Text, out code))
            {
                MessageBox.Show("Код должен быть числом.");
                return;
            }

            order.item_number = tbItems.Text;
            order.Order_date = dpOrderDate.SelectedDate;
            order.delivery_date = dpDeliveryDate.SelectedDate;
            order.address_pvz = pvz;
            order.full_name = tbClient.Text;
            order.code = string.IsNullOrWhiteSpace(tbCode.Text)
                ? (int?)null : int.Parse(tbCode.Text);

            order.status = cbStatus.Text;

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
