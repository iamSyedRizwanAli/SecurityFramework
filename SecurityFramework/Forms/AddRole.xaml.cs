using Security.DAL;
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
    /// Interaction logic for AddRole.xaml
    /// </summary>
    public partial class AddRole : Window
    {
        private List<Security.Entities.Application> apps;
        private List<Permission> permissions;
        private List<String> permStrings, selectedPermStrings;
        private int selectedAppId = -1;
        private User user;
        private controlPanel parent;
        private EditRole eparent;
        private Boolean isParentControlPanel;
        private Role ofeRole;

        public AddRole(User user, controlPanel parent)
        {
            InitializeComponent();
            this.user = user;
            this.parent = parent;
            apps = ApplicationDAO.getAllApplicationRecords();
            
            String [] appNames = (from app in apps select app.Application_Name).ToArray<String>();
            appComboBox.ItemsSource = appNames;
            appComboBox.SelectedIndex = 0;
            
            isParentControlPanel = true;
            edit_btn.Visibility = System.Windows.Visibility.Hidden;
            isActive_labelv.Visibility = System.Windows.Visibility.Hidden;
            isActive_combobox.Visibility = System.Windows.Visibility.Hidden;

        }

        public AddRole(User user, EditRole parent, Role role, Security.Entities.Application app)
        {
            InitializeComponent();
            
            this.user = user;
            this.eparent = parent;

            addButton.Content = "Save Role";
            winTitle.Content = "Edit Role";

            String[] appName = { app.Application_Name};
            appComboBox.ItemsSource = appName;
            appComboBox.IsEnabled = false;

            isParentControlPanel = false;
            
            ofeRole = role;
            apps = new List<Security.Entities.Application>();
            apps.Add(app);

            loadRole(role, app);
        }

        private void loadRole(Role role, Security.Entities.Application app)
        {
            appComboBox.SelectedIndex = 0;

            role_name.Text = role.Role_Name;
            role_name.IsEnabled = false;

            selectedPermStrings = (from rol in role.Permissions select rol.Permission_Name).ToList<String>();
            permStrings = permStrings.Except(selectedPermStrings).ToList<String>();
            selected_applications_list_box.ItemsSource = selectedPermStrings.ToArray<String>();
            avalaible_applications_list_box.ItemsSource = permStrings.ToArray<String>();

            String[] active = { "Active" , "Disabled"};
            isActive_combobox.ItemsSource = active;

            isActive_combobox.SelectedIndex = role.IsActive ? 0 : 1;

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (isParentControlPanel)
                parent.release();
            else
                eparent.release();

            base.OnClosing(e);
        }

        private void appComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String appName = appComboBox.SelectedItem.ToString();
            List<Security.Entities.Application> temp = (from a in apps where a.Application_Name.Equals(appName) select a).ToList<Security.Entities.Application>();

            if (selectedAppId != temp.ElementAt(0).App_ID)
            {
                selectedAppId = temp.ElementAt(0).App_ID;
                selectedPermStrings = new List<string>();

                getPermissionsOfThisApplication(temp.ElementAt(0));
                avalaible_applications_list_box.ItemsSource = permStrings.ToArray<String>();
                selected_applications_list_box.Items.Clear();
            }
        }

        private void getPermissionsOfThisApplication(Security.Entities.Application application)
        {
            permissions = PermissionDAO.getAllPermissionsForApplication(application.App_ID);
            
            permStrings = (from perms in permissions select perms.Permission_Name).ToList<String>();
        }

        private void add_to_selected_permissions_Click(object sender, RoutedEventArgs e)
        {
            if (permStrings.Count > 0)
            {
                int idx = avalaible_applications_list_box.SelectedIndex;

                if (idx > -1)
                {
                    selectedPermStrings.Add(permStrings.ElementAt(idx));
                    permStrings.RemoveAt(idx);

                    avalaible_applications_list_box.ItemsSource = permStrings.ToArray<String>();
                    selected_applications_list_box.ItemsSource = selectedPermStrings.ToArray<String>();
                }
                else
                {
                    MessageBox.Show("Kindly select a permission to add", "UNKNOWN PERMISSION");
                }
            }
            else
            {
                MessageBox.Show("No permissions to add", "EMPTY LIST");
            }
        }

        private void remove_from_selected_permissions_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPermStrings.Count > 0)
            {
                int idx = selected_applications_list_box.SelectedIndex;

                if (idx > -1)
                {
                    permStrings.Add(selectedPermStrings.ElementAt(idx));
                    selectedPermStrings.RemoveAt(idx);

                    avalaible_applications_list_box.ItemsSource = permStrings.ToArray<String>();
                    selected_applications_list_box.ItemsSource = selectedPermStrings.ToArray<String>();
                }
                else
                {
                    MessageBox.Show("Please select a Permission to remove", "UNKNOWN PERMISSION");
                }
            }
            else
            {
                MessageBox.Show("No permissions to remove", "EMPTY LIST");
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (role_name.Text.Length > 1)
            {
                if (selectedPermStrings.Count > 0)
                {
                    bool flag = false;
                    if (isParentControlPanel)
                    {
                        List<Permission> permissionAssigned = (from pInList in permissions join strings in selectedPermStrings on pInList.Permission_Name equals strings select pInList).ToList<Permission>();

                        DateTime dateTime = DateTime.Now;
                        Role role = new Role(0, role_name.Text, selectedAppId, permissionAssigned, true, dateTime, dateTime, user.User_ID, user.User_ID);

                        flag = RoleDAO.saveRole(role);
                    }
                    else
                    {
                        List<Permission> permissionAssigned = (from pInList in permissions join strings in selectedPermStrings on pInList.Permission_Name equals strings select pInList).ToList<Permission>();
                        ofeRole.Permissions = permissionAssigned;
                        ofeRole.LastModifiedOn = DateTime.Now;
                        ofeRole.LastModifiedBy = user.User_ID;
                        ofeRole.Role_Name = role_name.Text;
                        ofeRole.IsActive = isActive_combobox.SelectedIndex == 0 ? true : false;

                        flag = RoleDAO.updateRole(ofeRole);

                    }

                    if (flag)
                    {
                        MessageBox.Show("Role added", "SUCCESS");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Role not added", "FAILURE");
                    }

                }
                else
                {
                    MessageBox.Show("Kindly enter some permissions for the role", "NO PERMISSION");
                }
            }
            else
            {
                MessageBox.Show("Kindly enter a valid Role Name", "INVALID ROLE NAME");
            }
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            role_name.IsEnabled = true;
        }

    }
}
