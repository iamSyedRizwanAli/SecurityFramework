using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class ManipulationRecordObject
    {
        //datamembers from manipulation record table
        
        private Boolean isActive;
        private DateTime createdOn, lastModifiedOn;
        private int createdBy, lastModifiedBy;

        public ManipulationRecordObject(Boolean isActive, DateTime createdOn, DateTime lastModifiedOn, int createdBy, int lastModifiedBy)
        {
            this.isActive = isActive;
            this.createdOn = createdOn;
            this.createdBy = createdBy;
            this.lastModifiedOn = lastModifiedOn;
            this.lastModifiedBy = lastModifiedBy;
        }

        // getter setter from datamembers from manipulation record table

        public Boolean IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        public DateTime LastModifiedOn
        {
            get { return lastModifiedOn; }
            set { lastModifiedOn = value; }
        }

        public int CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        public int LastModifiedBy
        {
            get { return lastModifiedBy; }
            set { lastModifiedBy = value; }
        }


    }
}
