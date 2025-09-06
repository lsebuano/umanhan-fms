using System.Net;
using System.Text.Json;

namespace Umanhan.Dtos.HelperModels
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public string ErrorTitle { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }

        public static ApiResponse<T> Success(T data) => new()
        {
            IsSuccess = true,
            StatusCode = (int)HttpStatusCode.OK,
            Data = data
        };

        public static ApiResponse<T> Failure(string title, string errorMessage, Dictionary<string, List<string>>? validationErrors = null) => new()
        {
            IsSuccess = false,
            ErrorTitle = title,
            ErrorMessage = errorMessage,
            Errors = validationErrors
        }; 
        
        public override string ToString()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true // for pretty-print
            };
            return JsonSerializer.Serialize(this, options);
        }
    }
}
