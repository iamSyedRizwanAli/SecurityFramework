using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DAL
{
    public static class SecurityClient
    {

        public static User validateUser(int app_id, String login, String password)
        {
            User loadedUser = null;

            loadedUser = UserDAO.getUser(app_id, login, password);

            return loadedUser == null ? null : loadedUser.IsActive ? loadedUser : null;
            
        }

    }
}
