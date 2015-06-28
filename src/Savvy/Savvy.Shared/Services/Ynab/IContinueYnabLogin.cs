using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Savvy.Services.Ynab
{
    internal interface IContinueYnabLogin
    {
        Task ContinueAsync(WebAuthenticationBrokerContinuationEventArgs args);
    }
}