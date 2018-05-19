using UnityEngine;
using static Drawing;
using static Particles.Two.TwoParticlesAndSpring;

namespace Particles.Two
{
    class Particle
    {
        //
        public Vector3 position;
        Vector3 velocity;

        //contructor
        public Particle()
        {
            //set particle's initial position and velocity
            position = new Vector3(Random.Range(0, Width), Random.Range(0, Width));
            velocity = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
        }

        public void React(Particle p)
        {
            //1 - calculate displacement based on distance between particles and rest length
            float distance = Vector3.Distance(position, p.position);
            float displacement = distance - rest;

            //2 - find particle's direction - we'll add the force "along" the spring
            Vector3 acceleration = p.position - position;
            //create a unit vector of the direction
            acceleration.Normalize();

            //3 - calculate acceleration based on Newoton's 2nd Law and Hooke's Law of Elasticity
            //Hooke's Law of Elasticity-> Force = k*displacement
            //2nd Newtonian Law->         Force = Mass*acceleration
            //acceleration = Force/Mass
            //acceleration = k*displacement/mass
            acceleration *= (k * displacement / mass);

            //4 - Add acceleration to velocity
            velocity += acceleration;

            //5 - check boundary conditions
            Bounce();

            //6 - Damp movement, as if taking FRICTION into account
            velocity *= damping;
        }

        public void Move()
        {
            position += velocity;
        }

        public void Draw()
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
