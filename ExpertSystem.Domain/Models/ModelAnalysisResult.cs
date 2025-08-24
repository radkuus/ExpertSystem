namespace ExpertSystem.Domain.Models
{
    using System.Text.Json.Serialization;

    public class ModelAnalysisResult
    {
        [JsonPropertyName("f1")]
        public double F1 { get; set; }

        [JsonPropertyName("precision")]
        public double Precision { get; set; }

        [JsonPropertyName("recall")]
        public double Recall { get; set; }

        [JsonPropertyName("accuracy")]
        public double Accuracy { get; set; }

        public string ModelName { get; set; }
    }


}