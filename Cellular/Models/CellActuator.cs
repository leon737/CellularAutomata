using System;

namespace Cellular.Models
{
    public class CellActuator : ICellActuator
    {

        private readonly int _activationThreshold;

        private readonly double _activationProbability;

        private readonly double _memoRecognizationThreshold;

        public CellActuator(int activationThreshold, double activationProbability, double memoRecognizationThreshold)
        {
            _activationThreshold = activationThreshold;
            _activationProbability = activationProbability;
            _memoRecognizationThreshold = memoRecognizationThreshold;
        }

        public bool Actuate(ITrackingField trackingField)
            => trackingField.GetNumberOfActiveCells() > _activationThreshold;

        public bool RecognizeMemo(ITrackingField trackingField, PatternMemo memo) => trackingField.GetPatternSimilarity(memo) > _memoRecognizationThreshold;

        public bool TestActuate() => new Random().NextDouble() > 1.0 - _activationProbability;
    }
}