using Security.DAL;
using Security.Entities;
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
    /// Interaction logic for AddPermission.xaml
    /// </summary>
    public partial class AddPermission : Window
    {
        private List<Security.Entities.Application> apps;
        private User user;
        private controlPanel parent;

        public AddPermission(User user, controlPanel parent)
        {
            InitializeComponent();
            apps = Security.DAL.ApplicationDAO.getAllApplicationRecords();
            String[] appsArr = (from app in apps select app.Application_Name).ToArray<String>();
            this.user = user;
            this.parent = parent;
            appComboBox.ItemsSource = appsArr;
            appComboBox.SelectedIndex = 0;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            parent.release();
            base.OnClosing(e);
        }

        private void addPermissionButton_Click(object sender, RoutedEventArgs e)
        {
            if (permissionName.Text.Length > 1)
            {
                String permName = permissionName.Text;
                int idx = appComboBox.SelectedIndex;
                Permission permission = new Permission(0, permName, apps.ElementAt(idx).App_ID, true, DateTime.Now, DateTime.Now, user.User_ID, user.User_ID);

                if (PermissionDAO.savePermission(permission))
                {
                    MessageBox.Show("The Permission is saved", "SUCCESS");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Permission not saved", "FAILURE");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid name for permission", "INVALID PERMISSION");
            }
        }
    }
}
