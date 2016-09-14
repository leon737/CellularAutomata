using System;
using System.Collections.Generic;
using System.Linq;

namespace Cellular.Models
{
    public class TrackingField : ITrackingField
    {
        private readonly int _radius;

        private readonly int _x;

        private readonly int _y;

        private CellStates[] _binaryField;

        private int _width;

        private int _height;

        //private readonly Func<int, int, CellStates> _getCellStateFunc;
        public delegate CellStates GetCellStateFuncDlg(int x, int y);
        private readonly GetCellStateFuncDlg _getCellStateFunc;

        public TrackingField(int radius, int x, int y, GetCellStateFuncDlg getCellStateFunc)
        {
            _radius = radius;
            _x = x;
            _y = y;
            _getCellStateFunc = getCellStateFunc;
        }

        public int GetNumberOfActiveCells()
        {
            int count = 0;
            for (int y = -_radius; y <= _radius; ++y)
                for (int x = -_radius; x <= _radius; ++x)

                {
                    var localState =  _binaryField != null ? 
                        GetFromBinaryField(_x + x, _y + y) : _getCellStateFunc(_x + x, _y + y);
                    if (localState == CellStates.Active || localState == CellStates.ReactivePositive)
                        count++;
                }
            return count;
        }
        private CellStates GetFromBinaryField(int x, int y)
        {
            return x < 0 || y < 0 || x >= _width || y >= _height ? CellStates.Inactive : _binaryField[y*_width + x];
        }

        public double GetPatternSimilarity(PatternMemo memo) =>
            (double)memo.GetNeighbourStates()
            .Select(v => _binaryField != null ?
                GetFromBinaryField(_x + v.Dx, _y + v.Dy) : _getCellStateFunc(_x + v.Dx, _y + v.Dy))
            .Count(x => x == CellStates.Active || x == CellStates.ReactivePositive)
            / memo.GetNeighbourStates().Count();

        public PatternMemo TakeMemo(CellStates state) => 
            new PatternMemo(GetTrackingCoordinates()
                .Select(v => new { pos = v, state = _binaryField != null ?
                    GetFromBinaryField(_x + v.Dx, _y + v.Dy) : _getCellStateFunc(_x + v.Dx, _y + v.Dy) })
            .Where(x => x.state == CellStates.Relaxed).Select(v => v.pos).ToList(), state);

        public void SetBinaryField(CellStates[] binaryField, int width, int height)
        {
            _binaryField = binaryField;
            _width = width;
            _height = height;
        }

        private IEnumerable<Neighbour> GetTrackingCoordinates()
        {
            for (int x = -_radius; x <= _radius; ++x)
                for (int y = -_radius; y <= _radius; ++y)
                    yield return new Neighbour(x, y);
        }
    }
}