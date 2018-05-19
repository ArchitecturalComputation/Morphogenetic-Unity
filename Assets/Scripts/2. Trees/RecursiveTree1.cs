using UnityEngine;
using static UnityEngine.Mathf;


namespace Trees
{
    class RecursiveTree1 : Drawing
    {
        // A simple recursive tree
        // Alasdair Turner
        int seed = 0;

        void Start()
        {
            Size(Width, Width);
            Background(204);
            StrokeWeight(10);
        }

        void Update()
        {
            Translate(Width / 2, Height);
            Random.InitState(seed);
            Branch(0);
        }

        void Branch(int depth)
        {
            if (depth < 10)
            {
                Line(0, 0, 0, -Height / 3f);
                PushMatrix();
                {
                    Translate(0, -Height / 5f);
                    Rotate(Random.Range(-PI / 4, PI / 4));
                    Scale(0.7f);
                    Branch(depth + 1);
                }
                PopMatrix();
                PushMatrix();
                {
                    Translate(0, -Height / 3f);
                    Rotate(Random.Range(-PI / 4, PI / 4));
                    Scale(0.7f);
                    Branch(depth + 1);
                }
                PopMatrix();
            }
        }

        void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                seed++;
            }
        }
    }
}