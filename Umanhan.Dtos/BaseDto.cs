using System.Text.Json;
using System.Text.Json.Serialization;

namespace Umanhan.Dtos
{
    public abstract class BaseDto
    {
        public override string ToString()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // optional
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles // avoids circular reference issues
            };

            return JsonSerializer.Serialize(this, this.GetType(), options);
        }
    }
}
