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

namespace SecurityFramework.Forms
{
    /// <summary>
    /// Interaction logic for EditPermission.xaml
    /// </summary>
    public partial class EditPermission : Window
    {
        private User user;
        private controlPanel parent;
        private List<Security.Entities.Application> applications;
        private List<Permission> permissions;

        public EditPermission(User user, controlPanel parent)
        {
            this.user = user;
            this.parent = parent;
            InitializeComponent();
            applications = ApplicationDAO.getAllApplicationRecords();
            permissions = PermissionDAO.getAllPermissions();

            String [] permStrings = (from p in permissions select p.Permission_Name).ToArray<String>();
            String[] active = { "Active", "Disabled"};
            permission_combobox.ItemsSource = permStrings;
            isActive_combobox.ItemsSource = active;
            save_btn.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            Permission perm = permissions.ElementAt(permission_combobox.SelectedIndex);
            perm.IsActive = isActive_combobox.SelectedIndex == 0 ? true : false;
            perm.LastModifiedBy = user.User_ID;
            perm.LastModifiedOn = DateTime.Now;
            
            if (PermissionDAO.updatePermission(perm))
            {
                save_btn.IsEnabled = false;                
            }
            else
            {
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            parent.release();
            base.OnClosing(e);
        }

        private void permission_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = permission_combobox.SelectedIndex;

            List<Security.Entities.Application> temp = (from ap in applications where ap.App_ID == permissions.ElementAt(idx).SupportedApplicationId select ap).ToList<Security.Entities.Application>();
            app_name_field.Text = temp.ElementAt(0).Application_Name;

            isActive_combobox.SelectedIndex = permissions.ElementAt(idx).IsActive ? 0 : 1;

        }

        private void isActive_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (permission_combobox.SelectedIndex > -1)
            {
                int idx = permission_combobox.SelectedIndex;

                if (permissions.ElementAt(idx).IsActive != (isActive_combobox.SelectedIndex == 0 ? true : false))
                {
                    save_btn.IsEnabled = true;
                }
                else
                {
                    save_btn.IsEnabled = false;
                }

            }
        }
    }
}
