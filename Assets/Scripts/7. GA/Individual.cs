using System;

namespace GA
{
    partial class GeneticAlgorithm : Drawing
    {
        //The Comparable Interface (JAVA) imposes a TOTAL ORDERING 
        //on the object of the class that implements it

        class Individual : IComparable<Individual>
        {
            public Genotype i_genotype;
            public Phenotype i_phenotype;
            float i_fitness;

            public Individual()
            {
                i_genotype = new Genotype();
                i_phenotype = new Phenotype(i_genotype);
                i_fitness = 0;
            }

            public void Draw()
            {
                i_phenotype.Draw();
            }

            public void Evaluate()
            {
                i_fitness = i_phenotype.Evaluate();
            }

            //method of the Comparable Interface
            public int CompareTo(Individual objI)
            {
                Individual iToCompare = objI;
                if (i_fitness < iToCompare.i_fitness)
                {
                    return -1; //if i am less fit than iToCompare return -1
                }
                else if (i_fitness > iToCompare.i_fitness)
                {
                    return 1; //if i am fitter than iToCompare return 1
                }

                return 0; // //if we are equally fit return 0
            }
        }

        static Individual Breed(Individual a, Individual b)
        {
            Individual c = new Individual();
            c.i_genotype = Crossover(a.i_genotype, b.i_genotype);
            c.i_genotype.Mutate();
            c.i_phenotype = new Phenotype(c.i_genotype);
            return c;
        }
    }
}