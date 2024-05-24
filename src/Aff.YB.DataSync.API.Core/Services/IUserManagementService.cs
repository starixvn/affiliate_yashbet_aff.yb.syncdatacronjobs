namespace Aff.YB.DataSync.API.Core.Services
{
    public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
    }
}
