using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdvGraphics
{
    public class Explosive : Particle
    {
        public Explosive(Explosive other)
        {
            center_of_object = new Point3D(other.center_of_object.GetPoint().X, other.center_of_object.GetPoint().Y, other.center_of_object.GetZ());
            velocity = new double[3];
            acceleration = new double[3];

            Array.Copy( other.velocity, velocity, 3);
            Array.Copy( other.acceleration, acceleration, 3);
            max_age = 50;

            color = Color.FromArgb(other.color.A, other.color.R, other.color.G, other.color.B);
            size = other.size;
            age = other.age;
        }

        public Explosive(int x, int y, int z, double accel_x, double accel_y, double velocity_x, double velocity_y)
        {
            center_of_object = new Point3D(x, y, z);
            velocity = new double[3];
            Random rand = new Random(DateTime.Now.Millisecond);
            velocity[0] = velocity_x;
            velocity[1] = velocity_y;
            velocity[2] = 0.0;

            acceleration = new double[3];
            acceleration[0] = 0.0;
            acceleration[1] = -9.8;
            acceleration[2] = 0.0;
            max_age = 75;

            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[rand.Next(names.Length)];
            color = Color.FromKnownColor(randomColorName);
             
            //color = Color.FromArgb(255, Convert.ToInt32(rand.NextDouble() * 255), Convert.ToInt32(rand.NextDouble() * 255), Convert.ToInt32(rand.NextDouble() * 255));
            size = 2;
            age = 1;
        }

        public override void Move(double delta_accel_x, double delta_accel_y)
        {
            int x = center_of_object.GetPoint().X;
            int y = center_of_object.GetPoint().Y;

            x = Convert.ToInt32(x - velocity[0]);
            y = Convert.ToInt32(y - velocity[1]);

            //new point
            center_of_object = new Point3D(x, y, 0);

            //new velocity
            velocity[0] = velocity[0] + acceleration[0] + delta_accel_x;
            velocity[1] = velocity[1] + acceleration[1] + delta_accel_y;
        }

        public override void Slow()
        {
            velocity[0] *= .75;
            velocity[1] *= .75;
        }

        public override void AgeParticle(int increment = 1)
        {
            if (tracer)
            {
                base.AgeParticle(increment * 2);

                int a = Convert.ToInt32(color.A * .35);
                int r = color.R;
                int g = color.G;
                int b = color.B;

                color = Color.FromArgb(a, r, g, b);

                if (a == 0)
                    age = max_age;

            }
            else
            {
                base.AgeParticle();

                int a = color.A;
                int r = color.R;
                int g = color.G;
                int b = color.B;

                if( r < 255 )
                {
                    r += 20;
                    if (r > 255)
                        r = 255;
                }
                //fade from red to orage to yellow to white
                if (g < 255)
                {
                    b = 0;
                    g += 30;
                    if (g > 255)
                        g = 255;
                }
                else
                {
                    if (b < 255)
                    {
                        b += 30;
                        if (b > 255)
                            b = 255;
                    }
                }
                
                a = Convert.ToInt32(a * .90);

                if (a <= 5)
                {
                    a = 0;
                    age = max_age;
                }

                color = Color.FromArgb(a, r, g, b);

                if (a == 0 || color.ToKnownColor() == KnownColor.White)
                    age = max_age;
            }
        }

        public override bool Draw(int h, int w, int[,] pixels)
        {
            BresenhamLine line = new BresenhamLine(h, w);
            bool bDrawn = false;
            //draw a square for the rocket = easier to fill 
            for (int i = 0; i < size; i++)
            {
                if (center_of_object.GetPoint().Y + i < h && center_of_object.GetPoint().Y + i > 0 && center_of_object.GetPoint().X > 0 && center_of_object.GetPoint().X + size < w)
                    bDrawn = true;

                line.DrawBresenhamLine(pixels, center_of_object.GetPoint().Y + i, center_of_object.GetPoint().X,
                                       center_of_object.GetPoint().Y + i, center_of_object.GetPoint().X + size, color);
            }

            return bDrawn;
        }

        public override void Draw(Graphics g)
        {
            g.DrawEllipse(new Pen(color), center_of_object.GetPoint().X, center_of_object.GetPoint().Y, size, size);
            g.FillEllipse(new SolidBrush(color), center_of_object.GetPoint().X, center_of_object.GetPoint().Y, size, size);
        }

        public override void MorphShape()
        {
            throw new NotImplementedException();
        }

        public override Color GetColor()
        {
            return color;
        }

        public override void SetColor(Color clr)
        {
            color = clr;
        }

        public override double GetSize()
        {
            return size;
        }

        public override bool IsAlive()
        {
            return age < max_age;
        }

        public override bool LeaveTrace()
        {
            if (!tracer)
                return true;

            return false;
        }
    }
}
