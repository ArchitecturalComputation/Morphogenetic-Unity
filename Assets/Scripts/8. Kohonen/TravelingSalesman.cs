using UnityEngine;
using static UnityEngine.Mathf;

namespace Kohonen
{
    partial class TravelingSalesman : Drawing
    {
        //number of houses/output nodes
        int nHouses = 10;
        int nOuts = 10;
        House[] houses;
        Out[] outs;
        float window;
        float learning = 0.1f;

       // int winner;
       // float currentX, currentY;


        void Start()
        {
            Size(500, 500);
            // RectMode(CENTER);
            //initialise houses array
            RandomHouses();
            //initialise outs array
            SetupOuts();
        }

        void Update()
        {
            if (Input.GetKeyDown("r")) Start();
            if (Input.GetKeyDown("n")) SetupOuts();
            if (Input.GetKeyDown("f")) ScreenCapture.CaptureScreenshot(Random.Range(0,1000) + ".png");

            window *= 0.99f;
            learning *= 0.999f;
            print(window);
            TrainAll();

            Background(255);
            DisplayHouses();
            DisplayOuts();
        }

        void RandomHouses()
        {
            houses = new House[nHouses];
            for (int i = 0; i < nHouses; i++)
            {
                houses[i] = new House();
            }
        }

        void DisplayHouses()
        {
            NoFill();
            Stroke(255, 0, 0);
            for (int i = 0; i < nHouses; i++)
            {
                houses[i].Display();
            }
        }

        void SetupOuts()
        {
            outs = new Out[nOuts];
            for (int i = 0; i < nOuts; i++)
            {
                outs[i] = new Out();
            }
            window = 2.0f;
            learning = 0.1f;
        }

        void DisplayOuts()
        {
            NoFill();
            Stroke(0);
            for (int j = 0; j < nOuts; j++)
            {
                int j2 = (j + 1) % nOuts;
                Line(outs[j].w[0], outs[j].w[1], outs[j2].w[0], outs[j2].w[1]);
            }
        }

        void TrainAll()
        {
            for (int i = 0; i < houses.Length; i++)
            {
                TrainOne(houses[i]);
            }
        }

        void TrainOne(House h)
        {
            int won = FindWinner(h);
            UpdateWinners(h, won);
        }

        int FindWinner(House h)
        {
            int win = 0;
            float minDist = Width + Height;

            for (int i = 0; i < outs.Length; i++)
            {
                float d = Dist(outs[i].w[0], outs[i].w[1], h.pos[0], h.pos[1]);
                if (d < minDist)
                {
                    minDist = d;
                    win = i;
                }
            }

            return win;
        }

        void UpdateWinners(House h, int winner)
        {
            for (int j = 0; j < outs.Length; j++)
            {
                outs[j].w[0] += learning * Gauss(winner, j) * (h.pos[0] - outs[j].w[0]);
                outs[j].w[1] += learning * Gauss(winner, j) * (h.pos[1] - outs[j].w[1]);
            }
        }

        float Gauss(int k, int j)
        {
            float f = Exp(-Sq(j - k) / (2 * Sq(window)));

            int l = k - nHouses;
            f += Exp(-Sq(j - l) / (2 * Sq(window)));

            l = k + nHouses;
            f += Exp(-Sq(j - l) / (2 * Sq(window)));

            return f;
        }
    }
}
