using UnityEngine;

namespace Nurbs
{
    class NurbsSurface01 : Drawing
    {
        //control points
        Vector3[,] cPts;

        //number of transitions - knot spans
        int NU = 5;
        int NV = 5;


        float[] knotsU;
        float[] knotsV;

        //Degree: 
        //number of knots = number of control points  +  degree  +  1  =>  
        //degree = number of knots  -  number of control points  -1
        //Remember that Order = Degree + 1
        int DU = 2;//knotsU.length - NU - 1;
        int DV = 2;//knotsV.length - NV - 1;

        //grid spacing
        float u_spacing;
        float v_spacing;


        void Start()
        {
            print(DU);

            Size(400, 400, true);
            Background(0);
            ColorMode(ColorModes.HSB, 1.0f);
            Lights();
            //noStroke();

            MakeCtrlPts();
            MakeKnots();
        }

        void Update()
        {
            Translate(0, 0, -Height);
            //draw control points
            Fill(0.5f, 1, 1);

            for (int i = 0; i < NU; i++)
            {
                for (int j = 0; j < NV; j++)
                {
                    PushMatrix();
                    Translate(cPts[i, j].x, cPts[i, j].y, cPts[i, j].z);
                    Sphere(8);
                    PopMatrix();
                }
            }

            //draw surface
            Fill(1);

            for (float u = knotsU[DU]; u <= knotsU[knotsU.Length - DU - 1]; u += 0.01f)
            {
                for (float v = knotsV[DV]; v <= knotsV[knotsV.Length - DV - 1]; v += 0.01f)
                {
                    SurfPos(u, v);
                }
            }
        }

        void MakeCtrlPts()
        {
            // set up control points in a regular grid on the xz plane with a random height:

            u_spacing = Width / (NU - 1f);
            v_spacing = Height / (NV - 1f);

            cPts = new Vector3[NU, NV];
            for (int i = 0; i < NU; i++)
            {
                for (int j = 0; j < NV; j++)
                {
                    cPts[i, j] = new Vector3(i * u_spacing, Random.Range(0, Height), j * v_spacing);
                }
            }
        }

        void MakeKnots()
        {
            knotsU = new float[NU + DU + 1];
            knotsV = new float[NV + DV + 1];
            float counterU = 0;
            float counterV = 0;
            float counterMidU = 1.0f;
            float counterMidV = 1.0f;

            for (int i = 0; i < knotsU.Length; i++)
            {
                if (i < DU + 1)
                {
                    knotsU[i] = counterU;
                    counterU += 0.001f;
                }
                else if (i >= knotsU.Length - (DU + 1))
                {
                    counterU -= 0.001f;
                    knotsU[i] = 1.00f - counterU;
                }
                else
                {

                    knotsU[i] = counterMidU / (NU - DU); //float(NU + DU + 1 - (2*(DU+1))+1);
                    counterMidU += 1.0f;
                }
            }
            for (int j = 0; j < knotsV.Length; j++)
            {
                if (j < DV + 1)
                {
                    knotsV[j] = counterV;
                    counterV += 0.001f;
                }
                else if (j > knotsV.Length - (DV + 1))
                {
                    counterV -= 0.001f;
                    knotsV[j] = 1 - counterV;
                }
                else
                {
                    knotsV[j] = counterMidV / (NV - DV);
                    counterMidV += 1.0f;
                }
            }
            print(knotsU);
        }

        //Generalised Basis Function

        float FaderU(float u, int k)
        {
            return Basisn(u, k, DU, knotsU);
        }

        float FaderV(float v, int k)
        {
            return Basisn(v, k, DV, knotsV);
        }

        float Basisn(float uv, int k, int d, float[] knots)
        {
            if (d == 0)
            {
                return Basis0(uv, k, knots);
            }
            else
            {
                float b1 = Basisn(uv, k, d - 1, knots) * (uv - knots[k]) / (knots[k + d] -
                  knots[k]);
                float b2 = Basisn(uv, k + 1, d - 1, knots) * (knots[k + d + 1] - uv) / (knots[k + d + 1]
                  - knots[k + 1]);
                return b1 + b2;
            }
        }

        float Basis0(float uv, int k, float[] knots)
        {
            if (uv >= knots[k] && uv < knots[k + 1]) return 1;
            else return 0;
        }

        //To find the curve position each time we need to multiply each control point's position with its basis function and sum them up

        void SurfPos(float u, float v)
        {
            Vector3 pt = new Vector3();
            for (int i = 0; i < NU; i++)//cPts.length
            {
                for (int j = 0; j < NV; j++)//cPts.length
                {
                    Vector3 pt_k = new Vector3(cPts[i, j].x, cPts[i, j].y, cPts[i, j].z);
                    //for surfaces multiply faderU*faderV
                    pt_k *= (FaderU(u, i) * FaderV(v, j));
                    //add the above to find the point on the curve for a given u value (u=parameter space of curve, 0<=u<=1)
                    pt += (pt_k);
                }
            }

            PushMatrix();
            Translate(pt.x, pt.y, pt.z);
            Sphere(2);
            PopMatrix();
        }
    }
}