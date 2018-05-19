using UnityEngine;

namespace GA
{
    partial class GeneticAlgorithm : Drawing
    {
        class Genotype
        {
            public int[] genes;

            public Genotype()
            {
                genes = new int[3];
                for (int i = 0; i < genes.Length; i++)
                {
                    genes[i] = Random.Range(0, 256);
                }
            }


            public void Mutate()
            {
                //5% mutation rate
                for (int i = 0; i < genes.Length; i++)
                {
                    if (Random.Range(0, 100) < 5)
                    {
                        genes[i] = Random.Range(0, 256);
                    }
                }
            }

        }

        static Genotype Crossover(Genotype a, Genotype b)
        {
            Genotype c = new Genotype();
            for (int i = 0; i < c.genes.Length; i++)
            {
                //50-50 chance of selection
                if (Random.Range(0, 1f) < 0.5f)
                {
                    c.genes[i] = a.genes[i];
                }
                else
                {
                    c.genes[i] = b.genes[i];
                }
            }
            return c;
        }
    }
}