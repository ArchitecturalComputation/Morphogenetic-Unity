using UnityEngine;

namespace Ann
{
    partial class Ann : Drawing
    {
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
                m_output = LookupSigmoid(Random.Range(-5.0f, 5.0f));
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