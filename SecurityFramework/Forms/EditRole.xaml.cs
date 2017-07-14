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
    /// Interaction logic for EditRole.xaml
    /// </summary>
    public partial class EditRole : Window
    {
        private User user;
        private controlPanel parent;
        private List<Security.Entities.Application> apps;
        private List<Role> roles;
        private int childCount = 0;

        public EditRole(User user, controlPanel parent)
        {
            InitializeComponent();
            this.user = user;
            this.parent = parent;
            apps = ApplicationDAO.getAllApplicationRecords();
            String[] appNames = (from app in apps select app.Application_Name).ToArray<String>();
            application_combobox.ItemsSource = appNames;

            application_combobox.SelectedIndex = 0;
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            int idx = application_combobox.SelectedIndex;
            Security.Entities.Application app = apps.ElementAt(idx);
            idx = role_combobox.SelectedIndex;

            childCount++;
            new AddRole(user, this, roles.ElementAt(idx), app).Show();
            edit_btn.IsEnabled = false;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            parent.release();
            base.OnClosing(e);
        }

        public void release()
        {
            childCount--;
            if (childCount == 0)
                edit_btn.IsEnabled = true;
        }

        private void application_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = application_combobox.SelectedIndex;
            roles = (from role in apps.ElementAt(idx).Roles select role).ToList<Role>();
            role_combobox.ItemsSource = (from role in roles select role.Role_Name).ToArray<String>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
