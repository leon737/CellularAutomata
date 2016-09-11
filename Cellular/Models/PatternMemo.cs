using System.Collections.Generic;

namespace Cellular.Models
{
    public class PatternMemo
    {
        private readonly List<Neighbour> _neighbourStates;

        private readonly CellStates _state;

        public PatternMemo(List<Neighbour> neighbourStates, CellStates state)
        {
            _neighbourStates = neighbourStates;
            _state = state == CellStates.Relaxed ? CellStates.ReactivePositive : CellStates.ReactiveNegative;
        }

        public IEnumerable<Neighbour> GetNeighbourStates() => _neighbourStates;

        public CellStates State => _state;
    }
}