using System;
using System.Collections.Generic;
using System.Drawing;


namespace AdvGraphics
{
    public abstract class Particle
    {
        protected Point3D center_of_object;   //location
        protected double[] velocity;   //vector
        protected double[] acceleration; //vector
        protected int size;
        protected Color color;
        protected double age;
        protected double max_age;
        protected bool tracer;

        //ax = acceleration in X
        //ay = acceleration in Y
        //vx = length cos theta - velocity ( detla x over time )
        //vy = length sin theta - velcity ( delta y over time )
        //x = x + vx * delta time
        //y = y + vy * delta time
        //vx = vx + ax * detla time
        //vy = vy + ay * delta time

        public void SetMaxAge(double max)
        {
            max_age = max;
        }

        public virtual void Draw( Graphics g )
        {
            
        }

        public virtual bool Draw(int h, int w, int[,] pixels)
        {
            return false;
        }

        public virtual void Move( double delta_accel_x, double delta_accel_y )
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

            if (velocity[1] < -9.81)
                velocity[1] = -9.81;
        }

        public virtual void AgeParticle( int increment = 1 )
        {
            age += increment;
            return;
        }

        public virtual void MorphShape()
        {
            return;
        }

        public virtual void Slow()
        {
            if (velocity[0] > 0)
                velocity[0] *= .80;

            if (velocity[1] > 0)
                velocity[1] *= .80;
        }

        public virtual Color GetColor()
        {
            return color;
        }

        public virtual void SetColor(Color clr)
        {
            color = clr;
        }

        public virtual double GetSize()
        {
            return size;
        }

        public virtual void SetSize(int val)
        {
            size = val;
        }

        public virtual bool IsAlive()
        {
            return true;
        }

        public virtual List<Particle> Die()
        {
            return null;
        }

        public virtual bool LeaveTrace()
        {
            if (!tracer)
                return true;

            return false;
        }

        public virtual bool GetTracer()
        {
            return tracer;
        }

        public virtual void SetTracer( bool trace )
        {
            if (LeaveTrace())
                tracer = trace;
        }
    }
}
