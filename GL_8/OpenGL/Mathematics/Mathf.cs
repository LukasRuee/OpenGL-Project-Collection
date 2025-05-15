namespace OpenGL.Mathematics
{
    /// <summary>
    /// Math helper class.
    /// </summary>
    public static class Mathf
    {
        /// <summary>
        /// Math constant PI.
        /// </summary>
        public const float PI = 3.14159265358979323846264338327f;

        /// <summary>
        /// Math constant 2*PI
        /// </summary>
        public const float TWO_PI = 6.28318530717958647692528676654f;


        /// <summary>
        ///   <para>Linearly interpolates between a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Mathf.Clamp01(t);
        }

        /// <summary>
        ///   <para>Clamps value between 0 and 1 and returns value.</para>
        /// </summary>
        /// <param name="value"></param>
        public static float Clamp01(float value)
        {
            return Clamp(value, 0.0f, 1.0f);
        }

        /// <summary>
        ///   <para>Clamps a value between a minimum float and maximum float value.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static float Clamp(float value, float min, float max)
        {
            if ((double)value < (double)min) { value = min; }
            else if ((double)value > (double)max) { value = max; }

            return value;
        }

        /// <summary>
        ///   <para>Returns the specified angle deg in radians.</para>
        /// </summary>
        /// <param name="deg">Angle in degrees</param>
        public static float ToRad(float deg)
        {
            return (deg / 360.0f) * TWO_PI;
        }

        /// <summary>
        ///   <para>Returns the specified angle rad in degrees.</para>
        /// </summary>
        /// <param name="rad">Angle in radians</param>
        public static float ToDeg(float rad)
        {
            // TODO: Implement to radian conversion
            return -1;
        }

        /// <summary>
        ///   <para>Returns the sine of angle f in radians.</para>
        /// </summary>
        /// <param name="f">Angle in radians</param>
        public static float Sin(float f)
        {
            return (float)System.Math.Sin((double)f);
        }

        /// <summary>
        ///   <para>Returns the cosine of angle f in radians.</para>
        /// </summary>
        /// <param name="f">Angle in radians</param>
        public static float Cos(float f)
        {
            return (float)System.Math.Cos((double)f);
        }

        /// <summary>
        ///   <para>Returns the tangent of angle f in radians.</para>
        /// </summary>
        /// <param name="f">Angle in radians</param>
        public static float Tan(float f)
        {
            return (float)System.Math.Tan((double)f);
        }
    }
}
