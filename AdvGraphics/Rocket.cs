using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdvGraphics
{
    public class Rocket : Particle
    {
        private double num_pieces;
        public Rocket( Rocket other )
        {
            num_pieces = other.num_pieces;
            center_of_object = other.center_of_object;
            velocity = new double[3];
            Array.Copy( other.velocity, velocity, 3);
            acceleration = new double[3];
            Array.Copy(other.acceleration, acceleration, 3);

            color = other.color;
            age = other.age;
            max_age = other.max_age;
            num_pieces = 0;
            size = other.size;
            tracer = other.tracer;
        }

        public Rocket( int x, int y, int z, double velocity_x, double velocity_y )
        {
            center_of_object = new Point3D(x, y, z);
            velocity = new double[3];
            Random rand = new Random(DateTime.Now.Millisecond);
            velocity[0] = velocity_x;
            velocity[1] = velocity_y;
            velocity[2] = 0;

            acceleration = new double[3];
            acceleration[0] = 0.0;
            acceleration[1] = -1.8;
            acceleration[2] = 0.0;

            num_pieces = 200;

            size = 5;

            color = Color.FromArgb(255, Convert.ToInt32(rand.NextDouble() * 255), Convert.ToInt32(rand.NextDouble() * 255), Convert.ToInt32(rand.NextDouble() * 255));
            tracer = false;
        }

        public void SetNumPieces( int pieces )
        {
            num_pieces = pieces;
        }

        public override void Move(double delta_accel_x, double delta_accel_y)
        {
            bool bSwapped = false;
            bool bPositive = true;

            int x = center_of_object.GetPoint().X;
            int y = center_of_object.GetPoint().Y;

            if (velocity[1] < 0.0)
                bPositive = false;

            x = Convert.ToInt32(x - velocity[0]);
            y = Convert.ToInt32(y - velocity[1]);

            //new point
            center_of_object = new Point3D(x, y, 0);

            //new velocity
            velocity[0] = velocity[0] + acceleration[0] + delta_accel_x;
            velocity[1] = velocity[1] + acceleration[1] + delta_accel_y;

            if (bPositive)
            {
                if (velocity[1] < 0.0)
                    bSwapped = true;
            }
            else
            {
                if (velocity[1] > 0.0)
                    bSwapped = true;
            }

            if (bSwapped)
                age = max_age;
        }

        public override bool Draw(int h, int w, int[,] pixels)
        {
            BresenhamLine line = new BresenhamLine( h, w );

            bool bDrawn = false;
            //draw a square for the rocket = easier to fill 
            for (int i = 0; i < size; i++)
            {
                if (center_of_object.GetPoint().Y + i < h && center_of_object.GetPoint().Y + i > 0 && center_of_object.GetPoint().X > 0 && center_of_object.GetPoint().X+size < w)
                    bDrawn = true;

                line.DrawBresenhamLine(pixels, center_of_object.GetPoint().Y + i, center_of_object.GetPoint().X,
                                       center_of_object.GetPoint().Y + i, center_of_object.GetPoint().X + size, color);
            }

            return bDrawn;
        }

        public override void Draw(Graphics g)
        {
            //g.DrawEllipse( new Pen(color), center_of_object.GetPoint().X, center_of_object.GetPoint().Y, size, size );
            g.FillEllipse(new SolidBrush(color), center_of_object.GetPoint().X, center_of_object.GetPoint().Y, size, size);
        }

        public override void AgeParticle(int increment = 1)
        {
            if (tracer)
            {
                age += increment * 2.5;
                color = Color.FromArgb(color.A/2, color.R, color.G, color.B);
            }
            else
            {
                age += increment;
            }
        }

        public override bool IsAlive()
        {
            if (age < max_age)
                return true;

            return false;
        }

        public override List<Particle> Die()
        {
            List<Particle> pieces = new List<Particle>();

            if (tracer)
                return pieces;

            Random rand = new Random(DateTime.Now.Millisecond);

            double degrees = 360/num_pieces;
            if( degrees == 0.0 )
            {
                double temp = 360/Convert.ToDouble(num_pieces);

                while (temp < 1)
                    temp *= 10.0;

                degrees = Convert.ToInt32(temp);
            }

            double percent = rand.NextDouble();

            while (percent < 0.5)
                percent = rand.NextDouble();

            for (int i = 0; i < num_pieces; i++)
            {
                double val = rand.Next(1, 50);

                double velx = (val * percent) * Math.Cos(Math.PI * degrees * i / 180.0);
                double vely = (val * percent) * Math.Sin(Math.PI * degrees * i / 180.0);

                //"explode" particles in 360 degrees around the center point all with varying initial velocities in X & Y directions
                Explosive exp = new Explosive(center_of_object.GetPoint().X, center_of_object.GetPoint().Y, 0, 0, -9.8, velx, vely);
                pieces.Add(exp);
            }

            return pieces;
        }

        public override bool LeaveTrace()
        {
            if( !tracer )
                return true;

            return false;
        }
    }
}
