using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class Permission : ManipulationRecordObject
    {
        private int permissionId = -1, supportedApplicationId = -1;
        private String permissionName;

        public Permission(int permissionId, String permissionName, int supportedApplicationId, Boolean isActive, DateTime createdOn, DateTime lastModifiedOn, int createdBy, int lastModifiedBy)
            : base(isActive, createdOn, lastModifiedOn, createdBy, lastModifiedBy)
        {
            this.permissionName = permissionName;
            this.permissionId = permissionId;
            this.supportedApplicationId = supportedApplicationId;
        }
        
        public String Permission_Name
        {
            get { return permissionName; }
            set { permissionName = value; }
        }

        public int Permission_Id
        {
            get { return permissionId; }
            set { permissionId = value; }
        }

        public int SupportedApplicationId
        {
            get { return supportedApplicationId; }
            set { supportedApplicationId = value; }
        }

    }
}
