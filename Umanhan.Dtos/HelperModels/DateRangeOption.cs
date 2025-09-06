namespace Umanhan.Dtos.HelperModels
{
    public class DateRangeOption(string text, string value)
    {
        private readonly string _text = text;
        private readonly string _value = value;

        public string Text => _text;
        public string Value => _value;
    }
}
