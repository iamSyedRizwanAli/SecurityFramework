using Security.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DAL
{
    public static class ApplicationDAO
    {

        public static Boolean updateApplication(Application app)
        {
            String query = String.Format("update Applications_manipulation_record set isActive = {0}, LastModifiedBy = {1}, LastModifiedOn = '{2}' where App_id = {3}", app.IsActive == true ? "1" : "0", app.LastModifiedBy, app.LastModifiedOn.ToString(), app.App_ID);

            using (DBHelper helper = new DBHelper())
            {
                helper.ExecuteQuery(query);
            }

            return true;
        }

        public static Boolean saveApplication(Application app)
        {
            String queryA = String.Format("insert into Applications(app_name) values('{0}')", app.Application_Name),
                queryB = String.Format("select app_id from Applications where app_name like '{0}'", app.Application_Name);

            using (DBHelper helper = new DBHelper())
            {
                helper.ExecuteQuery(queryA);
                app.App_ID = getIdFromReader(helper.ExecuteDataReader(queryB));
            }

            using (DBHelper helper = new DBHelper())
            {
                queryA = String.Format("insert into applications_manipulation_record(app_id, isActive, CreatedOn, CreatedBy, LastModifiedBy, LastModifiedOn) values ({0}, {1}, '{2}', {3}, {4}, '{5}')", app.App_ID, app.IsActive ? 1 : 0, app.CreatedOn.ToString(), app.CreatedBy, app.LastModifiedBy, app.LastModifiedOn.ToString());
                helper.ExecuteQuery(queryA);
            }

            return true;

        }

        private static int getIdFromReader(SqlDataReader reader)
        {
            int id = -1;

            if (reader.Read())
                id = reader.GetInt32(0);

            return id;

        }

        public static List<Application> getAllApplicationRecords()
        {
            String query = "Select a.App_id, a.App_name, e.IsActive, e.CreatedOn, e.CreatedBy, e.LastModifiedBy, e.LastModifiedOn from Applications a, Applications_Manipulation_Record e where a.App_Id = e.App_Id";
            List<Application> list = null;

            using(DBHelper helper = new DBHelper())
            {
                SqlDataReader reader = helper.ExecuteDataReader(query);
                list = convertReaderToList(reader);
            }

            return list;
        }

        public static List<int> getAppIdForThisUser(int user_id)
        {
            List<int> id = new List<int>();

            String query = "Select app_id from user_applications where user_id = " + user_id;

            using (DBHelper helper = new DBHelper())
            {
                SqlDataReader reader = helper.ExecuteDataReader(query);
                
                while (reader.Read())
                    id.Add(reader.GetInt32(0));
            }

            return id;
        }

        public static Application getApplicationOfID(int id)
        {
            Application application = null;

            String query = "Select a.App_id, a.App_name, e.IsActive, e.CreatedOn, e.CreatedBy, e.LastModifiedBy, e.LastModifiedOn from Application a, Applications_Manipulation_Record e where a.App_Id = e.App_Id and a.App_Id = " + id;
            SqlDataReader reader;

            using (DBHelper helper = new DBHelper())
            {
                reader = helper.ExecuteDataReader(query);
                application = getDesiredApplicationFromReader(reader);
            }

            return application;
        }



        private static Application getDesiredApplicationFromReader(SqlDataReader reader)
        {
            Application app = null;
            
            if (reader.Read())
            {
                int id = reader.GetInt32(0);
                String name = reader.GetString(1);
                Boolean isActive = reader.GetBoolean(2);
                DateTime creationDate = reader.GetDateTime(3);
                int createdBy = reader.GetInt32(4);
                int lastModifiedBy = reader.GetInt32(5);
                DateTime lastModifiedOn = reader.GetDateTime(6);

                app = new Application(id, name, null, isActive, creationDate, lastModifiedOn, createdBy, lastModifiedBy);
            }

            return app;

        }

        private static List<Application> convertReaderToList(SqlDataReader reader)
        {
            List<Application> list = new List<Application>();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                String name = reader.GetString(1);
                Boolean isActive = reader.GetByte(2) == 1;
                DateTime creationDate = reader.GetDateTime(3);
                int createdBy = reader.GetInt32(4);
                int lastModifiedBy = reader.GetInt32(5);
                DateTime lastModifiedOn = reader.GetDateTime(6);

                List<Role> roles = RoleDAO.getAllRolesForApplication(id);

                Application app = new Application(id, name, roles, isActive, creationDate, lastModifiedOn, createdBy, lastModifiedBy);
                list.Add(app);
            }

            return list;
        }

    }
}
