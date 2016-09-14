using System;
using System.Threading.Tasks;

namespace Cellular.Models
{
    public class Field
    {
        public int Width { get; }

        public int Height { get; }

        private Cell[,] _cells;

        private readonly FieldOptions _options;

        private long cnt = 0;

        public Field(int width, int height, FieldOptions options)
        {
            Width = width;
            Height = height;
            _cells = new Cell[Width, Height];
            _options = options;
        }

        public void InitWithZeroCells()
        {
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                {
                    var trackingField = new TrackingField(_options.TrackingRadius, i, j, GetCellState);
                    var actuator = new CellActuator(_options.ActivationThreshold, new Random().NextDouble(), _options.MemoRecognizationThreshold);
                    var cell = new Cell(trackingField, actuator, _options.TimeToRelax, _options.TimeToTakeSnapshot);
                    _cells[i, j] = cell;
                }
        }

        public void SetCell(int x, int y, Cell cell)
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            _cells[x, y] = cell;
        }

        public void RunIteration()
        {
            cnt = 0;
            var binaryField = MakeBinaryField();

            Parallel.For(0, Width, i =>
            {
                for (int j = 0; j < Height; ++j)
                    _cells[i, j] = _cells[i, j].Act(binaryField, Width, Height);
                    //_cells[i, j] = _cells[i, j].Act(null, 0, 0);
            });
        }

        private CellStates[] MakeBinaryField()
        {
            var field = new CellStates[Width*Height];
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                {
                    var state = _cells[x, y]._state;
                    field[y * Width + x] = state;
                }
            return field;
        }

        public CellStates GetCellState(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return CellStates.Inactive;
            cnt++;
            return _cells[x, y]._state;
        }
    }
}
