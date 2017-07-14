using Security.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DAL
{
    public static class RoleDAO
    {

        public static Boolean updateRole(Role role)
        {
            String queryA = String.Format("delete from role_permissions where role_id = {0}", role.Role_ID),
                queryB = String.Format("update roles set role_name = '{0}' where role_id = {1}", role.Role_Name, role.Role_ID),
                queryC = String.Format("update roles_manipulation_record set isActive = {0}, lastmodifiedby = {1}, lastmodifiedon = '{2}' where role_id = {3}", role.IsActive?1:0, role.LastModifiedBy, role.LastModifiedOn.ToString(), role.Role_ID);

            using (DBHelper helper = new DBHelper())
            {
                helper.ExecuteQuery(queryA);
                helper.ExecuteQuery(queryB);
                helper.ExecuteQuery(queryC);

                foreach (Permission p in role.Permissions)
                {
                    queryA = String.Format("insert into role_permissions(role_id, permission_id) values({0}, {1})", role.Role_ID, p.Permission_Id);
                    helper.ExecuteQuery(queryA);
                }

            }

            return true ;
        }

        public static Boolean saveRole(Role role)
        {
            String queryA = String.Format("Insert into roles(role_name, app_id) values ('{0}', {1})", role.Role_Name, role.SupportedApplicationId),
                queryB = String.Format("select role_id from roles where role_name like '{0}' and app_id = {1}", role.Role_Name, role.SupportedApplicationId);

            using (DBHelper helper = new DBHelper())
            {
                helper.ExecuteQuery(queryA);
                role.Role_ID = extractRoleId(helper.ExecuteDataReader(queryB));
            }

            using(DBHelper helper = new DBHelper())
            {
                String queryC = String.Format("insert into roles_manipulation_record(role_id, isActive, createdOn, createdBy, lastModifiedby, lastModifiedOn) values ({0}, {1}, '{2}', {3}, {4}, '{5}')", role.Role_ID, role.IsActive ? "1" : "0", role.CreatedOn.ToString(), role.CreatedBy, role.CreatedBy, role.LastModifiedOn.ToString());
                helper.ExecuteQuery(queryC);

                foreach (Permission p in role.Permissions)
                {
                    String queryD = String.Format("insert into role_permissions(role_id, permission_id) values({0}, {1})", role.Role_ID, p.Permission_Id);
                    helper.ExecuteQuery(queryD);
                }

            }

            return true;
        }

        private static int extractRoleId(SqlDataReader reader)
        {
            int id = 0;
        
            if (reader.Read())
                id = reader.GetInt32(0);

            return id;
        }

        public static List<Role> getAllRoles()
        {
            String query = "select r.Role_Id, r.Role_Name, r.App_Id, m.isActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Roles r, Roles_Manipulation_Record m where r.role_id = m.role_id";
            return dataReadingCode(query);
        }

        public static List<Role> getAllRolesForApplication(int app_id)
        {
            String query = "select r.Role_Id, r.Role_Name, r.App_Id, m.isActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Roles r, Roles_Manipulation_Record m where r.role_id = m.role_id and r.App_Id = " + app_id;
            return dataReadingCode(query);
        }

        public static List<Role> getAllRolesForThisUser(int user_id)
        {
            String query = "select r.Role_Id, r.Role_Name, r.App_Id, m.isActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Roles r, Roles_Manipulation_Record m where r.role_id = m.role_id and r.role_id in (select role_id from user_roles where user_id = " + user_id + ")";
            return dataReadingCode(query);
        }

        public static DataTable getAllRolesForGrid()
        {
            String query = "select r.Role_Id, r.Role_Name, r.App_Id, m.isActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Roles r, Roles_Manipulation_Record m where r.role_id = m.role_id";

            DataTable table = null;

            using (DBHelper helper = new DBHelper())
            {
                table = helper.getDataTable(query);
            }

            return table;

        }

        private static List<Role> dataReadingCode(String query)
        {
            List<Role> roles = null;

            using (DBHelper helper = new DBHelper())
            {
                SqlDataReader reader = helper.ExecuteDataReader(query);
                roles = convertReaderToList(reader);
            }

            return roles;
        }

        public static List<int> getRoleIdForThisUser(int user_id)
        {
            List<int> id = new List<int>();
            String query = "select role_id from user_roles where user_id = " + user_id;

            using (DBHelper helper = new DBHelper())
            {
                SqlDataReader reader = helper.ExecuteDataReader(query);

                while (reader.Read())
                    id.Add(reader.GetInt32(0));
            }

            return id;
        }
   

        private static List<Role> convertReaderToList(SqlDataReader reader)
        {
            List<Role> list = new List<Role>();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                String name = reader.GetString(1);
                int sAppId = reader.GetInt32(2);
                Boolean activeFlag = reader.GetByte(3) == 1;
                DateTime createdDate = reader.GetDateTime(4);
                int createdBy = reader.GetInt32(5);
                int lastModifiedBy = reader.GetInt32(6);
                DateTime lastModifiedDate = reader.GetDateTime(7);

                List<Permission> listOfPermissions = PermissionDAO.getAllPermissionsForRole(id);

                Role role = new Role(id, name, sAppId, listOfPermissions, activeFlag, createdDate, lastModifiedDate, createdBy, lastModifiedBy);
                list.Add(role);
            }

            return list;
        }

    }
}
