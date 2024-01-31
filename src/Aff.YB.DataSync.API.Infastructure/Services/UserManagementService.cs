using Aff.YB.DataSync.API.Core.Services;
using Aff.YB.DataSync.API.Infastructure.ConfigurationModel;
using Aff.YB.DataSync.API.Infastructure.Extentions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Aff.YB.DataSync.API.Infastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IEnumerable<User> _users;

        public UserManagementService(IOptions<List<User>> userListAccessor)
        {
            _users = userListAccessor.Value;
        }

        public bool IsValidUser(string username, string password)
            => _users.Any(x => x.UserName.Equals(username)
            && x.Password.Equals(password.Sha256()));

    }
}
