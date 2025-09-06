namespace Umanhan.Dtos.HelperModels
{
    public class AnalyzeDataRequest
    {
        public string ConvoId { get; set; }
        public string Prompt { get; set; }
        public dynamic Data { get; set; }
    }
}
