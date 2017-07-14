using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class Role : ManipulationRecordObject
    {
        private int role_id = -1, supportedApplicationId = -1;
        private String roleName;
        private List<Permission> permissions = null;

        public Role(int role_id, String roleName, int supportedApplicationId, List<Permission> permissions, Boolean isActive, DateTime createdOn, DateTime lastModifiedOn, int createdBy, int lastModifiedBy)
            : base(isActive, createdOn, lastModifiedOn, createdBy, lastModifiedBy)
        {
            this.roleName = roleName;
            this.role_id = role_id;
            this.permissions = permissions;
            this.supportedApplicationId = supportedApplicationId;
        }

        public int Role_ID
        {
            get { return role_id; }
            set { role_id = value; }
        }

        public String Role_Name
        {
            get { return roleName; }
            set { roleName = value; }
        }

        public List<Permission> Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }

        public int SupportedApplicationId
        {
            get { return supportedApplicationId; }
            set { supportedApplicationId = value; }
        }

    }
}
