using System;
using System.Drawing;

namespace Perceptron {
    public class DataPoint {
        private double mX;
        private double mY;
        private PointF mPoint;
        private int mAnswer;
        private double mBias = 1.0;
        public readonly Size Size = new Size(16, 16);

        public static double m = 1.0;
        public static double b = 0.0;

        //public static double F(double x) { return -0.756 * x + 217.49; }
        public static double F(double x) { return m * x + b; }

        public DataPoint(Random r, int w, int h) {
            mX = (r.NextDouble() * 2 - 1) * w / 2;
            mY = (r.NextDouble() * 2 - 1) * h / 2;
            mPoint = new PointF((float)mX, (float)mY);
            mAnswer = (F(mX) >= mY ? 1 : -1);
        }

        public PointF Point { get { return mPoint; } }

        public int Answer { get { return mAnswer; } }

        public double Bias { get { return mBias; } }

        public double[] Inputs { get { return new double[3] { mX, mY, mBias }; } }

        public void Render(Graphics g) {
            g.FillEllipse(mAnswer == 1 ? Brushes.White : Brushes.Black, mPoint.X - Size.Width / 2, mPoint.Y - Size.Height / 2, Size.Width, Size.Height);
            using(Pen p = new Pen(Color.Black, 2)) {
                g.DrawEllipse(p, mPoint.X - Size.Width / 2, mPoint.Y - Size.Height / 2, Size.Width, Size.Height);
            }
        }

        public override string ToString() {
            return $"{mPoint.X}:{mPoint.Y} -> {mAnswer}";
        }
    }
}
