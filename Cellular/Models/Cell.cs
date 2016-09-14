using System;
using System.Collections.Generic;
using System.Linq;

namespace Cellular.Models
{
    public class Cell
    {
        public CellStates _state;

        private readonly List<PatternMemo> _patternMemos;

        private readonly int _timeToRelax;

        private int? _remainingTimeToRelax;

        private readonly int _timeToTakeSnapshot;

        private int? _remainingTimeToTakeSnapshot;

        private readonly ITrackingField _trackingField;

        private readonly ICellActuator _cellActuator;

        public Cell(ITrackingField trackingField, ICellActuator cellActuator, int timeToRelax, int timeToTakeSnapshot, CellStates state = CellStates.Inactive)
        {
            _state = state;
            _patternMemos = new List<PatternMemo>();

            _trackingField = trackingField;
            _cellActuator = cellActuator;
            _timeToRelax = timeToRelax;
            _timeToTakeSnapshot = timeToTakeSnapshot;

            if(state == CellStates.Active || state == CellStates.Negative)
            {
                _remainingTimeToRelax = _timeToRelax;
                _remainingTimeToTakeSnapshot = _timeToTakeSnapshot;
            }
        }

        //private Cell(Cell cell, CellStates newState) : this(cell._trackingField, cell._cellActuator, cell._timeToRelax, cell._timeToTakeSnapshot, newState)
        //{
        //    _patternMemos = cell._patternMemos;

        //    if (newState == CellStates.Relaxed || newState == CellStates.RelaxedNegative)
        //    {
        //        _remainingTimeToRelax = cell._remainingTimeToRelax - 1;
        //        _remainingTimeToTakeSnapshot = cell._remainingTimeToTakeSnapshot - 1;
        //    }
        //    if (_remainingTimeToRelax == 0)
        //        _remainingTimeToRelax = null;
        //    if (_remainingTimeToTakeSnapshot == 0)
        //    {
        //        _patternMemos.Add(TakeSnapshot());
        //        _remainingTimeToTakeSnapshot = null;
        //    }
        //}

        public CellStates State => _state;

        public Cell Act(CellStates[] binaryField, int width, int height)
        {
            //return new Cell(this, EvaluateNewState(binaryField, width, height));
            return SelfModify(EvaluateNewState(binaryField, width, height));
        }

        public Cell SelfModify(CellStates newState)
        {
            if (newState == CellStates.Active || newState == CellStates.Negative)
            {
                _remainingTimeToRelax = _timeToRelax;
                _remainingTimeToTakeSnapshot = _timeToTakeSnapshot;
            }

            if (newState == CellStates.Relaxed || newState == CellStates.RelaxedNegative)
            {
                _remainingTimeToRelax--;
                _remainingTimeToTakeSnapshot--;
            }
            if (_remainingTimeToRelax == 0)
                _remainingTimeToRelax = null;
            if (_remainingTimeToTakeSnapshot == 0)
            {
                 _patternMemos.Add(TakeSnapshot());
                _remainingTimeToTakeSnapshot = null;
            }

            _state = newState;
            return this;
        }

        private CellStates EvaluateNewState(CellStates[] binaryField, int width, int height)
        {
            if (_state == CellStates.ReactivePositive || _state == CellStates.ReactiveNegative) return CellStates.Inactive;
            if (_state == CellStates.Active) return CellStates.Relaxed;
            if (_state == CellStates.Negative) return CellStates.RelaxedNegative;
            if ((_state == CellStates.Relaxed || State == CellStates.RelaxedNegative) && _remainingTimeToRelax == null) return CellStates.Inactive;
            if (_state == CellStates.Relaxed || _state == CellStates.RelaxedNegative) return _state;

            _trackingField.SetBinaryField(binaryField, width, height);

            foreach (var memo in _patternMemos)
            {
                if (_cellActuator.RecognizeMemo(_trackingField, memo))
                    return memo.State;
            }

            if (_cellActuator.Actuate(_trackingField))
                return _cellActuator.TestActuate() ? CellStates.Active : CellStates.Negative;

            return CellStates.Inactive;
        }

        private PatternMemo TakeSnapshot() => _trackingField.TakeMemo(_state);
    }
}
