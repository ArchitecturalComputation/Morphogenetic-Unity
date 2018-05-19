using UnityEngine;
using static UnityEngine.Mathf;

namespace Trees
{
    class RecursiveTree2 : Drawing
    {
        //A simple recursive tree
        // 3D version
        // Alasdair Turner 2009

        int seed = 0;

        void Start()
        {
            Size(800, 800, true);

            Background(0);
            Lights();
            Stroke(125);
            Fill(255);
        }

        void Update()
        {
            Translate(Width / 2, Height);
            RotateY(Time.frameCount * 0.01f);
            Random.InitState(seed);
            Branch(-3);

        }

        void Branch(int depth)
        {
            if (depth < 10)
            {
                PushMatrix();
                {
                    Translate(0, -Height / 6f, 0);
                    Box(10, Height / 3, 10);
                }
                PopMatrix();
                PushMatrix();
                {
                    Translate(0.0f, -Height / 5f, 0);
                    RotateY(Random.Range(0, PI));
                    RotateX(Random.Range(-PI / 4, PI / 4));
                    Scale(0.7f);
                    Branch(depth + 1);
                }
                PopMatrix();
                PushMatrix();
                {
                    Translate(0.0f, -Height / 3f, 0);
                    RotateY(Random.Range(0, PI));
                    Rotate(Random.Range(-PI / 4, PI / 4));
                    Scale(0.7f);
                    Branch(depth + 1);
                }
                PopMatrix();
            }
        }

        void OnMouseDown()
        {
            seed++;
        }
    }
}
