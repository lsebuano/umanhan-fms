
using Blazored.LocalStorage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Umanhan.Dtos.HelperModels
{
    public class WebAppSetting : INotifyPropertyChanged
    {
        private readonly ILocalStorageService _localStorage;
        private const string StorageKey = "WebAppSetting";

        public WebAppSetting() { }

        public WebAppSetting(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        // Cognito settings
        private string _cognitoDomain;
        public string CognitoDomain
        {
            get => _cognitoDomain;
            set => SetProperty(ref _cognitoDomain, value);
        }

        private string _cognitoClientId;
        public string CognitoClientId
        {
            get => _cognitoClientId;
            set => SetProperty(ref _cognitoClientId, value);
        }

        public string CognitoUserPoolId { get; set; }
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

        // Logout URL — recomputed dynamically
        public string LogoutUrl => SetLogoutUrl();

        // global; being set in the components
        private Guid _farmId;
        public Guid FarmId
        {
            get => _farmId;
            set => SetProperty(ref _farmId, value);
        }

        private string _farmName;
        public string FarmName
        {
            get => _farmName;
            set => SetProperty(ref _farmName, value);
        }

        private string _isFarmSetupComplete;
        public string IsFarmSetupComplete
        {
            get => _isFarmSetupComplete;
            set => SetProperty(ref _isFarmSetupComplete, value);
        }

        private string _isFarmSetupStarted;
        public string IsFarmSetupStarted
        {
            get => _isFarmSetupStarted;
            set => SetProperty(ref _isFarmSetupStarted, value);
        }

        private double _farmLat;
        public double FarmLat
        {
            get => _farmLat;
            set => SetProperty(ref _farmLat, value);
        }

        private double _farmLng;
        public double FarmLng
        {
            get => _farmLng;
            set => SetProperty(ref _farmLng, value);
        }

        private bool _isLoaded;
        public bool IsLoaded
        {
            get => _isLoaded;
            set => SetProperty(ref _isLoaded, value);
        }

        private string SetLogoutUrl()
        {
            return $"https://{CognitoDomain}/logout" +
                $"?client_id={CognitoClientId}" +
                $"&logout_uri={CognitoReturnUrl}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            _ = SaveAsync(); // fire and forget

            return true;
        }

        public async Task LoadAsync()
        {
            var saved = await _localStorage.GetItemAsync<WebAppSetting>(StorageKey);
            if (saved != null)
            {
                //FarmId = saved.FarmId; //<-- don't overwrite FarmId
                FarmName = saved.FarmName;
                FarmLat = saved.FarmLat;
                FarmLng = saved.FarmLng;
                IsLoaded = saved.IsLoaded;
            }
        }

        private async Task SaveAsync()
        {
            await _localStorage.SetItemAsync(StorageKey, this);
        }
    }
}
