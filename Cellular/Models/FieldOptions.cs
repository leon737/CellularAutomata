namespace Cellular.Models
{
    public class FieldOptions
    {
        public int TrackingRadius { get; set; }

        public int ActivationThreshold { get; set; }

        public double ActivationProbability { get; set; }

        public double MemoRecognizationThreshold { get; set; }

        public int TimeToRelax { get; set; }

        public int TimeToTakeSnapshot { get; set; }
    }
}