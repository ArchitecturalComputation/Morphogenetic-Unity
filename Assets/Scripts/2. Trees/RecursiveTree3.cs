using UnityEngine;

namespace Trees
{
    class RecursiveTree3 : Drawing
    {
        // A simple recursive tree using a proto-L-System
        // Alasdair Turner 2010
        Branch axiom;

        void Start()
        {
            Size(800, 800, true);
            axiom = new Branch();

            Background(0);
            //Stroke(192);
            Fill(255);
            Lights();
        }

        void Update()
        {
            Translate(Width / 2, Height);
            RotateY(Time.frameCount * 0.01f);
            axiom.Draw();

            if (Time.frameCount % 50 == 0)
            {
                axiom.Replace(0);
            }
        }

        void OnMouseDown()
        {
            axiom = new Branch();
        }
    }
}