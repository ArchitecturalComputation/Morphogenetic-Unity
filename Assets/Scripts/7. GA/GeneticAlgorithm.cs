using UnityEngine;
using static UnityEngine.Mathf;

namespace GA
{
    partial class GeneticAlgorithm : Drawing
    {

        //Genotype a;
        //Phenotype b;
        Population p;

        static int populationNum = 100;
        int numRows;

        void Start()
        {
            numRows = (int)Sqrt(populationNum);

            Size(500, 500, true);

            //a = new Genotype();
            //b = new Phenotype(a);
            p = new Population();

            //println(b.evaluate());

            Background(204);
            Lights();
            //noStroke();
            Fill(255);
        }

        void Update()
        {
            if (Time.frameCount % 10 == 0)
            {
                p.Evolve();
            }
            /*translate(Width/2, Height/2);
             rotateY(0.01*frameCount);
             b.draw();*/

            for (int i = 0; i < p.pop.Length; i++)
            {
                PushMatrix();
                Scale(1.0f / numRows, 1.0f / numRows, 1.0f / numRows);
                Translate(Width * (i % numRows), Height * (i / numRows));
                Translate(Width / 2, Height / 2, 0);
                RotateY(0.01f * Time.frameCount);
                p.pop[i].Draw();
                PopMatrix();
            }
        }
    }
}