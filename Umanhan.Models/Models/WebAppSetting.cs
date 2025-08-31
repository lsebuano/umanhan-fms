using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class WebAppSetting
    {
        public string CognitoDomain { get; set; }
        public string CognitoUserPoolId { get; set; }
        public string CognitoClientId { get; set; }
        public string CognitoAuthority { get; set; }
        public string CognitoMetadataUrl { get; set; }
        public string CognitoResponseType { get; set; }
        public string CognitoRedirectUri { get; set; }
        public string CognitoReturnUrl { get; set; }
        public string WebApiUrlOperations { get; set; }
        public string WebApiUrlMasterdata { get; set; }
        public string WebApiUrlWeather { get; set; }
        public string WebApiUrlNlp { get; set; }
        public string WebApiUrlUsers { get; set; }
        public string WebApiUrlSecrets { get; set; }
        public string WebApiUrlLogger { get; set; }
        public string WebApiUrlReport { get; set; }
        public string WebApiUrlLogs { get; set; }
        public string WebApiUrlSettings { get; set; }
        public string GoogleMapsApiKey { get; set; }
        public string LogoutUrl => SetLogoutUrl();

        // global; being set in the components
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public string IsFarmSetupComplete { get; set; }
        public string IsFarmSetupStarted { get; set; }
        public double FarmLat { get; set; }
        public double FarmLng { get; set; }
        public bool IsLoaded { get; set; }

        private string SetLogoutUrl()
        {
            return $"https://{CognitoDomain}/logout" +
                $"?client_id={CognitoClientId}" +
                $"&logout_uri={CognitoReturnUrl}";
        }
    }
}
