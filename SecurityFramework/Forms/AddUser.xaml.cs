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
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        private User user;
        private controlPanel parent;
        private List<Security.Entities.Application> apps;
        private List<Role> roles;
        private List<String> availableApps, selectedApps, availableRoles, selectedRoles;

        private Boolean isParentControlPanel;
        private EditUser eParent;
        private User ofeUser;

        public AddUser(User user, controlPanel parent)
        {
            this.user = user;
            this.parent = parent;
            InitializeComponent();

            isParentControlPanel = true;

            isActive_comboBox.Visibility = System.Windows.Visibility.Hidden;
            isActive_Label.Visibility = System.Windows.Visibility.Hidden;

            apps = ApplicationDAO.getAllApplicationRecords();
            availableApps = (from app in apps select app.Application_Name).ToList<String>();
            selectedApps = new List<string>();
            roles = new List<Role>();
            availableRoles = new List<string>();
            selectedRoles = new List<string>();

            avalaible_applications_list_box.ItemsSource = availableApps;
        }

        public AddUser(User user, User eUser, EditUser es, List<Security.Entities.Application> applications)
        {
            this.user = user;
            this.eParent = es;
            InitializeComponent();

            winTitle.Content = "Edit User";

            isParentControlPanel = false;
            apps = applications;
            ofeUser = eUser;

            availableApps = (from app in apps select app.Application_Name).ToList<String>();
            selectedApps = new List<string>();
            roles = new List<Role>();
            availableRoles = new List<string>();
            selectedRoles = new List<string>();

            avalaible_applications_list_box.ItemsSource = availableApps;

            addButton.Content = "Save User";
            String[] active = { "Active", "Disabled" };
            isActive_comboBox.ItemsSource = active;

            isActive_comboBox.SelectedIndex = ofeUser.IsActive ? 0 : 1;

            loadUser();
        }

        private void loadUser()
        {
            login_field.Text = ofeUser.LoginID;
            password_field.Text = ofeUser.Password;

            List<String> tempStrings = (from a in ofeUser.App_ID join x in apps on a equals x.App_ID select x.Application_Name).ToList<String>();

            foreach (String str in tempStrings)
            {
                int idx = avalaible_applications_list_box.Items.IndexOf(str);
                avalaible_applications_list_box.SelectedIndex = idx;
                add_to_selected_applications_Click(null, null);
            }

            tempStrings = (from x in ofeUser.Roles join y in availableRoles on x.Role_Name equals y.Substring(0, y.IndexOf("(")) select y).ToList<String>();

            foreach (String str in tempStrings)
            {
                int idx = available_roles_listbox.Items.IndexOf(str);
                available_roles_listbox.SelectedIndex = idx;
                add_into_selectedroles_Click(null, null);
            }


        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (isParentControlPanel)
                parent.release();
            else
                eParent.release();

            base.OnClosing(e);
        }

        private void add_to_selected_applications_Click(object sender, RoutedEventArgs e)
        {
            if (availableApps != null && availableApps.Count > 0)
            {
                if (avalaible_applications_list_box.SelectedIndex > -1)
                {
                    int idx = avalaible_applications_list_box.SelectedIndex;

                    String tempAppRef = availableApps.ElementAt(idx);

                    List<Security.Entities.Application> tempo = (from a in apps where a.Application_Name.Equals(tempAppRef) select a).ToList<Security.Entities.Application>();
                    List<String> tempRoles = (from r in tempo.ElementAt(0).Roles select r.Role_Name + "(" + tempo.ElementAt(0).Application_Name + ")").ToList<String>();

                    availableRoles.AddRange(tempRoles);

                    selectedApps.Add(availableApps.ElementAt(idx));
                    availableApps.RemoveAt(idx);

                    avalaible_applications_list_box.ItemsSource = availableApps.ToArray<String>();
                    selected_applications_list_box.ItemsSource = selectedApps.ToArray<String>();

                    available_roles_listbox.ItemsSource = availableRoles.ToArray<String>();
                    roles.AddRange(tempo.ElementAt(0).Roles);
                }
                else
                {
                    MessageBox.Show("Select an application to add", "UNKNOWN APPLICATION");                
                }
            }
            else
            {
                MessageBox.Show("There are no available applications to add", "FAILURE");                
            }
        }

        private void remove_from_selected_applications_Click(object sender, RoutedEventArgs e)
        {
            if (selectedApps != null && selectedApps.Count > 0)
            {
                if (selected_applications_list_box.SelectedIndex > -1)
                {
                    int idx = selected_applications_list_box.SelectedIndex;

                    String removedApplication = selectedApps.ElementAt(idx);

                    availableApps.Add(removedApplication);
                    selectedApps.RemoveAt(idx);

                    avalaible_applications_list_box.ItemsSource = availableApps.ToArray<String>();
                    selected_applications_list_box.ItemsSource = selectedApps.ToArray<String>();

                    List<String> actSelectedRoles = (from sr in selectedRoles where !sr.Contains("(" + removedApplication + ")") select sr).ToList<String>(),
                        actAvailableRoles = (from ar in availableRoles where !ar.Contains("(" + removedApplication + ")") select ar).ToList<String>();

                    roles = (from r in roles join x in actAvailableRoles on r.Role_Name equals x select r).ToList<Role>();

                    selectedRoles = actSelectedRoles;
                    availableRoles = actAvailableRoles;

                    available_roles_listbox.ItemsSource = availableRoles.ToArray<String>();
                    selected_roles_listbox.ItemsSource = selectedRoles.ToArray<String>();

                }
                else
                {
                    MessageBox.Show("Select an application to remove", "UNKNOWN APPLICATION");                
                }
            }
            else
            {
                MessageBox.Show("There are no selected application to remove", "FAILURE");
            }
        }

        private void add_into_selectedroles_Click(object sender, RoutedEventArgs e)
        {
            if (availableRoles != null && availableRoles.Count > 0)
            {
                if (available_roles_listbox.SelectedIndex > -1)
                {
                    int idx = available_roles_listbox.SelectedIndex;

                    selectedRoles.Add(availableRoles.ElementAt(idx));
                    availableRoles.RemoveAt(idx);

                    selected_roles_listbox.ItemsSource = selectedRoles.ToArray<String>();
                    available_roles_listbox.ItemsSource = availableRoles.ToArray<String>();
                }
                else
                {
                    MessageBox.Show("Select an role to add", "UNKNOWN ROLE");                
                }
            }
            else
            {
                MessageBox.Show("There are no available roles to add", "FAILURE");
            }
        }

        private void remove_from_selected_roles_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRoles != null && selectedRoles.Count > 0)
            {
                if (selected_roles_listbox.SelectedIndex > -1)
                {
                    int idx = selected_roles_listbox.SelectedIndex;

                    availableRoles.Add(selectedRoles.ElementAt(idx));
                    selectedRoles.RemoveAt(idx);

                    selected_roles_listbox.ItemsSource = selectedRoles.ToArray<String>();
                    available_roles_listbox.ItemsSource = availableRoles.ToArray<String>();

                }
                else
                {
                    MessageBox.Show("Select a role to remove", "UNKNOWN ROLE");                
                }
            }
            else
            {
                MessageBox.Show("There are no selected roles to remove", "FAILURE");
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (login_field.Text.Length > 1)
            {
                if (password_field.Text.Length >= 5)
                {
                    if (selectedApps.Count > 0)
                    {
                        if (areRolesValid())
                        {
                            bool flag = false;

                            if (isParentControlPanel)
                            {
                                List<int> app_ids = (from sapp in apps join appStr in selectedApps on sapp.Application_Name equals appStr select sapp.App_ID).ToList<int>();
                                List<String> actRoles = (from srole in selectedRoles select srole.Substring(0, srole.IndexOf("("))).ToList<String>();
                                List<Role> sRoles = (from srole in actRoles join role in roles on srole equals role.Role_Name select role).ToList<Role>();

                                User deUser = new User(0, login_field.Text, password_field.Text, app_ids, sRoles, true, DateTime.Now, DateTime.Now, user.User_ID, user.User_ID);

                                flag = UserDAO.saveUser(deUser);
                            }
                            else
                            {
                                ofeUser.LoginID = login_field.Text;
                                ofeUser.Password = password_field.Text;

                                List<int> app_ids = (from sapp in apps join appStr in selectedApps on sapp.Application_Name equals appStr select sapp.App_ID).ToList<int>();
                                List<String> actRoles = (from srole in selectedRoles select srole.Substring(0, srole.IndexOf("("))).ToList<String>();
                                List<Role> sRoles = (from srole in actRoles join role in roles on srole equals role.Role_Name select role).ToList<Role>();

                                ofeUser.App_ID = app_ids;
                                ofeUser.Roles = sRoles;
                                ofeUser.IsActive = isActive_comboBox.SelectedIndex == 0 ? true : false;
                                ofeUser.LastModifiedBy = user.User_ID;
                                ofeUser.LastModifiedOn = DateTime.Now;

                                flag = UserDAO.updateUser(ofeUser);

                            }

                            if (flag)
                            {
                                MessageBox.Show("The user has been saved", "SUCCESS");
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("The user cannot be saved", "FAILURE");
                            }
                        }
                        else
                        {
                            String appps = "\n";
                            foreach(String str in selectedApps)
                                appps += "\t-> " + str + "\n";
                            MessageBox.Show("Please select role(s) from following applications " + appps + "\t for this user.", "No roles found");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select an application for this user", "NO APPLICATIONS FOUND");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter valid password", "INVALID PASSWORD");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid Username", "INVALID USERNAME");
            }
        }

        private Boolean areRolesValid()
        {
            if (selectedRoles.Count > 0)
            {
                foreach (String str in selectedApps)
                {
                    int count = (from roleStr in selectedRoles where roleStr.Contains("(" + str + ")") select roleStr).Count();
                    if (count == 0)
                        return false;
                }
            }

            return true;
        }

    }
}
