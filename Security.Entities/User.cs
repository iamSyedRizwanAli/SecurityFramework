using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class User : ManipulationRecordObject
    {
        private int user_id;
        private List<int> app_id;
        private String login_id, password;
        private List<Role> roles;

        public User(int user_id, String login_id, String password, List<int> app_id, List<Role> roles, Boolean isActive, DateTime createdOn, DateTime lastModifiedOn, int createdBy, int lastModifiedBy)
            : base(isActive, createdOn, lastModifiedOn, createdBy, lastModifiedBy)
        {
            this.user_id = user_id;
            this.login_id = login_id;
            this.password = password;
            this.app_id = app_id;
            this.roles = roles;
        }

        public int User_ID
        {
            get { return user_id; }
            set { user_id = value; }
        }

        public String LoginID
        {
            get { return login_id; }
            set { login_id = value; }
        }

        public String Password
        {
            get { return password; }
            set { password = value; }
        }
        
        public List<int> App_ID
        {
            get { return app_id; }
            set { app_id = value; }
        }

        public List<Role> Roles
        {
            get { return roles; }
            set { roles = value; }
        }

    }
}
