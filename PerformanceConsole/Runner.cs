using System;
using System.Diagnostics;
using Cellular.Models;

namespace PerformanceConsole
{
    public class Runner
    {
        private FieldOptions _fieldOptions;
        private Field _field;

        public Runner()
        {
            _fieldOptions = new FieldOptions
            {
                ActivationThreshold = 7,
                ActivationProbability = 0.3,
                MemoRecognizationThreshold = 0.5,
                TrackingRadius = 5,
                TimeToRelax = 50,
                TimeToTakeSnapshot = 3
            };

            _field = new Field(500, 500, _fieldOptions);

            _field.InitWithZeroCells();

            for (int x = 248; x < 253; x++)
            {
                for (int y = 248; y < 253; y++)
                {
                    var trackingField = new TrackingField(_fieldOptions.TrackingRadius, x, y, _field.GetCellState);
                    var actuator = new CellActuator(_fieldOptions.ActivationThreshold, new Random().NextDouble(), _fieldOptions.MemoRecognizationThreshold);
                    var cell = new Cell(trackingField, actuator, _fieldOptions.TimeToRelax, _fieldOptions.TimeToTakeSnapshot, CellStates.Active);
                    _field.SetCell(x, y, cell);
                }
            }
        }

        public void Run()
        {
            Console.WriteLine("Starting run...");
            int numberOfIterationsToRun = 10;
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < numberOfIterationsToRun; ++i)
            {
                _field.RunIteration();
            }
            var elapsed = sw.ElapsedMilliseconds;
            Console.WriteLine($"Total time: {elapsed} ms");
            Console.WriteLine($"Total per iteration: {((double)elapsed / numberOfIterationsToRun):0.00} ms");
        }
    }
}