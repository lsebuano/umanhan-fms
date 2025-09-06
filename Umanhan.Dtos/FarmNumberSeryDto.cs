namespace Umanhan.Dtos
{
    public class FarmNumberSeryDto
    {
        public Guid NumberSeryId { get; set; }

        public Guid FarmId { get; set; }

        public string FarmName { get; set; }

        public string Type { get; set; }

        public int CurrentSery { get; set; }

        public string Format { get; set; }
    }
}
