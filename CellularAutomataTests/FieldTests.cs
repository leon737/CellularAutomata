using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cellular.Models;
using NUnit.Framework;

namespace CellularAutomataTests
{
    public class FieldTests
    {

        private FieldOptions _fieldOptions;
        private Field _field;


        [SetUp]
        public void Setup()
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

        [Test]
        public void TestSimpleRun()
        {
            _field.RunIteration();
        }
    }
}
