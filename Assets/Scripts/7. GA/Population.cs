using System;
using Random = UnityEngine.Random;

namespace GA
{
    partial class GeneticAlgorithm : Drawing
    {
        class Population
        {
            public Individual[] pop;

            public Population()
            {
                pop = new Individual[populationNum];
                for (int i = 0; i < populationNum; i++)
                {
                    pop[i] = new Individual();
                    pop[i].Evaluate();
                }

                //Arrays is a JAVA class identifier 
                //sort is one of its functions: it sorts a class of comparable elements
                Array.Sort(pop);
            }

            public void Evolve()
            {
                Individual a, b, x;

                for (int i = 0; i < populationNum; i++)
                {
                    a = Select();
                    b = Select();
                    //breed the two selected individuals
                    x = Breed(a, b);
                    //place the offspring in the lowest position in the population, thus replacing the previously weakest offspring
                    pop[i] = x;
                    //evaluate the new individual (grow)
                    pop[i].Evaluate();
                }
                //the fitter offspring will find its way in the population ranks
                Array.Sort(pop);
            }

            Individual Select()
            {
                //skew distribution; multiplying by 99.999999 scales a number from 0-1 to 0-99, BUT NOT 100
                //the Sqrt of a number between 0-1 has bigger possibilities of giving us a smaller number
                //if we subtract that squares number from 1 the opposite is true-> we have bigger possibilities of having a larger number
                int which = (int)((populationNum - 1e-3) * (1.0f - Sq(Random.Range(0, 1f))));
                return pop[which];
            }
        }
    }
}