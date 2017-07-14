using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class DummyUser
    {
        private int user_id = -1
            , app_id = -1;
        private String login_id, 
            password;
        private Boolean isActive;

        public DummyUser(int user_id, String login, String password, int app_id, Boolean activeFlag)
        {
            this.user_id = user_id;
            this.login_id = login;
            this.password = password;
            this.app_id = app_id;
            this.isActive = activeFlag;
        }

        public int User_ID
        {
            get { return user_id; }
            set { user_id = value; }
        }

        public String Login_ID
        {
            get { return login_id; }
            set { login_id = value; }
        }

        public String Password
        {
            get { return password; }
            set { password = value; }
        }

        public int SupportedAppID
        {
            get { return app_id; }
            set { app_id = value; }
        }

        public Boolean IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

    }
}
