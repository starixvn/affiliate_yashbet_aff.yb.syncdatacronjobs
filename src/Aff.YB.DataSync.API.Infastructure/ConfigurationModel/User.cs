using System;

namespace Aff.YB.DataSync.API.Infastructure.ConfigurationModel
{
    [Serializable]
    public class User
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

}
