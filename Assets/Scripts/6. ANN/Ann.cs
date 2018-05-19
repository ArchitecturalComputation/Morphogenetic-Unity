using UnityEngine;
using static UnityEngine.Mathf;

namespace Ann
{
    partial class Ann : Drawing
    {
        // Simple neural nets
        // (c) Alasdair Turner 2009

        // Free software: you can redistribute this program and/or modify
        // it under the terms of the GNU General Public License as published by
        // the Free Software Foundation, either version 3 of the License, or
        // (at your option) any later version.

        Network neuralnet;

        // PFont font;

        void Start()
        {
            Size(400, 400);

            // font = loadFont("LucidaSans-20.vlw");
            // TextFont(font);

            SetupSigmoid();
            LoadData();

            neuralnet = new Network(196, 49, 10);

            Background(220, 204, 255);
            NoStroke();
            //smooth();
            PushMatrix();
            neuralnet.Draw();
            PopMatrix();
            Fill(0);
        }

        bool b_dataloaded = false;

        // left click to test, right click (or ctrl+click on a Mac) to train
        bool b_train = false, b_test = false;
        int response = -1, actual = -1;

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
                b_test = true;
            if (Input.GetMouseButtonUp(1))
                b_train = true;


            if (!b_dataloaded)
            {
                LoadData();
                b_dataloaded = true;
                b_test = true;
            }
            if (b_train)
            {
                // this allows some fast training without displaying:
                for (int i = 0; i < 500; i++)
                {
                    // select a random training input and train
                    int row = (int)(Random.Range(0, training_set.Length));
                    response = neuralnet.Respond(training_set[row].inputs);
                    actual = training_set[row].output;
                    neuralnet.Train(training_set[row].outputs);
                }
            }
            else if (b_test)
            {
                int row = (int)(Random.Range(0, testing_set.Length));
                response = neuralnet.Respond(testing_set[row].inputs);
                actual = testing_set[row].output;
            }
            //if (b_train || b_test)
            {
                // draw
                // Background(220, 204, 255);
                // NoStroke();
                //smooth();
                PushMatrix();
                neuralnet.Draw();
                PopMatrix();
                b_train = b_test = false;
                Fill(0);
                Text(response.ToString(), 350, 27);
                Text(actual.ToString(), 350, 275);
            }
            // Fill(0);
        }


        // Simple neural nets: sigmoid functions
        // (c) Alasdair Turner 2009

        // Free software: you can redistribute this program and/or modify
        // it under the terms of the GNU General Public License as published by
        // the Free Software Foundation, either version 3 of the License, or
        // (at your option) any later version.

        // a sigmoid function is the neuron's response to inputs
        // the sigmoidal response ranges from -1.0 to 1.0

        // for example, the weighted sum of inputs might be "2.1"
        // our response would be lookupSigmoid(2.1) = 0.970

        // this is a look up table for sigmoid (neural response) values
        // which is valid from -5.0 to 5.0
        static float[] g_sigmoid = new float[200];

        // this function precalculate a sigmoid (response) function
        static void SetupSigmoid()
        {
            for (int i = 0; i < 200; i++)
            {
                float x = (i / 20.0f) - 5.0f;
                g_sigmoid[i] = 2.0f / (1.0f + Exp(-2.0f * x)) - 1.0f;
            }
        }

        // once the sigmoid has been set up, this function accesses it:
        static float LookupSigmoid(float x)
        {
            return g_sigmoid[Clamp((int)((x + 5.0) * 20.0), 0, 199)];
        }
    }
}