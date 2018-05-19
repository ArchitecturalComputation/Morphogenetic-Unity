using UnityEngine;

namespace Kohonen
{
    partial class TravelingSalesman : Drawing
    {
        class Out
        {
            //declare the weight vector
            public float[] w;

           public Out()
            {
                w = new float[2];
                //randomly generate components of weight vector
                w[0] = Random.Range(0, Width);
                w[1] = Random.Range(0, Height);
            }
        }

        class House
        {
            //x,y co-ordinates of House
            public float[] pos;

            public House()
            {
                pos = new float[2];
                //randomly generate components of position vector
                pos[0] = Random.Range(0, Width);
                pos[1] = Random.Range(0, Height);
            }

            public void Display()
            {
                //draw a rectangle where this house is
                Rect(pos[0], pos[1], 10, 10);
            }
        }
    }
}