using Security.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DAL
{
    public static class PermissionDAO
    {
        public static Boolean savePermission(Permission permission)
        {
            String queryA = String.Format("insert into permissions(permission_Name, app_id) values ('{0}', {1})", permission.Permission_Name, permission.SupportedApplicationId),
                queryB = String.Format ("select permission_id from permissions where permission_name like '{0}' and app_id = {1}", permission.Permission_Name, permission.SupportedApplicationId);

            using (DBHelper helper = new DBHelper())
            {
                helper.ExecuteQuery(queryA);
                permission.Permission_Id = extractIdFromReader(helper.ExecuteDataReader(queryB));
            }

            using (DBHelper helper = new DBHelper())
            {
                queryA = String.Format("insert permissions_manipulation_record(permission_id, isActive, CreatedOn, CreatedBy, LastModifiedBy, LastModifiedOn) values ({0}, {1}, '{2}', {3}, {4}, '{5}')", permission.Permission_Id, permission.IsActive ? 1 : 0, permission.CreatedOn.ToString(), permission.CreatedBy, permission.LastModifiedBy, permission.LastModifiedOn.ToString());
                helper.ExecuteQuery(queryA);
            }

            return true;
        }

        private static int extractIdFromReader(SqlDataReader reader)
        {
            int id = -1;

            if (reader.Read())
                id = reader.GetInt32(0);

            return id;
        }

        public static Boolean updatePermission(Permission perm)
        {
            String query = String.Format("update permissions_manipulation_record set isActive = {0}, LastModifiedBy = {1}, LastModifiedOn = '{2}' where permission_id = {3}", perm.IsActive == true ? "1" : "0", perm.LastModifiedBy, perm.LastModifiedOn.ToString(), perm.Permission_Id);

            using (DBHelper helper = new DBHelper())
            {
                helper.ExecuteQuery(query);
            }

            return true;
        }

        public static List<Permission> getAllPermissions()
        {
            String query = "select p.Permission_Id, p.Permission_Name, p.App_Id, m.IsActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Permissions p, Permissions_Manipulation_Record m where p.Permission_Id = m.Permission_Id";
            return DataReadingCode(query);
        }

        public static List<Permission> getAllPermissionsForApplication(int app_id)
        {
            String query = "select p.Permission_Id, p.Permission_Name, p.App_Id, m.IsActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Permissions p, Permissions_Manipulation_Record m where p.Permission_Id = m.Permission_Id and p.App_Id = " + app_id;
            return DataReadingCode(query);
        }

        public static List<Permission> getAllPermissionsForRole(int role_id)
        {
            String query = "select p.Permission_Id, p.Permission_Name, p.App_Id, m.IsActive, m.CreatedOn, m.CreatedBy, m.LastModifiedBy, m.LastModifiedOn from Permissions p, Permissions_Manipulation_Record m where p.Permission_Id = m.Permission_Id and p.Permission_Id in (select permission_id from Role_permissions where role_id = " + role_id + ")";
            return DataReadingCode(query);
        }

        private static List<Permission> DataReadingCode(String query)
        {
            List<Permission> list = null;

            using (DBHelper helper = new DBHelper())
            {
                SqlDataReader reader = helper.ExecuteDataReader(query);
                list = convertReaderIntoList(reader);
            }

            return list;
        }

        private static List<Permission> convertReaderIntoList(SqlDataReader reader)
        {
            List<Permission> perms = new List<Permission>();

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

                Permission perm = new Permission(id, name, sAppId, activeFlag, createdDate, lastModifiedDate, createdBy, lastModifiedBy);
                perms.Add(perm);
            }

            return perms;
        }

    }
}
