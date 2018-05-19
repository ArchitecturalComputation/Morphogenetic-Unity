
namespace Ann
{
    partial class Ann : Drawing
    {
        // Simple neural nets: load data
        // (c) Alasdair Turner 2009

        // Free software: you can redistribute this program and/or modify
        // it under the terms of the GNU General Public License as published by
        // the Free Software Foundation, either version 3 of the License, or
        // (at your option) any later version.

        // this uses the MNIST database of handwritten digits
        // http://yann.lecun.com/exdb/mnist/ (accessed 04.06.09)
        // Yann LeCun and Corinna Cortes

        // note: I have reduced the originals to 14 x 14 from 28 x 28

        static Datum[] training_set;
        static Datum[] testing_set;

        class Datum
        {
            public float[] inputs;
            public float[] outputs;
            public int output;

            public Datum()
            {
                inputs = new float[196];
                outputs = new float[10];
            }

            public void ImageLoad(byte[] images, int offset)
            {
                for (int i = 0; i < 196; i++)
                {
                    // note, you must use int() to convert rather than (int) to cast:
                    inputs[i] = (int)(images[i + offset]) / 128.0f - 1.0f;
                }
            }

            public void LabelLoad(byte[] labels, int offset)
            {
                output = (int)labels[offset];
                for (int i = 0; i < 10; i++)
                {
                    if (i == output)
                    {
                        outputs[i] = 1.0f;
                    }
                    else
                    {
                        outputs[i] = -1.0f;
                    }
                }
            }
        }

        void LoadData()
        {
            byte[] images = LoadBytes("t10k-images-14x14.idx3-ubyte");
            byte[] labels = LoadBytes("t10k-labels.idx1-ubyte");

            training_set = new Datum[8000];
            int tr_pos = 0;
            testing_set = new Datum[2000];
            int te_pos = 0;

            for (int i = 0; i < 10000; i++)
            {
                if (i % 5 != 0)
                {
                    training_set[tr_pos] = new Datum();
                    training_set[tr_pos].ImageLoad(images, 16 + i * 196);
                    training_set[tr_pos].LabelLoad(labels, 8 + i);
                    tr_pos++;
                }
                else
                {
                    testing_set[te_pos] = new Datum();
                    testing_set[te_pos].ImageLoad(images, 16 + i * 196);
                    testing_set[te_pos].LabelLoad(labels, 8 + i);
                    te_pos++;
                }
            }
        }
    }
}