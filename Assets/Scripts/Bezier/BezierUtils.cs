using UnityEngine;

namespace Assets.Scripts.Bezier
{
    public static class BezierUtils
    {
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            // 2. Optimized calculation for pow(x, n)
            float oneMinusT = 1f - t;
            float omt2 = oneMinusT * oneMinusT;
            float t2 = t * t;

            return
                omt2 * oneMinusT * p0 +             // oneMinusT^3 * p0
                3f * omt2 * t * p1 +                // 3 * oneMinusT^2 * t * p1
                3f * oneMinusT * t2 * p2 +          // 3 * oneMinusT * t^2 * p2
                t2 * t * p3;                        // t^3 * p3
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            // 2. Optimized calculation for pow(x, n)
            float oneMinusT = 1f - t;
            float omt2 = oneMinusT * oneMinusT;
            float t2 = t * t;

            return
                3f * omt2 * (p1 - p0) +             // 3 * oneMinusT^2 * (p1 - p0)
                6f * oneMinusT * t * (p2 - p1) +
                3f * t2 * (p3 - p2);                // 3 * t^2 * (p3 - p2)
        }
    }
}