using System.Collections.Generic;
using System.Drawing;

namespace AdvGraphics
{
    public class ParticleSystem
    {
        private List<Particle> particles;
        private int frame;
        private int animation;
        private bool save_images;
        private int height;
        private int width;
        private int[,] pixels;

        public ParticleSystem()
        {
            save_images = false;
            particles = new List<Particle>();
            frame = 0;
            animation = 0;
        }

        public void SetSaveImages( bool save )
        {
            save_images = save;
        }

        public void AddParticle( Particle part )
        {
            particles.Add(part);
        }

        public bool UpdateSystem( Graphics g, int w = 0, int h = 0 )
        {
            height = h;
            width = w;
            pixels = new int[height,width]; //all initialized to 0 which equates to black

            bool drawn = false;

            //if there are no particles in the system - nothing to do
            if (particles.Count == 0)
            {
                frame = 0;
                return false;
            }

            List<Particle> dead_items = new List<Particle>();
            List<Particle> new_items = new List<Particle>();

            for ( int i = particles.Count-1; i >= 0; i-- )
            {
                Particle part = particles[i];
                //no change in acceleration
                part.Move(0.0, 0.0);

                int aging_factor = 1;
                if( part.GetTracer() )
                    aging_factor = 3;

                part.AgeParticle(aging_factor);
                
                part.Draw(g);
                if (save_images)
                    if (part.Draw(height, width, pixels))
                        drawn = true;

                if( !part.IsAlive() )
                {
                    List<Particle> items = part.Die();
                    dead_items.Add(part);
                    if( items != null )
                        foreach (var particle in items)
                                new_items.Add(particle);
                }
            }

            //walk the particles looking to see if they have tracers
            List<Particle> aged_particles = new List<Particle>();
            foreach (var part in particles)
            {
                if (part.LeaveTrace())
                {
                    Rocket rock = part as Rocket;
                    if (rock != null)
                    {
                        if (!rock.GetTracer())
                        {
                            Rocket temp = new Rocket(rock);
                            temp.SetTracer(true);
                            temp.Slow();
                            aged_particles.Add(temp);
                        }
                    }
                    else
                    {
                        Explosive exp = part as Explosive;
                        if (!exp.GetTracer())
                        {
                            Explosive temp = new Explosive(exp);
                            temp.SetTracer(true);
                            temp.Slow();
                            aged_particles.Add(temp);
                        }
                    }
                }
            }

            if (dead_items.Count > 0)
                foreach (var deadItem in dead_items)
                    particles.Remove(deadItem);

            if (new_items.Count > 0)
                foreach (var particle in new_items)
                    particles.Add(particle);

            if (aged_particles.Count > 0)
                foreach (var particle in aged_particles)
                    particles.Add(particle);

            if(save_images && drawn)
            {
                ColorManipulation manip = new ColorManipulation();
                manip.H = height;
                manip.W = width;

                string file_name = string.Format("particle_{0,4:D4}_{1}.png", frame, animation);
                manip.SaveToImage(pixels, file_name);
                frame++;    
            }

            if (particles.Count == 0)
                animation++;

            return true;
        }
    }
}
