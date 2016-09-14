using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cellular.Models;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        private readonly FieldOptions _fieldOptions;
        private readonly Field _field;
        private int _iterationNumber = 0;
        private BackgroundWorker _worker;

        public Form1()
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

            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += RunIterations;
            InitializeComponent();
        }

        private void RunIterations(object sender, DoWorkEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var worker = sender as BackgroundWorker;
            for (;;)
            {
                _iterationNumber++;
                _field.RunIteration();
                double fps = (double)_iterationNumber / sw.ElapsedMilliseconds * 1000.0;
                RedrawField();
                lblIterationNumber.Invoke((Action)(() =>
                {
                    lblIterationNumber.Text = _iterationNumber.ToString();
                    lblFps.Text = fps.ToString("000.0");
                }));
                if (worker.CancellationPending) break;
            }
        }

        private void btnRunIteration_Click(object sender, EventArgs e)
        {
            btnRunIteration.Enabled = false;
            _field.RunIteration();
            RedrawField();
            btnRunIteration.Enabled = true;
        }

        private void canvas_MouseClick(object sender, MouseEventArgs e)
        {
            for (int x = -5; x < 5; ++x)
                for (int y = -5; y < 5; ++y)
                {
                    var trackingField = new TrackingField(_fieldOptions.TrackingRadius, e.X+x, e.Y+y, _field.GetCellState);
                    var actuator = new CellActuator(_fieldOptions.ActivationThreshold, new Random().NextDouble(), _fieldOptions.MemoRecognizationThreshold);
                    var cell = new Cell(trackingField, actuator, _fieldOptions.TimeToRelax, _fieldOptions.TimeToTakeSnapshot, CellStates.Active);
                    _field.SetCell(e.X + x, e.Y + y, cell);
                }
            RedrawField();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int x = -5; x < 5; ++x)
                    for (int y = -5; y < 5; ++y)
                    {
                        var trackingField = new TrackingField(_fieldOptions.TrackingRadius, e.X+x, e.Y+y,
                            _field.GetCellState);
                        var actuator = new CellActuator(_fieldOptions.ActivationThreshold, new Random().NextDouble(),
                            _fieldOptions.MemoRecognizationThreshold);
                        var cell = new Cell(trackingField, actuator, _fieldOptions.TimeToRelax,
                            _fieldOptions.TimeToTakeSnapshot, CellStates.Active);
                        _field.SetCell(e.X+x, e.Y+y, cell);
                        using (var g = canvas.CreateGraphics())
                        {
                            g.FillRectangle(Brushes.Red, e.X+x, e.Y+y, 1, 1);
                        }
                    }
            }
        }

        private void RedrawField()
        {
            //var buf = new byte[Width*Height*4];
            //int index = 0;
            //for (int j = 0; j < _field.Height; ++j)
            //    for (int i = 0; i < _field.Width; ++i)
            //    {
            //        int color = 0;
            //        var state = CellStates.Active; //_field.GetCellState(i, j);
            //        if (state == CellStates.Active)
            //            color = Color.Red.ToArgb();
            //        if (state == CellStates.Relaxed)
            //            color = Color.Blue.ToArgb();
            //        if (state == CellStates.RelaxedNegative)
            //            color = Color.Gray.ToArgb();
            //        if (state == CellStates.ReactivePositive)
            //            color = Color.Green.ToArgb();
            //        if (state == CellStates.ReactiveNegative)
            //            color = Color.DarkGreen.ToArgb();
            //        if (state == CellStates.Negative)
            //            color = Color.Yellow.ToArgb();
            //        buf[index] = (byte)(color & 0xff);
            //        buf[index+1] = (byte)((color >> 8) & 0xff);
            //        buf[index+2] = (byte)((color >> 16) & 0xff);
            //        index += 4;
            //    }

            //var ms = new MemoryStream(buf);
            //ms.Position = 0;
            //var bmp = new Bitmap();
            //canvas.Image = bmp;

            canvas.SuspendLayout();
            using (var g = canvas.CreateGraphics())
            {
                g.Clear(Color.Black);
                for (int j = 0; j < Height; ++j)
                    for (int i = 0; i < Width; ++i)

                    {
                        var state = _field.GetCellState(i, j);
                        if (state == CellStates.Active)
                            g.FillRectangle(Brushes.Red, i, j, 1, 1);
                        if (state == CellStates.Relaxed)
                            g.FillRectangle(Brushes.Blue, i, j, 1, 1);
                        if (state == CellStates.RelaxedNegative)
                            g.FillRectangle(Brushes.Gray, i, j, 1, 1);
                        if (state == CellStates.ReactivePositive)
                            g.FillRectangle(Brushes.Green, i, j, 1, 1);
                        if (state == CellStates.ReactiveNegative)
                            g.FillRectangle(Brushes.DarkGreen, i, j, 1, 1);
                        if (state == CellStates.Negative)
                            g.FillRectangle(Brushes.Yellow, i, j, 1, 1);
                    }
            }
            canvas.ResumeLayout();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _worker.RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _worker.CancelAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int x = 200; x < 300; ++x)
            {
                for (int y = 250; y < 260; ++y)
                {
                    ActivateCellAt(x, y);
                }
            }

            for (int y = 200; y < 300; ++y)
            {
                for (int x= 250; x < 260; ++x)
                {
                    ActivateCellAt(x, y);
                }
            }

            RedrawField();
        }

        private void ActivateCellAt(int x, int y)
        {
            var trackingField = new TrackingField(_fieldOptions.TrackingRadius, x, y,
                            _field.GetCellState);
            var actuator = new CellActuator(_fieldOptions.ActivationThreshold, new Random().NextDouble(),
                _fieldOptions.MemoRecognizationThreshold);
            var cell = new Cell(trackingField, actuator, _fieldOptions.TimeToRelax,
                _fieldOptions.TimeToTakeSnapshot, CellStates.Active);
            _field.SetCell(x, y, cell);
        }
    }
}
