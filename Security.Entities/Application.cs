using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class Application : ManipulationRecordObject
    {
        private int app_id = -1;
        private String applicationName;
        private List<Role> roles = null;

        public Application(int app_id, String applicationName, List<Role> roles, Boolean isActive, DateTime createdOn, DateTime lastModifiedOn, int createdBy, int lastModifiedBy)
            : base(isActive, createdOn, lastModifiedOn, createdBy, lastModifiedBy)
        {
            this.applicationName = applicationName;
            this.app_id = app_id;
            this.roles = roles;
        }

        public String Application_Name
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public int App_ID
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
