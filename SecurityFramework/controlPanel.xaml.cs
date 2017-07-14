using Security.Entities;
using SecurityFramework.Forms;
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
    /// Interaction logic for controlPanel.xaml
    /// </summary>
    public partial class controlPanel : Window
    {
        private Window parent;
        private User user;
        private int activeChildren = 0;
        
        public controlPanel(User user, Window window)
        {
            InitializeComponent();
            parent = window;
            parent.Hide();
            this.user = user;
            init();
        }

        public void release()
        {
            activeChildren--;
        }

        public void captured()
        {
            activeChildren++;
        }

        private void init()
        {
            nameOfUser.Content = user.LoginID;

            String[] roles = (from ur in user.Roles select ur.Role_Name).ToArray<String>();
            roles_listbox.ItemsSource = roles;

            String[] permissions = null;
            List<String> permList = new List<string>();

            foreach (Role r in user.Roles)
            {
                List<String> res = (from p in r.Permissions where p.IsActive == true select p.Permission_Name).ToList<String>();
                permList.AddRange(res);
            }
            permissions = permList.ToArray<String>();

            permissions_ComboBox.ItemsSource = permissions;
            permissions_ComboBox.SelectedIndex = 0;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            parent.Show();
        }

        private void logout_btn_Click(object sender, RoutedEventArgs e)
        {
            if (activeChildren == 0)
                this.Close();
            else
                MessageBox.Show("You cannot log out until you close all of the running windows.", "ACTIVE APPLICATION");
        }

        private void do_button_Click(object sender, RoutedEventArgs e)
        {
            String action = permissions_ComboBox.SelectedItem.ToString();

            if (action.Equals("Create an Application"))
            {
                activeChildren++;
                new AddApplication(user, this).Show();
            }
            else if(action.Equals("Create a User"))
            {
                activeChildren++; 
                new AddUser(user, this).Show();
            }
            else if(action.Equals("Create a Role"))
            {
                activeChildren++;
                new AddRole(user, this).Show();
            }
            else if(action.Equals("Create a Permission"))
            {
                activeChildren++;
                new AddPermission(user, this).Show();
            }
            else if (action.Equals("View Users"))
            {
                activeChildren++;
                new ViewUsers(this).Show();
            }
            else if (action.Equals("View Roles"))
            {
                activeChildren++;
                new ViewRoles(this).Show();
            }
            else if (action.Equals("View Permissions"))
            {
                activeChildren++;
                new ViewPermissions(this).Show();   
            }
            else if (action.Equals("View Applications"))
            {
                activeChildren++;
                new ViewApplications(this).Show();
            }
            else if (action.Equals("Edit a User"))
            {
                activeChildren++;
                new EditUser(user, this).Show();
            }
            else if (action.Equals("Edit a Role"))
            {
                activeChildren++;
                new EditRole(user, this).Show();
            }
            else if (action.Equals("Edit a Permission"))
            {
                activeChildren++;
                new EditPermission(user, this).Show();
            }
            else if (action.Equals("Edit an Application"))
            {
                activeChildren++;
                new EditApplication(user, this).Show();
            }
        }

    }
}
