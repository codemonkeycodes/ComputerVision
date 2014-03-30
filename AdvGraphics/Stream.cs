using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdvGraphics
{
    public class Stream : Particle
    {
        private int num_pieces;
        public Stream( Stream other )
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

            tracer = other.tracer;
        }

        public Stream(int x, int y, int z, double accel_x, double accel_y, double velocity_x, double velocity_y  )
        {
            center_of_object = new Point3D(x, y, z);
            velocity = new double[3];
            Random rand = new Random(DateTime.Now.Millisecond);
            velocity[0] = velocity_x;
            velocity[1] = velocity_y;
            velocity[2] = 0;

            acceleration = new double[3];
            acceleration[0] = accel_x;
            acceleration[1] = accel_y;
            acceleration[2] = 0.0;

            num_pieces = 1;

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

        public override void Draw(Graphics g)
        {
            g.DrawEllipse( new Pen(color), center_of_object.GetPoint().X, center_of_object.GetPoint().Y, size, size );
            //g.FillEllipse(new SolidBrush(color), center_of_object.GetPoint().X, center_of_object.GetPoint().Y, size, size);
        }

        public override void AgeParticle(int increment = 1)
        {
            if (tracer)
            {
                age += increment * 2.5;
                //color = Color.FromArgb(color.A/2, color.R, color.G, color.B);
            }
            else
            {
                age += increment;
            }
        }

        public override List<Particle> Die()
        {
            List<Particle> pieces = new List<Particle>();

            if (tracer)
                return pieces;

            Random ran = new Random(DateTime.Now.Millisecond);

            double velx = ran.NextDouble();
            double vely = ran.NextDouble()*5;

            if (velx < .5)
                velx *= -1.0;

            Random rand = new Random();

            int degrees = 180/num_pieces;
            for (int i = 0; i < num_pieces; i++)
            {
                double val = rand.Next(1, 100);
                //"explode" particles in 360 degrees around the center point all with varying initial velocities in X & Y directions
                Stream exp = new Stream(center_of_object.GetPoint().X, center_of_object.GetPoint().Y, 0, 0, -9.8, velx * Math.Cos(Math.PI * degrees * i / 180.0), vely * Math.Sin(Math.PI * degrees * i / 180.0));
                exp.SetColor(Color.Blue);
                pieces.Add(exp);
            }

            return pieces;
        }

        public override bool LeaveTrace()
        {
            return true;
        }
    }
}
