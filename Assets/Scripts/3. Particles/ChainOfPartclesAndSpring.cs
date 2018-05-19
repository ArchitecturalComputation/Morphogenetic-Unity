
namespace Particles.Chain
{
    class ChainOfPartclesAndSpring : Drawing
    {
        Particle[] chain;

        //Global Variables
        public static float k = 1.0f;  //spring constant
        public static float mass = 1000.0f; //mass
        public static float rest = 100.0f; //resting position
        public static float damping = 0.99f;
        int numOfPParticle = 10;

        void Start()
        {
            Size(400, 400);

            chain = new Particle[numOfPParticle];
            for (int i = 0; i < chain.Length; i++)
            {
                chain[i] = new Particle();
            }

            Background(255);
        }

        void Update()
        {
            //1 - react (add acceleration to velocity and check boundary conditions
            for (int i = 0; i < chain.Length; i++)
            {
                chain[i].React(chain, i);
            }

            //2 - add velocity to position
            for (int i = 0; i < chain.Length; i++)
            {
                chain[i].Move();
            }

            //3 - draw spring
            for (int i = 0; i < chain.Length; i++)
            {
                chain[i].DrawSpring(chain, i);
            }

            //4 - draw particles
            for (int i = 0; i < chain.Length; i++)
            {
                chain[i].DrawParticle();
            }
        }

        void OnMouseDown()
        {
            for (int i = 0; i < chain.Length; i++)
            {
                chain[i] = new Particle();
            }
        }
    }
}