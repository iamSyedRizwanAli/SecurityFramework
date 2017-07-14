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
    /// Interaction logic for EditApplication.xaml
    /// </summary>
    public partial class EditApplication : Window
    {
        private User user;
        private controlPanel parent;
        private List<Security.Entities.Application> applications;

        public EditApplication(User user, controlPanel parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.user = user;
            applications = ApplicationDAO.getAllApplicationRecords();

            String[] appStrings = (from app in applications select app.Application_Name).ToArray<String>();
            String[] active = { "Active", "Disabled" };
            application_combobox.ItemsSource = appStrings;
            isActive_combobox.ItemsSource = active;
            save_btn.IsEnabled = false;

        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            Security.Entities.Application app = applications.ElementAt(application_combobox.SelectedIndex);
            app.IsActive = isActive_combobox.SelectedIndex == 0 ? true : false;
            app.LastModifiedBy = user.User_ID;
            app.LastModifiedOn = DateTime.Now;

            if (ApplicationDAO.updateApplication(app))
            {
                MessageBox.Show("Application updated", "SUCCESS");
            }
            else
            {
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            parent.release();
            base.OnClosing(e);
        }


        private void isActive_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (application_combobox.SelectedIndex > -1)
            {
                int idx = application_combobox.SelectedIndex;

                if (applications.ElementAt(idx).IsActive != (isActive_combobox.SelectedIndex == 0 ? true : false))
                {
                    save_btn.IsEnabled = true;
                }
                else
                {
                    save_btn.IsEnabled = false;
                }

            }
        }

        private void application_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = application_combobox.SelectedIndex;

            isActive_combobox.SelectedIndex = applications.ElementAt(idx).IsActive ? 0 : 1;

        }
    }
}
