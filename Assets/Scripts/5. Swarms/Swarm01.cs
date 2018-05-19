
namespace Swarms
{
    class Swarm01 : Drawing
    {
        Fly[] swarm = new Fly[200];

        void Start()
        {
            Size(400, 400, true);
            for (int i = 0; i < swarm.Length; i++)
            {
                swarm[i] = new Fly();
            }

            Background(0);
        }

        void Update()
        {
             for (int i = 0; i < swarm.Length; i++)
            {
                swarm[i].Move(swarm);
                swarm[i].Draw();
            }
        }
    }
}