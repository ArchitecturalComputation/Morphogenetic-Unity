
namespace GA
{
    partial class GeneticAlgorithm : Drawing
    {
        class Phenotype
        {
            float i_Width;
            float i_Height;
            float i_depth;
            float m = 0.001f;

            public Phenotype(Genotype g)
            {
                i_Width = g.genes[0] * Width / 256f;
                i_Height = g.genes[1] * Height / 256f;
                i_depth = g.genes[2] * Height / 256f;
            }

            public void Draw()
            {
                Box(i_Width + m, i_Height + m, i_depth + m);
            }

            public float Evaluate()
            {
                //for this case maximise square of sides and minimise volume
                float fitness = 0;
                fitness += Sq(i_Width + i_Height + i_depth);
                fitness -= i_Width * i_Height * i_depth;
                return fitness;
            }
        }
    }
}