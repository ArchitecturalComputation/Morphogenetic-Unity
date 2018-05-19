using static UnityEngine.Mathf;

namespace Ann
{
    partial class Ann : Drawing
    {
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

            public int Respond(float[] inputs)
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
                int response = -1;
                float best = Max(responses);
                for (int a = 0; a < responses.Length; a++)
                {
                    if (responses[a] == best)
                    {
                        response = a;
                    }
                }
                return response;
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
                for (int i = 0; i < m_input_layer.Length; i++)
                {
                    PushMatrix();
                    Translate((i % 14) * Width / 25.0f + Width * 0.22f, (i / 14) * Height / 25.0f + Height * 0.42f);
                    m_input_layer[i].Draw(true);
                    PopMatrix();
                }

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
                for (int k = 0; k < m_output_layer.Length; k++)
                {
                    PushMatrix();
                    Translate(((k + 9) % 10) * Width / 20.0f + Width * 0.25f, Height * 0.05f);
                    m_output_layer[k].Draw(true);
                    PopMatrix();
                }
            }
        }

    }
}