using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 AddX(this Vector3 vector, float value)
        {
            vector.x += value;
            return vector;
        }
        public static Vector3 AddY(this Vector3 vector, float value)
        {
            vector.y += value;
            return vector;
        }
        public static Vector3 AddZ(this Vector3 vector, float value)
        {
            vector.z += value;
            return vector;
        }
    }
}
