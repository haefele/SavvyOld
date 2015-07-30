using System;

namespace Savvy.Services.Settings
{
    public class HardCodedSettings : ISettings
    {
        public string DropboxClientId => "ef2iibv3k4anhvk";

        public string DropboxClientSecret => "5igqchrpl66ijoh";
        
        // ReSharper disable once ExceptionNotDocumented
        public Uri RedirectUrl => new Uri("https://www.dropbox.com/1/oauth2/redirect_receiver");
    }
}