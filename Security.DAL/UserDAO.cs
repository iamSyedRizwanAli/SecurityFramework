using Security.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DAL
{
    public class UserDAO
    {
        public static Boolean updateUser(User user)
        {
            String queryA = String.Format("delete from user_roles where user_id = {0}", user.User_ID),
                queryB = String.Format("delete from user_applications where user_id = {0}", user.User_ID),
                queryC = String.Format("update users set login = '{0}', password = '{1}' where user_id = {2}", user.LoginID, user.Password, user.User_ID),
                queryD = String.Format("update users_manipulation_record set isActive = {0}, lastmodifiedby = {1}, lastmodifiedon = '{2}' where user_id = {3}", user.IsActive ? 1 : 0, user.LastModifiedBy, user.LastModifiedOn.ToString(), user.User_ID);

            using (DBHelper helper = new DBHelper())
            {
                helper.ExecuteQuery(queryA);
                helper.ExecuteQuery(queryB);
                helper.ExecuteQuery(queryC);
                helper.ExecuteQuery(queryD);

                foreach (int appId in user.App_ID)
                {
                    queryA = String.Format("insert into user_applications(user_id, app_id) values ({0}, {1})", user.User_ID, appId);
                    helper.ExecuteQuery(queryA);
                }

                foreach (Role role in user.Roles)
                {
                    queryA = String.Format("insert into user_roles(user_id, role_id) values({0}, {1})", user.User_ID, role.Role_ID);
                    helper.ExecuteQuery(queryA);
                }

            }

            return true;

        }

        public static Boolean saveUser(User user)
        {

            String queryA = String.Format("insert into users(Login, password) values('{0}', '{1}')", user.LoginID, user.Password)
                , queryB = String.Format("select user_id from users where login like '{0}' and password like '{1}'", user.LoginID, user.Password);

            using (DBHelper helper = new DBHelper())
            {
                helper.ExecuteQuery(queryA);
                user.User_ID = getUserId(helper.ExecuteDataReader(queryB));
            }

            using(DBHelper helper = new DBHelper())
            {

                queryA = String.Format("insert into users_manipulation_record(user_id, isActive, CreatedOn, CreatedBy, LastModifiedBy, LastModifiedOn) values ({0}, {1}, '{2}', {3}, {4}, '{5}')", user.User_ID, user.IsActive ? 1 : 0, user.CreatedOn.ToString(), user.CreatedBy, user.LastModifiedBy, user.LastModifiedOn.ToString());
                helper.ExecuteQuery(queryA);

                foreach (Role r in user.Roles)
                {
                    queryA = String.Format("insert into user_roles(user_id, role_id) values ({0}, {1})", user.User_ID, r.Role_ID);
                    helper.ExecuteQuery(queryA);
                }

                foreach (int i in user.App_ID)
                {
                    queryA = String.Format("insert into user_applications(user_id, app_id) values({0}, {1})", user.User_ID, i);
                    helper.ExecuteQuery(queryA);
                }
            }

            return true;
        }

        private static int getUserId(SqlDataReader reader)
        {
            int id = -1;

            if (reader.Read())
                id = reader.GetInt32(0);

            return id;
        }

        public static List<User> getUserOfApplication(int app_id, List<Role> listOfRoles, List<Application> listOfApplications)
        {
            String query = "select u.user_id, u.Login, u.Password, m.IsActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Users u, Users_Manipulation_Record m where u.user_id = m.user_id and u.user_id in (select user_id from User_Applications where App_Id = " + app_id + ")";
            return costlyDataReadingCode(query, listOfRoles, listOfApplications);
        }

        public static List<User> getActiveUsers(List<Role> listOfRoles, List<Application> listOfApplications)
        {
            String query = "select u.user_id, u.Login, u.Password, m.IsActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Users u, Users_Manipulation_Record m where u.user_id = m.user_id and m.isActive = true";
            return costlyDataReadingCode(query, listOfRoles, listOfApplications);
        }

        private static List<User> costlyDataReadingCode(String query, List<Role> listOfRoles, List<Application> listOfApplications)
        {
            List<User> users = null;

            using (DBHelper helper = new DBHelper())
            {
                SqlDataReader reader = helper.ExecuteDataReader(query);
                users = createListOfUsersFromReader(reader, listOfRoles, listOfApplications);
            }

            return users;            
        }

        public static User getUser(int app_id, String login, String password)
        {
            User user = null;

            String query = "select u.user_id, u.Login, u.Password, m.IsActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Users u, Users_Manipulation_Record m where u.user_id = m.user_id and m.IsActive = 1 and u.Login like '" + login + "' and u.Password like '" + password + "' and u.User_Id in (select User_Id from User_Applications where app_id = " + app_id + " )";

            using (DBHelper helper = new DBHelper())
            {
                SqlDataReader reader = helper.ExecuteDataReader(query);
                user = makeUserOutOfReader(reader);
            }

            return user;
        }

        private static User makeUserOutOfReader(SqlDataReader reader)
        {
            User user = null;

            if (reader.Read())
            {
                int id = reader.GetInt32(0);
                String login = reader.GetString(1),
                    password = reader.GetString(2);
                Boolean activeFlag = reader.GetByte(3) == 1;        
                DateTime createdDate = reader.GetDateTime(4);
                int createdBy = reader.GetInt32(5);
                int lastModifiedBy = reader.GetInt32(6);
                DateTime lastModifiedDate = reader.GetDateTime(7);

                List<int> supportedAppId = ApplicationDAO.getAppIdForThisUser(id);
                List<Role> role = RoleDAO.getAllRolesForThisUser(id);

                user = new User(id, login, password, supportedAppId, role, activeFlag, createdDate, lastModifiedDate, createdBy, lastModifiedBy);
            }

            return user;
        }

        private static List<User> makeUsersOutOfReader(SqlDataReader reader)
        {
            List<User> user = new List<User>();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                String login = reader.GetString(1),
                    password = reader.GetString(2);
                Boolean activeFlag = reader.GetByte(3) == 1;
                DateTime createdDate = reader.GetDateTime(4);
                int createdBy = reader.GetInt32(5);
                int lastModifiedBy = reader.GetInt32(6);
                DateTime lastModifiedDate = reader.GetDateTime(7);

                List<int> supportedAppId = ApplicationDAO.getAppIdForThisUser(id);
                List<Role> role = RoleDAO.getAllRolesForThisUser(id);

                user.Add(new User(id, login, password, supportedAppId, role, activeFlag, createdDate, lastModifiedDate, createdBy, lastModifiedBy));
            }

            return user;
        }

        public static List<User> getAllUsers()
        {
            List<User> users = null;
            String query = "select u.user_id, u.Login, u.Password, m.IsActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Users u, Users_Manipulation_Record m where u.user_id = m.user_id";

            using (DBHelper helper = new DBHelper())
            {
                SqlDataReader reader = helper.ExecuteDataReader(query);
                users = makeUsersOutOfReader(reader);
            }

            return users;
        }

        public static List<User> createListOfUsersFromReader(SqlDataReader reader, List<Role> roles, List<Application> applications)
        {
            List<User> list = new List<User>();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                String login = reader.GetString(1),
                    password = reader.GetString(2);
                Boolean activeFlag = reader.GetBoolean(3);
                DateTime createdDate = reader.GetDateTime(4);
                int createdBy = reader.GetInt32(5);
                int lastModifiedBy = reader.GetInt32(6);
                DateTime lastModifiedDate = reader.GetDateTime(7);
 
                List<int> supportedAppId = ApplicationDAO.getAppIdForThisUser(id);
                List<int> userRole = RoleDAO.getRoleIdForThisUser(id);

                List<Role> role = getRoleFromList(roles, userRole);

                User user = new User(id, login, password, supportedAppId, role, activeFlag, createdDate, lastModifiedDate, createdBy, lastModifiedBy);
                list.Add(user);
            }

            return list;
        }

        private static List<Role> getRoleFromList(List<Role> roles, List<int> userRole)
        {
            List<Role> resRole = (from role in roles where userRole.Contains(role.Role_ID) select role).ToList<Role>();
            return resRole;
        }

        private static Application getApplicationFromList(List<Application> applications, int supportedAppId)
        {
            var resApp = from app in applications where app.App_ID == supportedAppId select app;
            return (Application)resApp;
        }

    }
}
