using System.Threading.Tasks;

namespace Savvy.Services.Session
{
    public interface ISessionService
    {
        string DropboxAccessToken { get; }
        long DropboxUserId { get; }

        void UserLoggedIn(string accessToken, long dropboxUserId);
        void ClearState();

        Task SuspendStateAsync();
        Task<bool> ResumeStateAsync();
    }
}