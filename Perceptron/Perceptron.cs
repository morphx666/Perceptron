using System;

// Code inspired by the Perceptron tutorials from The Coding Train
// Tutorial 1: https://www.youtube.com/watch?v=ntKn5TPHHAk&t=870s
// Tutorial 2: https://www.youtube.com/watch?v=DGxIcDjPzac&t=1183s

namespace Perceptron {
    public class Perceptron {
        double[] mWeigths;
        double mLearningRate = 0.1;
        double mod = 0;
        double lr;

        public Perceptron(Random r, int n, double learningRate) {
            mLearningRate = learningRate;
            lr = mLearningRate;
            mod = lr / 1000;

            mWeigths = new double[n];
            for(int i = 0; i < mWeigths.Length; i++) {
                //mWeigths[i] = r.Next(0, 10) >= 5 ? 1 : -1;
                mWeigths[i] = r.NextDouble() * 2 - 1;
            }
        }

        public double LearningRate {
            get {
                return mLearningRate;
            }
        }

        public int Guess(double[] inputs) {
            double sum = 0.0;
            for(int i = 0; i < mWeigths.Length; i++) {
                sum += inputs[i] * mWeigths[i];
            }

            // Activation function
            return (sum > 0 ? 1 : -1);
        }

        public double[] Weigths { get { return mWeigths; } }

        public void Train(double[] inputs, int target) {
            int error = target - Guess(inputs);
            if(error == 0) return;

            for(int i = 0; i < mWeigths.Length; i++) {
                mWeigths[i] += error * inputs[i] * lr;
            }

            if(lr < 1) lr += mod; // This logic should be inverted, so that learningRate decreases as time pases.
                                                      // It is just implemented this way for cosmetic purposes only.
        }
    }
}
