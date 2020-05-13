using MorphxLibs;
using System;

namespace Metaballs {
    public class Blob {
        private double mRadius;
        private double r4;

        public Vector Position { get; set; }
        public double Radius {
            get => mRadius;
            set { 
                mRadius = value;
                r4 = value / 4;
            } 
        }

        public Vector Velocity { get; }

        public Blob(double x, double y, double diameter, Vector velocity) {
            Position = new Vector(1, 0, x, y);
            Radius = diameter;
            Velocity = velocity;
        }

        public void Move(int right, int bottom) {
            Position.Move(Velocity);
            CheckBoundsCollisions(right, bottom);
        }

        private void CheckBoundsCollisions(int r, int b) {
            if(Position.X1 - r4 < 0) { // Left Bound
                Position.TranslateAbs(0 + r4, Position.Y1);
                Velocity.Angle = 0 + Constants.PI180 - Velocity.Angle;
            } else if(Position.X1 + r4 > r) { // Right Bound
                Position.TranslateAbs(r - r4, Position.Y1);
                Velocity.Angle = Constants.PI180 + 0 - Velocity.Angle;
            }

            if(Position.Y1 + r4 > b) { // Top Bound
                Position.TranslateAbs(Position.X1, b - r4);
                Velocity.Angle = Constants.PI270 + Constants.PI90 - Velocity.Angle;
            } else if(Position.Y1 - r4 < 0) { // Bottom Bound
                Position.TranslateAbs(Position.X1, 0 + r4);
                Velocity.Angle = Constants.PI90 + Constants.PI270 - Velocity.Angle;
            }
        }
    }
}