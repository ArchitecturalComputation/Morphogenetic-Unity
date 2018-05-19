using UnityEngine;
using static Drawing;

namespace Swarms
{
    class Fly
    {
        Vector2 position;
        Vector2 direction;
        float speed;
        Color colour;

        public Fly()
        {
            position = new Vector2(Random.Range(0f, Width), Random.Range(0f, Height));
            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            colour = Random.ColorHSV();
            speed = Random.Range(1f, 2f);
        }

        public void Move(Fly[] swarm)
        {
            Vector2 centre = new Vector2(0, 0);
            for (int i = 0; i < swarm.Length; i++)
            {
                centre += swarm[i].position;
            }
            centre /= swarm.Length;

            int closest = -1;
            float closestdist = Width * Height;
            for (int i = 0; i < swarm.Length; i++)
            {
                float d = Vector2.Distance(swarm[i].position, position);
                if (swarm[i] != this && d < closestdist)
                {
                    closest = i;
                    closestdist = d;
                }
            }
            if (closestdist > 10)
            {
                Vector2 centredir = centre - position;
                centredir.Normalize();
                direction += centredir;
            }
            else
            {
                Vector2 closestdir = swarm[closest].position - position;
                closestdir.Normalize();
                direction -= closestdir;
            }

            direction.Normalize();
            direction *= speed;
            position += direction;
        }

        public void Draw()
        {
            Stroke(colour);
            StrokeWeight(3);
            Point(position.x, position.y);
        }
    }
}
