using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GenericStageInjectionCommon.Shared.Misc
{
    public static class MarshalUtilities
    {
        /// <summary>
        /// Writes an array of structures to unmanaged memory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="structArray">The array of marshalable structures to write to unmanaged memory/</param>
        /// <param name="address">The address to write the structure to.</param>
        public static void StructureArrayToPointer<T>(T[] structArray, IntPtr address) where T : struct
        {
            int structSize = Marshal.SizeOf<T>();

            for (int x = 0; x < structArray.Length; x++)
            {
                Marshal.StructureToPtr(structArray[x], address, true);
                address += structSize;
            }
        }
    }
}
