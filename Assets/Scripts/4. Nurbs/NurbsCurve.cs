using UnityEngine;
using static UnityEngine.Mathf;

namespace Nurbs
{
    class NurbsCurve : Drawing
    {
        //control points
        Vector3[] cPts =
            {
              new Vector3(60.0f,100.0f),
              new Vector3(160.0f,380.0f),
              new Vector3(340.0f,140.0f),
              new Vector3(200.0f,60.0f)
            };

        //number of transitions - knot spans
        int N;

        //knot vector
        //float[] knots = {0.0, 0.0001, 0.0002, 0., 0.998, 0.999, 1.0}; 
        //float[] knots = {0.0, 0.2, 0.4, 0.5, 0.55, 0.6, 0.8, 1.0}; 
        float[] knots = { 0.00f, 0.0001f, 0.0002f, 0.0003f, 0.9997f, 0.9998f, 0.9999f, 1.0f };//0.0, 0.167, 0.333, 0.5, 0.667, 0.833, 1.0};

        //Degree: 
        //number of knots = number of control points  +  degree  +  1  =>  
        //degree = number of knots  -  number of control points  -1
        // Remember that Order = Degree + 1
        int D;

        void Start()
        {
            N = cPts.Length - 1;
            D = knots.Length - cPts.Length - 1;
            print(D);

            Size(400, 400);
            Background(0);

            ColorMode(ColorModes.HSB, 1);
        }

        void Update()
        {
            //draw control points
            StrokeWeight(16);
            for (int i = 0; i < cPts.Length; ++i)
            {
                FaderColor(i);
                Point(cPts[i].x, cPts[i].y);
            }

            //draw "basis function" curve 
            StrokeWeight(1);
            for (float u = 0; u <= 1.0; u += 0.01f)
            {
                FaderPos(u);
            }

            //draw curve
            StrokeWeight(4);
            Stroke(1);
            //SOS change for NURBS
            for (float u = knots[D]; u <= knots[knots.Length - D - 1]; u += 0.001f)
            //for(float u=0; u<=1; u+=0.001)
            {
                CurvePos(u);
            }

            //draw knot spans
            StrokeWeight(1);
            Stroke(1, 1, 1);
            for (int i = 0; i < knots.Length; i++)
            {
                Line(knots[i] * Width, 0, knots[i] * Width, Height);
            }
        }

        //To find the curve position each time we need to multiply each control point's position with its basis function and sum them up
        void CurvePos(float u)
        {
            Vector3 pt = new Vector3();
            for (int k = 0; k < cPts.Length; k++)//cPts.length
            {
                Vector3 pt_k = new Vector3(cPts[k].x, cPts[k].y);
                //multiply control point's position with its equivalent basis function
                pt_k *= Fader(u, k);
                //add the above to find the point on the curve for a given u value (u=parameter space of curve, 0<=u<=1)
                pt += pt_k;
            }

            Point(pt.x, pt.y);
        }

        void FaderColor(int k)
        {
            Stroke((float)k / (N + 1), 1, 1);
        }

        void FaderPos(float u)
        {
            for (int k = 0; k < cPts.Length; k++)//cPts.length
            {
                float faderLvl = Fader(u, k);
                FaderColor(k);
                //0<=u<=1
                //when faderLvl=0 we want to start for the bottom of the screen, thus having a notional graph
                //that's why we multiply height with 1-faderLvl, so that our notional (0,0) is at the lower left corner of the screen
                Point(u * Width, (1 - faderLvl) * Height);
            }
        }

        //linear interpolation between 2 points
        float Fader1(float u, int k)
        {
            float faderLvl;
            if (k == 0)
            {
                faderLvl = 1.0f - u;
            }
            else
            {
                faderLvl = u - 0;
            }

            return faderLvl;
        }

        //linear interpolation between many points
        float Fader2(float u, int k)
        {
            float faderLvl = 0;
            u *= N;
            if (k == FloorToInt(u))
            {
                faderLvl = Ceil(u) - u;
            }
            else if (k == CeilToInt(u))
            {
                faderLvl = u - Floor(u);
            }

            return faderLvl;
        }

        //Blending Function for a Bezier Curve
        //does not allow for knot manipulation
        //number of control points = order
        float Fader3(float u, int k)
        {
            float faderLvl = Pow(u, k) * Pow(1 - u, N - k) * Fac(N) / (Fac(k) * Fac(N - k));

            return faderLvl;
        }

        //calculate factorial of a number
        float Fac(int v)
        {
            float f = 1;
            for (int i = v; i > 1; i--)
            {
                f *= i;
            }
            return f;
        }

        //Basis Function for Degree 1 curves
        float Fader4(float u, int k)
        {
            return Basis1a(u, k);
        }
        float Basis1a(float u, int k)
        {
            float b1 = Basis0a(u, k) * (u - knots[k]) / (knots[k + 1] - knots[k]);
            float b2 = Basis0a(u, k + 1) * (knots[k + 2] - u) / (knots[k + 2] - knots[k + 1]);
            return b1 + b2;
        }
        float Basis0a(float u, int k)
        {
            if (u >= knots[k] && u < knots[k + 1]) return 1;
            else return 0;
        }

        //Generalised Basis Function
        float Fader(float u, int k)
        {
            return Basisn(u, k, D);
        }

        float Basisn(float u, int k, int d)
        {
            if (d == 0)
            {
                return Basis0(u, k);
            }
            else
            {
                float b1 = Basisn(u, k, d - 1) * (u - knots[k]) / (knots[k + d] -
                  knots[k]);
                float b2 = Basisn(u, k + 1, d - 1) * (knots[k + d + 1] - u) / (knots[k + d + 1]
                  - knots[k + 1]);
                return b1 + b2;
            }
        }

        float Basis0(float u, int k)
        {
            if (u >= knots[k] && u < knots[k + 1]) return 1;
            else return 0;
        }
    }
}