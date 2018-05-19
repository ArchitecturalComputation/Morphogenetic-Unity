using UnityEngine;

namespace CA
{
    class CA2DGameOfLife : Drawing
    {
        int xNum = 50;
        int yNum = 50;
        int cellSize = 10;
        int xSize;
        int ySize;

        bool[,] state;
        bool[,] nextState;

        void Start()
        {
            xSize = xNum * cellSize;
            ySize = yNum * cellSize;

            Size(xSize, ySize);

            state = new bool[xNum, yNum];
            nextState = new bool[xNum, yNum];

            Glider(25, 25);
            Background(255);
            Fill(0);
        }

        void Update()
        {
            for (int i = 0; i < xNum; i++)
            {
                for (int j = 0; j < yNum; j++)
                {
                    if (state[i, j] == true)
                    {
                        Rect(i * cellSize, j * cellSize, cellSize, cellSize);
                    }
                    //count alive neighbours
                    int aliveCells = 0;
                    for (int m = -1; m <= 1; m++)
                    {
                        for (int k = -1; k <= 1; k++)
                        {
                            if (!(m == 0 && k == 0))
                            {
                                if (state[(m + i + xNum) % xNum, (k + j + yNum) % yNum] == true)
                                {
                                    aliveCells++;
                                }
                            }
                        }
                    }
                    //set next generation
                    if (aliveCells < 2 || aliveCells > 3)
                    {
                        nextState[i, j] = false;
                    }
                    else if (aliveCells == 3)
                    {
                        nextState[i, j] = true;
                    }
                    else
                    {
                        nextState[i, j] = state[i, j];
                    }
                }
            }

            for (int i = 0; i < xNum; i++)
            {
                for (int j = 0; j < yNum; j++)
                {
                    state[i, j] = nextState[i, j];
                }
            }
        }

        void Glider(int x, int y)
        {
            state[x, y] = true;
            state[x - 1, y] = true;
            state[x + 1, y] = true;
            state[x + 1, y - 1] = true;
            state[x, y - 2] = true;
        }
    }
}