//using Blazored.SessionStorage;
//using Blazored.SessionStorage;
using System.Text.Json;
using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class SystemSettingService
    {
        private readonly ApiService _apiService;
        private bool _isLoaded = false;
        private List<SystemSettingDto> _systemSettings;

        private Dictionary<string, JsonElement> Settings { get; set; } = [];

        public SystemSettingService(ApiService apiService, List<SystemSettingDto> systemSettings)
        {
            _apiService = apiService;
            _systemSettings = systemSettings;
        }

        public Task<ApiResponse<IEnumerable<SystemSettingDto>>> GetAllSystemSettingsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<SystemSettingDto>>("SettingsAPI", "api/system-settings");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<SystemSettingDto>> GetSystemSettingByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<SystemSettingDto>("SettingsAPI", $"api/system-settings/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<SystemSettingDto>> GetSystemSettingByNameAsync(string name)
        {
            try
            {
                return _apiService.GetAsync<SystemSettingDto>("SettingsAPI", $"api/system-settings/name/{name}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SystemSettingDto> CreateSystemSettingAsync(SystemSettingDto systemSetting)
        {
            try
            {
                var response = await _apiService.PostAsync<SystemSettingDto, SystemSettingDto>("SettingsAPI", "api/system-settings", systemSetting).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<SystemSettingDto> UpdateSystemSettingAsync(SystemSettingDto systemSetting)
        {
            try
            {
                var response = await _apiService.PutAsync<SystemSettingDto, SystemSettingDto>("SettingsAPI", $"api/system-settings/{systemSetting.SettingId}", systemSetting).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<SystemSettingDto> DeleteSystemSettingAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<SystemSettingDto>("SettingsAPI", $"api/system-settings/{id}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async System.Threading.Tasks.Task LoadSystemSettings()
        {
            if (_isLoaded)
                return;

            try
            {
                var result = await GetAllSystemSettingsAsync().ConfigureAwait(false);
                if (result.IsSuccess && result.Data?.Any() == true)
                {
                    Settings = result.Data.ToDictionary(x =>
                        x.SettingName,
                        x => JsonSerializer.SerializeToElement(x.SettingValue)
                    );
                    _isLoaded = true;
                    Console.WriteLine("System settings loaded successfully.");
                }
                else
                {
                    Console.WriteLine("System settings load result is empty or unsuccessful.");
                }
            }
            catch (Exception ex)
            {
                _isLoaded = false;
                Console.WriteLine($"[LoadSystemSettings] Failed: {ex.Message}");
            }
        }

        public T? GetSetting<T>(string key)
        {
            if (!_isLoaded)
            {
                Console.WriteLine("Settings not loaded.");
                return default;
            }

            if (!Settings.TryGetValue(key, out var element))
            {
                Console.WriteLine($"Key '{key}' not found in settings.");
                return default;
            }

            try
            {
                if (typeof(T).IsEnum && element.ValueKind == JsonValueKind.String)
                {
                    var enumString = element.GetString();
                    return (T)Enum.Parse(typeof(T), enumString!, ignoreCase: true);
                }

                if (typeof(T) == typeof(Guid) && element.ValueKind == JsonValueKind.String)
                {
                    var str = element.GetString();
                    if (Guid.TryParse(str, out var parsed))
                        return (T)(object)parsed;

                    Console.WriteLine($"Invalid GUID format: {str}");
                    return default;
                }

                return element.Deserialize<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization failed for key '{key}': {ex.Message}");
                return default;
            }
        }

        //public async System.Threading.Tasks.Task SetSetting(string key, string value)
        //{
        //    if (!_isLoaded)
        //        return;

        //    Settings[key] = JsonSerializer.SerializeToElement(value);
        //    var json = JsonSerializer.Serialize(Settings);
        //    await _sessionStorage.SetItemAsync("SystemSettings", json).ConfigureAwait(false);
        //}

        //public async System.Threading.Tasks.Task ClearSettings()
        //{
        //    _isLoaded = false;
        //    Settings.Clear();
        //    await _sessionStorage.RemoveItemAsync("SystemSettings");
        //}

        //public async System.Threading.Tasks.Task ReloadSettingsAsync()
        //{
        //    await ClearSettings().ConfigureAwait(false);
        //    await LoadSystemSettings().ConfigureAwait(false);
        //}
    }
}
