using System;

namespace SHA3Visualization.SHA3
{
    /// <summary>
    /// Implements a static class for bitwise operations.
    /// </summary>
    public static class Bin
    {
        private static readonly int[] B = new int[] { 2, 12, 240, 65280, -65536 };
        private static readonly int[] S = new int[] { 1, 2, 4, 8, 16 };

        /// <summary>
        /// Returns the base-2 logarithm of a 32-bits signed integer value.
        /// </summary>
        /// <param name="value">The value whose base-2 logarithm to get.</param>
        /// <returns>The base-2 logarithm of <paramref name="value"/>.</returns>
        public static int Log2(int value)
        {
            var log = 0;
            for (var i = 4; i > -1; i--)
            {
                if ((value & B[i]) == 0) continue;
                value >>= S[i];
                log |= S[i];
            }
            return log;
        }

        /// <summary>
        /// Returns a mask for specified number of lower bits of a byte value.
        /// </summary>
        /// <param name="count">The number of lower bits of the byte value to get a mask for.</param>
        /// <returns>A bit mask which selects the lower <paramref name="count"/> bits of a byte value.</returns>
        public static byte LowerMask(int count)
        {
            byte value = 0;
            if (count <= 0) return value;
            count %= 8;
            if (count == 0)
            {
                value = 255;
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    value |= (byte)(1 << i);
                }
            }
            return value;
        }

        /// <summary>
        /// Returns the integer r for which 0 &lt;= r &lt; <paramref name="mod"/> and <paramref name="value"/> - r is a
        /// multiple of <paramref name="mod"/>, when <paramref name="mod"/> is positive, or returns the integer r for
        /// which 0 &gt;= r &gt; <paramref name="mod"/> and <paramref name="value"/> - r is a multiple of
        /// <paramref name="mod"/>, when <paramref name="mod"/> is negative.
        /// </summary>
        /// <param name="value">The value for which getting the modulo.</param>
        /// <param name="mod">The modulo of <paramref name="value"/>.</param>
        /// <returns>The integer r for which 0 &lt;= r &lt; <paramref name="mod"/> and <paramref name="value"/> - r is a
        /// multiple of <paramref name="mod"/>, when <paramref name="mod"/> is positive, or the integer r for which
        /// 0 &gt;= r &gt; <paramref name="mod"/> and <paramref name="value"/> - r is a multiple of
        /// <paramref name="mod"/>, when <paramref name="mod"/> is negative.</returns>
        /// <remarks>See http://stackoverflow.com/questions/4003232/how-to-code-a-modulo-operator-in-c-c-obj-c-that-handles-negative-numbers
        /// </remarks>
        /// <example>
        /// <list type="bullet">
        /// <item><term>Mod(9, 5) = 4</term></item>
        /// <item><term>Mod(-2, 5) = 3</term></item>
        /// <item><term>Mod(2, -5) = -3</term></item>
        /// <item><term>Mod(-6, -5) = -1</term></item>
        /// </list>
        /// </example>
        public static int Mod(int value, int mod)
        {
            return value - mod * (int)Math.Floor((double)value / mod);
        }
    }
}
