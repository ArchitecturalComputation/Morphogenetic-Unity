using UnityEngine;
using static Drawing;
using static Particles.Chain.ChainOfPartclesAndSpring;

namespace Particles.Chain
{
    class Particle
    {
        Vector3 position;
        Vector3 velocity;

        //contructor
        public Particle()
        {
            //set particle's initial position and velocity
            position = new Vector3(Random.Range(0, Width), Random.Range(0, Height));
            velocity = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
        }

        public void React(Particle[] chain, int me)
        {
            //1 - check distances between neighboring particles
            //in this case acceleration will be calculated in relation to the reaction of both neighbours
            for (int i = me - 1; i <= me + 1; i++)
            {
                //make sure boundary particles take into account the start or end of chain
                int i_c = (i + chain.Length) % chain.Length;

                if (i_c != me)//check "me" against my neighbors, but NOT myself
                {
                    //1 - calculate displacement based on distance between particles and rest length
                    float distance = Vector3.Distance(position, chain[i_c].position);
                    float displacement = distance - rest;

                    //2 - find acceleration direction - we'll add the force "along" the spring
                    Vector3 acceleration = chain[i_c].position - position;
                    //create a unit vector of the direction
                    acceleration.Normalize();

                    //3 - calculate acceleration based on Newoton's 2nd Law and Hooke's Law of Elasticity
                    //Hooke's Law of Elasticity-> F=k*x
                    //Acceleration = Force/Mass
                    //acceleration = k*displacement/mass
                    acceleration *= (k * displacement / mass);

                    //4 - Add acceleration to velocity
                    velocity += acceleration;
                }
            }

            //5 - check boundary conditions
            Bounce();

            //6 - Damp movement, as if taking FRICTION into account
            velocity *= damping;
        }

        public void Move()
        {
            position += velocity;
        }

        public void DrawSpring(Particle[] chain, int me)
        {
            for (int i = me; i <= me + 1; i++)
            {
                int i_c = (i + chain.Length) % chain.Length;
                if (i_c != me)
                {
                    Stroke(150);
                    Line(position.x, position.y, chain[i_c].position.x, chain[i_c].position.y);
                }
            }
        }

        public void DrawParticle()
        {
            Fill(200, 200);
            Ellipse(position.x, position.y, 30, 30);
            Fill(100, 100);
            Ellipse(position.x, position.y, 20, 20);
            Fill(0);
            Ellipse(position.x, position.y, 5, 5);
        }

        void Bounce()
        {
            if ((position.x < 0 && velocity.x < 0) || (position.x > Width && velocity.x > 0))
            {
                velocity.x = -velocity.x;
            }
            if ((position.y < 0 && velocity.y < 0) || (position.y > Height && velocity.y > 0))
            {
                velocity.y = -velocity.y;
            }
        }
    }
}
