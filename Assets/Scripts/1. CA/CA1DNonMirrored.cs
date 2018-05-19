using UnityEngine;

namespace CA
{
    class CA1DNonMirrored : Drawing
    {
        Color black = Color.black;
        int row = 1;

        // rule 110
        // 2 + 4 + 8 + 32 + 64 = 110
        int[] responce = { 0, 1, 1, 1, 0, 1, 1, 0 };

        void Start()
        {
            Size(400, 400);
            //initial state t=0
            Set(399, 0, black);
        }

        void Update()
        {
            for (int i = 0; i < Width; i++)
            {
                Color a = Get((i - 1 + Width) % Width, row - 1);
                Color b = Get(i, row - 1);
                Color c = Get((i + 1 + Width) % Width, row - 1);

                int num = 0;

                if (a == black)
                {
                    num += 4;
                }
                if (b == black)
                {
                    num += 2;
                }
                if (c == black)
                {
                    num += 1;
                }

                if (responce[num] == 1)
                {
                    Set(i, row, black);
                }
            }

            row++;
        }
    }
}