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
    /// Interaction logic for AddApplication.xaml
    /// </summary>
    public partial class AddApplication : Window
    {
        private controlPanel parent;
        private User user;
        public AddApplication(User user, controlPanel window)
        {
            InitializeComponent();
            this.user = user;
            this.parent = window;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            String appli = appName.Text;

            DateTime dateTime = DateTime.Now;
            Security.Entities.Application app = new Security.Entities.Application(0, appli, null, true, dateTime, dateTime, user.User_ID, user.User_ID);

            if (ApplicationDAO.saveApplication(app))
            {
                MessageBox.Show("Application has been added", "SUCCESS");
                this.Close();
            }
            else
            {
                MessageBox.Show("Application cannot be added", "FAILURE");
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            parent.release();
            base.OnClosing(e);
        }
    }
}
