using UnityEngine;

namespace Swarms
{
    class Swarm02 : Drawing
    {
        Boid[] flock = new Boid[200];

        void Start()
        {
            Size(600, 600, true);
            for (int i = 0; i < flock.Length; i++)
            {
                flock[i] = new Boid(i);
            }

            Background(0);
            Lights();
            Random.InitState(41);
        }

        void Update()
        {
            Translate(Width / 2f, Height / 2f);
            for (int i = 0; i < flock.Length; i++)
            {
               flock[i].Move(flock);
               flock[i].Draw();
            }
        }
    }
}