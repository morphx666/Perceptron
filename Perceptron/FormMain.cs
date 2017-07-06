using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Perceptron {
    public partial class FormMain : Form {
        Perceptron brain;
        DataPoint[] points;

        private int pointsCount = 250;
        private double learningRate = 0.000000001;
        private double tmpM;
        private double tmpB;

        private Random r = new Random();
        private object syncObj = new object();
        Font mono = new Font("Consolas", 10, FontStyle.Regular);
        Stopwatch sw;

        long fCounter = 0;
        long iCounter = 0;
        DateTime startTime;
        TimeSpan runTime;
        bool isDone;
        bool isAdjustingParams;

        public FormMain() {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, false);

            this.KeyDown += (object o, KeyEventArgs s) => {
                if(s.KeyCode == Keys.Enter)
                    SetNewParams();
                Init();
            };

            sw = new Stopwatch();
            int refreshMult = 0;

            Thread autoRefresh = new Thread(() => {
                Thread.Sleep(500); // Wait for the form to initialize (lazy version)
                Init();

                //int degreeOfParallelism = Environment.ProcessorCount;
                //int max;
                //int len = points.Length;

                sw.Start();

                while(true) {
                    if(refreshMult == 15) {
                        refreshMult = 0;
                        this.Invalidate();
                        Thread.Sleep(30);
                    } else {
                        refreshMult++;
                    }

                    if(isDone) continue;
                    iCounter++;

                    lock(syncObj) {
                        double[] w = (double[])brain.Weigths.Clone();

                        //System.Threading.Tasks.Parallel.For(0, degreeOfParallelism, (int workerId) => {
                        //    max = len * (workerId + 1) / degreeOfParallelism;
                        //    for(int i = len * workerId / degreeOfParallelism; i < max; i++) {
                        //        brain.Train(points[i].Inputs, points[i].Answer);
                        //    }
                        //});

                        foreach(DataPoint dp in points) {
                            brain.Train(dp.Inputs, dp.Answer);
                        }

                        isDone = true;
                        for(int i = 0; i < w.Length; i++) {
                            if(w[i] != brain.Weigths[i]) {
                                isDone = false;
                                break;
                            }
                        }
                    }
                }
            }) {
                IsBackground = true
            };
            autoRefresh.Start();
        }

        private void Init() {
            lock(syncObj) {
                brain = new Perceptron(r, 3, learningRate); // Increase the learning rate to increase how fast the algorithm... "learns"
                points = new DataPoint[pointsCount]; // Increase the number of points to increase accuracy

                for(int i = 0; i < points.Length; i++) {
                    points[i] = new DataPoint(r, this.DisplayRectangle.Width, this.DisplayRectangle.Height);
                }

                startTime = DateTime.Now;
                iCounter = 0;
                isDone = false;
            }
        }

        private void SetNewParams() {
            isAdjustingParams = true;

            using(FormDataEntry fde = new FormDataEntry()) {
                fde.textBoxLineEqM.TextChanged += (s, o) => { double.TryParse(fde.textBoxLineEqM.Text, out tmpM); };
                fde.textBoxLineEqB.TextChanged += (s, o) => { double.TryParse(fde.textBoxLineEqB.Text, out tmpB); };

                fde.textBoxDataPoints.Text = points.Length.ToString();
                fde.textBoxLineEqM.Text = DataPoint.m.ToString();
                fde.textBoxLineEqB.Text = DataPoint.b.ToString();
                fde.textBoxLrExp.Text = Math.Log10(1 / learningRate).ToString();

                if(fde.ShowDialog(this) == DialogResult.OK) {
                    int.TryParse(fde.textBoxDataPoints.Text, out pointsCount);
                    double.TryParse(fde.textBoxLineEqM.Text, out DataPoint.m);
                    double.TryParse(fde.textBoxLineEqB.Text, out DataPoint.b);
                    double.TryParse(fde.textBoxLrExp.Text, out learningRate);
                    learningRate = Math.Pow(10, -learningRate);
                }
            }

            isAdjustingParams = false;
        }

        private void FormMain_Paint(object sender, PaintEventArgs e) {
            if(brain == null) return;

            Graphics g = e.Graphics;
            Size s = this.DisplayRectangle.Size;
            float w = s.Width / 2;
            float h = s.Height / 2;

            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Move the top/left coordinates to the center of the screen
            g.ScaleTransform(1, -1);
            g.TranslateTransform(w, -h);

            // Draw X/Y axis
            g.DrawLine(Pens.Gray, 0, h, 0, -h);
            g.DrawLine(Pens.Gray, -w, 0, w, 0);

            // Draw current function
            g.DrawLine(Pens.Blue, -w, (float)DataPoint.F(-w), w, (float)DataPoint.F(w));

            // Draw user line
            if(isAdjustingParams) {
                using(Pen p = new Pen(Color.Green, 3)) {
                    p.DashPattern = new float[] { 4, 2 };
                    g.DrawLine(p, -w, (float)(tmpM * -w + tmpB), w, (float)(tmpM * w + tmpB));
                }
            }

            // Draw predicted function
            double x1 = -w;
            double y1 = (-brain.Weigths[2] - brain.Weigths[0] * x1) / brain.Weigths[1];
            double x2 = w;
            double y2 = (-brain.Weigths[2] - brain.Weigths[0] * x2) / brain.Weigths[1];
            g.DrawLine(Pens.Red, (float)x1, (float)y1, (float)x2, (float)y2);

            foreach(DataPoint dp in points) {
                dp.Render(g);

                RectangleF r = new RectangleF(dp.Point.X - dp.Size.Width / 2, dp.Point.Y - dp.Size.Height / 2, dp.Size.Width, dp.Size.Height);
                r.Inflate(-dp.Size.Width / 5, -dp.Size.Height / 5);
                using(SolidBrush sb = new SolidBrush((brain.Guess(dp.Inputs) == dp.Answer ? Color.Green : Color.Red))) {
                    g.FillEllipse(sb, r);
                }
            }

            g.ResetTransform();
            using(SolidBrush sb = new SolidBrush(Color.FromArgb(230, Color.White))) {
                g.FillRectangle(sb, 0, 0, 200, (brain.Weigths.Length + 13) * 10);
            }
            using(SolidBrush sb = new SolidBrush(Color.FromArgb(32, Color.Black))) {
                g.FillRectangle(sb, 0, 0, 200, (brain.Weigths.Length + 13) * 10);
            }

            string rm = isDone && (DateTime.Now.Millisecond < 750) ? " √" : "";
            int y = 10;
            for(int i = 0; i < brain.Weigths.Length; i++) {
                g.DrawString($"w{i + 1} = " + $"{brain.Weigths[i]:F2}".PadLeft(7) + rm, mono, Brushes.Black, 10, y);
                y += 10;
            }

            y += 10;
            g.DrawString($"{DataPoint.m:F2}".PadLeft(7) + "x + " + $"{DataPoint.b:F2}".PadLeft(7), mono, Brushes.Blue, 10, y);
            y += 10;
            double m = (y2 - y1) / (x2 - x1);
            double b = y1 - m * x1;
            g.DrawString($"{m:F2}".PadLeft(7) + "x + " + $"{b:F2}".PadLeft(7), mono, Brushes.Red, 10, y);

            y += 10 * 2;
            g.DrawString("FPS: " + $"{++fCounter * 1000.0 / sw.ElapsedMilliseconds:F2}".PadLeft(11), mono, Brushes.Black, 10, y);
            y += 10;
            g.DrawString("Itr: " + $"{iCounter:N0}".PadLeft(8), mono, Brushes.Black, 10, y);
            y += 10;
            if(!isDone) runTime = DateTime.Now - startTime;
            g.DrawString("RnT: " + $"{runTime.Hours:00}:{runTime.Minutes:00}:{runTime.Seconds:00}.{runTime.Milliseconds:000}", mono, Brushes.Black, 10, y);

            y += 10 * 2;
            g.DrawString($"Press [ENTER]", mono, Brushes.Gray, 10, y);
            y += 10;
            g.DrawString($"To change parameters", mono, Brushes.Gray, 10, y);
        }
    }
}
