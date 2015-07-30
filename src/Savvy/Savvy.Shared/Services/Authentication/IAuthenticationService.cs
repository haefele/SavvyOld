using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Savvy.Services.Authentication
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Starts the login process.
        /// </summary>
        Task LoginAsync();
        /// <summary>
        /// Starts the logout process.
        /// </summary>
        Task LogoutAsync();

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Continues the login process.
        /// </summary>
        /// <param name="args">The <see cref="WebAuthenticationBrokerContinuationEventArgs"/> instance containing the event data.</param>
        Task ContinueLoginAsync(WebAuthenticationBrokerContinuationEventArgs args);
#endif
    }
}
