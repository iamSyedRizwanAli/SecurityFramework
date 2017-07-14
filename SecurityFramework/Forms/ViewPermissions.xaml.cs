using Security.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SecurityFramework
{
    /// <summary>
    /// Interaction logic for ViewPermissions.xaml
    /// </summary>
    public partial class ViewPermissions : Window
    {
        private controlPanel parent;

        public ViewPermissions(controlPanel parent)
        {
            InitializeComponent();
            this.parent = parent;
            dataGrid.IsReadOnly = true;
            dataGrid.ItemsSource = PermissionDAO.getAllPermissions();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            parent.release();
            base.OnClosing(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}