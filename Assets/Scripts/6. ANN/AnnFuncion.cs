using UnityEngine;
using static UnityEngine.Mathf;


namespace Ann.Function
{
    class AnnFuncion : Drawing
    {
        // Simple neural nets
        // (c) Alasdair Turner 2009
        // Example using functions: Stamatios Psarras 2017

        // Free software: you can redistribute this program and/or modify
        // it under the terms of the GNU General Public License as published by
        // the Free Software Foundation, either version 3 of the License, or
        // (at your option) any later version.

        Network neuralnet;

        ////////////////////////////
        //Training / Testing Set
        float[] y, x;
        int max_data = 2000;
        float[] xx;
        //PFont font;
        ////////////////////////////

        void Start()
        {
            Size(400, 400);

            //font = loadFont("LucidaSans-20.vlw");
            // textFont(font);

            SetupSigmoid();
            //loadData();

            neuralnet = new Network(1, 20, 1);

            Background(220, 204, 255);
            //NoStroke();
            //smooth();
            PushMatrix();
            //neuralnet.draw();
            PopMatrix();
            Fill(0);

            x = XRange(max_data);
            xx = XRange(200);
            y = Equations(x);//linear(x, -0.5, 0);
        }

        //bool b_dataloaded = false;

        // left click to test, right click (or ctrl+click on a Mac) to train
        bool b_train = false, b_test = false;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                b_test = !b_test;
                print($"{b_test} {b_train}");
            }
            if (Input.GetMouseButtonDown(1))
            {
                b_train = !b_train;
                print($"{b_test} {b_train}");
            }


            StrokeWeight(1.5f);
            //int response = -1;
            //float actual = -1;
            //if (!b_dataloaded) {
            //  loadData();
            //  b_dataloaded = true;
            //  b_test = true;
            //}
            if (b_train)
            {
                // this allows some fast training without displaying:
                for (int i = 0; i < 500; i++)
                {
                    // select a random training input and train
                    int row = Random.Range(0, max_data);
                    //float trainingInput = y[row];
                    float[] trainingInput = new float[1];
                    trainingInput[0] = x[row];
                    neuralnet.Respond(trainingInput);
                    float[] trainOutput = new float[1];//actual = x[row];
                    trainOutput[0] = y[row];
                    neuralnet.Train(trainOutput);
                }
            }
            if (b_test)
            {
                for (int i = 0; i < xx.Length; i++)
                {
                    //int row = (int) floor(Random.Range(0, max_data));
                    float[] testInput = new float[1];
                    testInput[0] = xx[i];
                    //Response now return an array with all the outputs, in this case just one but we still need to call it by its index, 0
                    float response = neuralnet.Respond(testInput)[0];
                    Stroke(255, 0, 0);
                    Point((xx[i] + 1) * Width / 2.0f, (response + 1) * Height / 2.0f);
                    //print(".");
                    //if(response > 0.01)
                    //print(response);

                    //float[] trainOutput = new float[1];//actual = x[row];
                    //trainOutput[0] = y[row];
                }
            }
            //if (b_train || b_test) {
            // draw
            Stroke(255);
            for (int i = 0; i < x.Length; i++)
            {
                Point((x[i] + 1f) * Width / 2.0f, (y[i] + 1f) * Height / 2.0f);
            }
            //background(220, 204, 255);
            //noStroke();
            //smooth();
            //pushMatrix();
            neuralnet.Draw();
            //popMatrix();
            //b_train = b_test = false;
            //fill(0);
            //text(str(response), 350, 27);
            //text(str(actual), 350, 275);
            //}
            Fill(0);


        }

        // Returns the y of a given equation
        float[] Equations(float[] x)
        {
            float[] y = new float[x.Length];
            //float[] x = xRange(num);
            // y = a * x + b;
            for (int i = 0; i < x.Length; i++)
            {
                //y[i] = a * x[i] + b;
                y[i] = x[i] * x[i];
                y[i] = Sqrt(Abs(x[i]));
                //y[i] = cos(abs(x[i]));
            }
            return y;
        }

        // Returns the y of a given equation
        float[] Linear(float[] x, float a, float b)
        {
            float[] y = new float[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                y[i] = a * x[i] + b;
            }
            return y;
        }

        // return numbers from (-1, 1)
        float[] XRange(int num)
        {
            float[] x = new float[num];
            for (int i = 0; i < num; i++)
            {
                x[i] = 2 * i / (float)num - 1f;
            }
            return x;
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
            return g_sigmoid[Clamp((int)((x + 5.0f) * 20.0f), 0, 199)];
        }

        // Simple neural nets: network
        // (c) Alasdair Turner 2009

        // Free software: you can redistribute this program and/or modify
        // it under the terms of the GNU General Public License as published by
        // the Free Software Foundation, either version 3 of the License, or
        // (at your option) any later version.

        // This class is for the neural network,
        // which is hard coded with three layers:
        // input, hidden and output

        class Network
        {
            // this network is hard coded to only have one hidden layer
            Neuron[] m_input_layer;
            Neuron[] m_hidden_layer;
            Neuron[] m_output_layer;
            // create a network specifying numbers of inputs, hidden layer neurons
            // and number of outputs, e.g. Network(4,4,3)
            public Network(int inputs, int hidden, int outputs)
            {
                m_input_layer = new Neuron[inputs];
                m_hidden_layer = new Neuron[hidden];
                m_output_layer = new Neuron[outputs];
                // set up the network topology
                for (int i = 0; i < m_input_layer.Length; i++)
                {
                    m_input_layer[i] = new Neuron();
                }
                // route the input layer to the hidden layer
                for (int j = 0; j < m_hidden_layer.Length; j++)
                {
                    m_hidden_layer[j] = new Neuron(m_input_layer);
                }
                // route the hidden layer to the output layer
                for (int k = 0; k < m_output_layer.Length; k++)
                {
                    m_output_layer[k] = new Neuron(m_hidden_layer);
                }
            }

            //Will return all the outputs
            public float[] Respond(float[] inputs)
            {
                float[] responses = new float[m_output_layer.Length];
                // feed forward
                // simply set the input layer to display the inputs
                for (int i = 0; i < m_input_layer.Length; i++)
                {
                    m_input_layer[i].m_output = inputs[i];
                }
                // now feed forward through the hidden layer
                for (int j = 0; j < m_hidden_layer.Length; j++)
                {
                    m_hidden_layer[j].Respond();
                }
                // and finally feed forward to the output layer
                for (int k = 0; k < m_output_layer.Length; k++)
                {
                    responses[k] = m_output_layer[k].Respond();
                }
                // now check the best response:
                //int response = -1;
                //float best = max(responses);
                //for (int a = 0; a < responses.Length; a++) {
                //  if (responses[a] == best) {
                //    response = a;
                //  }
                //}
                return responses;
            }

            public void Train(float[] outputs)
            {
                // adjust the output layer
                for (int k = 0; k < m_output_layer.Length; k++)
                {
                    m_output_layer[k].Finderror(outputs[k]);
                    m_output_layer[k].Train();
                }
                // propagate back to the hidden layer
                for (int j = 0; j < m_hidden_layer.Length; j++)
                {
                    m_hidden_layer[j].Train();
                }
                // the input layer doesn't learn:
                // it is simply the inputs
            }

            public void Draw()
            {
                // note, this draw is hard-coded for Network(196,49,10)
                // which reflects my use of the MNIST database of handwritten digits
                //for (int i = 0; i < m_input_layer.Length; i++) {
                //  pushMatrix();
                //  translate((i%14) * Width / 25.0 + Width * 0.22, (i/14) * Height / 25.0 + Height * 0.42);
                //  m_input_layer[i].draw(true);
                //  popMatrix();
                //}
                for (int j = 0; j < m_hidden_layer.Length; j++)
                {
                    PushMatrix();
                    Translate((j % 7) * Width / 25.0f + Width * 0.36f, (j / 7) * Height / 25.0f + Height * 0.12f);
                    m_hidden_layer[j].Draw(false);
                    PopMatrix();
                }
                // this is slightly tricky -- I've switched the order so the output
                // neurons are arrange 1,2,3...8,9,0 rather than 0,1,2...7,8,9
                // (that's what the (k+9) % 10 is doing)
                //for (int k = 0; k < m_output_layer.Length; k++) {
                //  pushMatrix();
                //  translate(((k+9)%10) * Width / 20.0 + Width * 0.25, Height * 0.05);
                //  m_output_layer[k].draw(true);
                //  popMatrix();
                //}
            }
        }

        // Simple neural nets: neuron
        // (c) Alasdair Turner 2009

        // Free software: you can redistribute this program and/or modify
        // it under the terms of the GNU General Public License as published by
        // the Free Software Foundation, either version 3 of the License, or
        // (at your option) any later version.

        // This class is for each neuron.  It works
        // as a feed-forward multilayer perceptron, 
        // learning by backpropagation

        static float LEARNING_RATE = 0.01f;

        class Neuron
        {
            Neuron[] m_inputs;
            float[] m_weights;
            float m_threshold;
            public float m_output;
            float m_error;
            // the input layer of neurons have no inputs:
            public Neuron()
            {
                m_threshold = 0.0f;
                m_error = 0.0f;
                // initial random output
                m_output = 0;//lookupSigmoid(Random.Range(-5.0,5.0));
            }
            // all other layers (hidden and output) have 
            // neural inputs
            public Neuron(Neuron[] inputs)
            {
                m_inputs = new Neuron[inputs.Length];
                m_weights = new float[inputs.Length];
                for (int i = 0; i < inputs.Length; i++)
                {
                    m_inputs[i] = inputs[i];
                    m_weights[i] = Random.Range(-1.0f, 1.0f);
                }
                m_threshold = Random.Range(-1.0f, 1.0f);
                m_error = 0.0f;
                // initial random output
                m_output = LookupSigmoid(Random.Range(-5.0f, 5.0f));
            }

            // respond looks at the layer below, and prepares a response:
            public float Respond()
            {
                float input = 0.0f;
                for (int i = 0; i < m_inputs.Length; i++)
                {
                    input += m_inputs[i].m_output * m_weights[i];
                }
                m_output = LookupSigmoid(input + m_threshold);
                // reset our error value ready for training
                m_error = 0.0f;
                return m_output;
            }

            // find error is used on the output neurons
            public void Finderror(float desired)
            {
                m_error = desired - m_output;
            }

            // train adjusts the weights by comparing actual output to correct output
            public void Train()
            {
                float delta = (1.0f - m_output) * (1.0f + m_output) * m_error * LEARNING_RATE;
                for (int i = 0; i < m_inputs.Length; i++)
                {
                    // tell the next layer down what it's doing wrong
                    m_inputs[i].m_error += m_weights[i] * m_error;
                    // correct our weights
                    m_weights[i] += m_inputs[i].m_output * delta;
                }
            }

            public void Draw(bool not_hidden)
            {
                float level = (0.5f - m_output * 0.5f);
                if (not_hidden)
                {
                    Fill((int)(255 * level));
                }
                else
                {
                    // merge hidden layer with background color
                    Fill(110 + 128 * level, 102 + 128 * level, 127 + 128 * level);
                }

                Ellipse(0, 0, 16, 16);
            }
        }
    }
}
