using System.Collections.Generic;

namespace UrGuide.Model.Admin
{
    /// <summary>
    /// Model for updating user roles
    /// </summary>
    public class UpdateUserRolesModel
    {
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
    }
}
