using Unity.Burst;

namespace Common
{
    // TODO: use Morton for ceil mapping during the next optimisation of GridSystem 
    [BurstCompile]
    public static class Morton
    {
        private const uint Bias = 0x80000000;

        /// <summary>
        /// Encodes two coordinates (x, y) into a single Morton number (Z-order curve).
        /// Supports negative values.
        /// </summary>
        [BurstCompile]
        public static ulong Encode(int x, int y)
        {
            // Shift int range to unsigned (from 0 to uint.MaxValue)
            ulong ux = (ulong)(x + Bias);
            ulong uy = (ulong)(y + Bias);

            return (InterleaveBits(ux) | (InterleaveBits(uy) << 1));
        }

        /// <summary>
        /// Decodes a Morton number (Z-order curve) back into two coordinates (x, y).
        /// </summary>
        [BurstCompile]
        public static void Decode(ulong morton, out int x, out int y)
        {
            x = (int)(DeinterleaveBits(morton) - Bias);
            y = (int)(DeinterleaveBits(morton >> 1) - Bias);
        }

        [BurstCompile]
        private static ulong InterleaveBits(ulong v)
        {
            v = (v | (v << 16)) & 0x0000FFFF0000FFFFUL;
            v = (v | (v << 8)) & 0x00FF00FF00FF00FFUL;
            v = (v | (v << 4)) & 0x0F0F0F0F0F0F0F0FUL;
            v = (v | (v << 2)) & 0x3333333333333333UL;
            v = (v | (v << 1)) & 0x5555555555555555UL;

            return v;
        }

        [BurstCompile]
        private static uint DeinterleaveBits(ulong v)
        {
            v &= 0x5555555555555555UL;
            v = (v | (v >> 1)) & 0x3333333333333333UL;
            v = (v | (v >> 2)) & 0x0F0F0F0F0F0F0F0FUL;
            v = (v | (v >> 4)) & 0x00FF00FF00FF00FFUL;
            v = (v | (v >> 8)) & 0x0000FFFF0000FFFFUL;
            v = (v | (v >> 16)) & 0x00000000FFFFFFFFUL;

            return (uint)v;
        }
    }
}