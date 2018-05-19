using UnityEngine;

namespace CA
{
    class CA1D : Drawing
    {
        // Simple Wolfram CA
        int row = 1;
        Color black = Color.black;
        // rule 110
        // 2 + 4 + 8 + 32 + 64 = 110
        int[] response = { 0, 1, 1, 1, 0, 1, 1, 0 };

        void Start()
        {
            Size(200, 400);
            Set(0, 0, black);
        }

        void Update()
        {
            for (int i = 0; i < Width; i++)
            {
                Color a = Get((i + Width - 1) % Width, row - 1);
                Color b = Get(i, row - 1);
                Color c = Get((i + Width + 1) % Width, row - 1);

                int number = 0;

                if (a == black)
                {
                    number += 1;
                }
                if (b == black)
                {
                    number += 2;
                }
                if (c == black)
                {
                    number += 4;
                }
                if (response[number] == 1)
                {
                    Set(i, row, black);
                }
            }

            row++;
        }
    }
}