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
    /// Interaction logic for EditUser.xaml
    /// </summary>
    public partial class EditUser : Window
    {
        private User user;
        private controlPanel parent;
        private List<User> users;
        private List<Security.Entities.Application> applications;
        private int activeChildren = 0;

        public EditUser(User user, controlPanel parent)
        {
            InitializeComponent();
            this.user = user;
            this.parent = parent;
            applications = ApplicationDAO.getAllApplicationRecords();
            users = UserDAO.getAllUsers();

            user_combobox.ItemsSource = (from u in users select u.LoginID).ToArray<String>();
            user_combobox.SelectedIndex = 0;
        }

        public void release()
        {
            activeChildren--;
            refresh();
        }

        private void refresh()
        {
            users = UserDAO.getAllUsers();
            user_combobox.ItemsSource = (from u in users select u.LoginID).ToArray<String>();
            user_combobox.SelectedIndex = 0;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            parent.release();
            base.OnClosing(e);
        }

        private void user_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = user_combobox.SelectedIndex;

            User temp = users.ElementAt(idx);

            userid_field.Text = temp.User_ID.ToString();
            applications_listbox.ItemsSource = (from a in applications join ua in temp.App_ID on a.App_ID equals ua select a.Application_Name).ToArray<String>();
            roles_listbox.ItemsSource = (from r in temp.Roles select r.Role_Name).ToArray<String>();

        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            int idx = user_combobox.SelectedIndex;
            User temp = users.ElementAt(idx);

            activeChildren++;
            new AddUser(user, temp, this, applications).Show();
        }

    }
}
