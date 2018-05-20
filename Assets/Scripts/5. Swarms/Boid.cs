using UnityEngine;
using static Drawing;
using static UnityEngine.Mathf;

namespace Swarms
{
    struct Boid
    {
        Matrix4x4 matrix;
        float speed;
        Color colour;
        float _maxSeparation;
        int _index;

        public Boid(int index)
        {
            _index = index;
            matrix = Matrix4x4.identity;
            matrix *= Matrix4x4.Translate(
                new Vector3(
                      Random.Range(-Width / 2f, Width / 2f),
                      Random.Range(-Height / 2f, Height / 2f),
                      Random.Range(-Height / 2f, Height / 2f))
                      );
            matrix *= Matrix4x4.Rotate(
                 Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), 0)
                        );
            speed = Random.Range(1.0f, 2.0f);
            colour = Random.ColorHSV();

            _maxSeparation = Width / 4f;
            _maxSeparation *= _maxSeparation;
        }

        public void Draw()
        {
            Fill(colour);
            PushMatrix();
            ApplyMatrix(ref matrix);

            Shape(
            new Vector3(-6, 0, -8),
            new Vector3(0, 0, 8),
            new Vector3(6, 0, -8)
            );

            Shape(
            new Vector3(0, 3, -8),
            new Vector3(0, 0, 8),
            new Vector3(0, 0, -8)
            );

            PopMatrix();
        }

        public void Move(Boid[] flock)
        {
            Matrix4x4 inverse = matrix.inverse;

            bool cohere = false;
            bool separate = false;
            Vector3 neighbourcentre = Vector3.zero;
            Vector3 neighbourdir = Vector3.zero;
            Vector3 kernelcentre = Vector3.zero;

            for (int i = 0; i < flock.Length; i++)
            {
                if (i != this._index)
                {
                    Matrix4x4 mat_i;
                    //  mat_i = inverse * flock[i].matrix;
                    Multiply(ref inverse, ref flock[i].matrix, out mat_i);
                    Vector3 p_i; Pos(out p_i, ref mat_i);

                    float separation = p_i.sqrMagnitude;
                    if (separation < _maxSeparation)
                    {
                        Vector3 d_i; Dir(out d_i, ref mat_i);

                        if (d_i.z > -0.25f)
                        {
                            // neighbourcentre += p_i;
                            Add(ref neighbourcentre, ref p_i);
                            //neighbourdir += d_i;
                            Add(ref neighbourdir, ref d_i);
                            cohere = true;
                            if (separation < 10)
                            {
                                //kernelcentre += p_i;
                                Add(ref kernelcentre, ref p_i);
                                separate = true;
                            }
                        }
                    }
                }
            }

            if (separate)
            {
                float angle = 180 / 12f;
                if (kernelcentre.x > 0) angle = -180 / 12f;
                Rotate(Quaternion.AngleAxis(angle, Vector3.up), ref matrix);
            }
            else if (cohere)
            {
                neighbourcentre.Normalize();
                neighbourdir.Normalize();
                Vector3 desiredir = Vector3.zero;
                desiredir += neighbourcentre;
                desiredir += neighbourdir;
                desiredir.Normalize();
                float ang = Acos(desiredir.z);
                if (ang > 0.01f)
                {
                    var q = Quaternion.AngleAxis(ang * (0.1f * Rad2Deg), new Vector3(-desiredir.y, desiredir.x, 0.0f));
                    Rotate(q, ref matrix);
                }
            }

            Translate(new Vector3(0, 0, speed), ref matrix);
        }

        void Pos(out Vector3 pos, ref Matrix4x4 matrix)
        {
            pos.x = matrix.m03; pos.y = matrix.m13; pos.z = matrix.m23;
        }

        void Dir(out Vector3 dir, ref Matrix4x4 matrix)
        {
            dir.x = matrix.m02; dir.y = matrix.m12; dir.z = matrix.m22;
        }

        void Add(ref Vector3 a, ref Vector3 b)
        {
            a.x += b.x;
            a.y += b.y;
            a.z += b.z;
        }
    }
}