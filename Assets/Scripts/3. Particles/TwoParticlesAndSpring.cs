
namespace Particles.Two
{
    class TwoParticlesAndSpring : Drawing
    {
        Particle a;// = new Particle();
        Particle b;// = new Particle();

        //Global Variables
        public static float k = 1.0f;  //spring constant
        public static float mass = 1000.0f; //mass
        public static float rest = 100.0f; //resting position
        public static float damping = 0.9999f;

        void Start()
        {
            Size(400, 400);
            a = new Particle();
            b = new Particle();

            Background(255);
        }

        void Update()
        {
            //1 - react (add acceleration to velocity and check boundary conditions
            a.React(b);
            b.React(a);

            //2 - add velocity to position
            a.Move();
            b.Move();

            //3 - draw spring
            Stroke(0, 50);
            Line(a.position.x, a.position.y, b.position.x, b.position.y);

            //4 - draw particles
            a.Draw();
            b.Draw();
        }

        private void OnMouseDown()
        {
            a = new Particle();
            b = new Particle();
        }
    }
}
