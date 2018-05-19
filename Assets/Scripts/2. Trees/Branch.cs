using UnityEngine;
using static Drawing;

namespace Trees
{
    class Branch
    {
        Matrix4x4 joint;
        Branch child_a;
        Branch child_b;

        public Branch()
        {
            joint = Matrix4x4.identity;
            child_a = null;
            child_b = null;
        }

        public void Replace(int depth)
        {
            // still need to prevent infinite recursion:
            if (depth > 16)
            {
                return;
            }
            // note: grow children before yourself
            if (child_a != null)
            {
                child_a.Replace(depth + 1);
            }
            if (child_b != null)
            {
                child_b.Replace(depth + 1);
            }
            // now I grow myself
            Rule();
        }

        void Rule()
        {
            // rule: grow offshoot branches
            // B -> B (+B) (-B)
            if (child_a == null)
            {
                // main stem -- almost straight
                child_a = new Branch();
                child_a.joint *= Matrix4x4.Rotate(Quaternion.Euler(
                    Random.Range(-180 / 16f, 180 / 16f),
                    Random.Range(0, 180f),
                    0));
            }
            if (child_b == null)
            {
                // branching off
                child_b = new Branch();
                child_a.joint *= Matrix4x4.Rotate(Quaternion.Euler(
                    Random.Range(-180 / 4f, 180 / 4f),
                    Random.Range(0, 180f),
                    0));
            }
        }

        public void Draw()
        {
            ApplyMatrix(ref joint);
            PushMatrix();
            {
                Translate(0, -Height / 8f, 0);
                Box(10, Height / 4f, 10);
            }
            PopMatrix();
            PushMatrix();
            {
                Translate(0, -Height / 4f, 0);
                Scale(0.7f);
                if (child_a != null)
                {
                    child_a.Draw();
                }
            }
            PopMatrix();
            PushMatrix();
            {
                Translate(0, -Height / 4f, 0);
                Scale(0.7f);
                if (child_b != null)
                {
                    child_b.Draw();
                }
            }
            PopMatrix();
        }
    }
}
