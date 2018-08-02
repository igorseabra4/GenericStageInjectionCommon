using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GenericStageInjectionCommon.Shared;
using Reloaded.Process.Functions.X64Functions;
using Reloaded.Process.Functions.X86Functions;
using Reloaded.Process.Memory;

namespace GenericStageInjection
{
    /// <summary>
    /// Defines the individual delegates that we will use for hooking individual game functions as part of level injection.
    /// </summary>
    public class Hooks
    {
        /*
            NtCreateFile hook taken from Reloaded's File Redirector mod (non-legacy).
        */

        /// <summary>
        /// Creates a new file or directory, or opens an existing file, device, directory, or volume.
        /// (The description here is a partial, lazy copy from MSDN)
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [X64ReloadedFunction(X64CallingConventions.Microsoft)]
        [ReloadedFunction(CallingConventions.Stdcall)]
        public delegate int NtCreateFile(out IntPtr handle, FileAccess access, ref OBJECT_ATTRIBUTES objectAttributes,
            ref IO_STATUS_BLOCK ioStatus, ref long allocSize, uint fileAttributes, FileShare share, uint createDisposition, uint createOptions,
            IntPtr eaBuffer, uint eaLength);

        /// <summary>
        /// A driver sets an IRP's I/O status block to indicate the final status of an I/O request, before calling IoCompleteRequest for the IRP.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct IO_STATUS_BLOCK
        {
            public UInt32 status;
            public IntPtr information;
        }

        /// <summary>
        /// The OBJECT_ATTRIBUTES structure specifies attributes that can be applied to objects or object
        /// handles by routines that create objects and/or return handles to objects.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_ATTRIBUTES : IDisposable
        {
            /// <summary>
            /// Lengthm of this structure.
            /// </summary>
            public int Length;

            /// <summary>
            /// Optional handle to the root object directory for the path name specified by the ObjectName member.
            /// If RootDirectory is NULL, ObjectName must point to a fully qualified object name that includes the full path to the target object.
            /// If RootDirectory is non-NULL, ObjectName specifies an object name relative to the RootDirectory directory.
            /// The RootDirectory handle can refer to a file system directory or an object directory in the object manager namespace.
            /// </summary>
            public IntPtr RootDirectory;

            /// <summary>
            /// Pointer to a Unicode string that contains the name of the object for which a handle is to be opened.
            /// This must either be a fully qualified object name, or a relative path name to the directory specified by the RootDirectory member.
            /// </summary>
            private IntPtr objectName;

            /// <summary>
            /// Bitmask of flags that specify object handle attributes. This member can contain one or more of the flags in the following table (See MSDN)
            /// </summary>
            public uint Attributes;

            /// <summary>
            /// Specifies a security descriptor (SECURITY_DESCRIPTOR) for the object when the object is created.
            /// If this member is NULL, the object will receive default security settings.
            /// </summary>
            public IntPtr SecurityDescriptor;

            /// <summary>
            /// Optional quality of service to be applied to the object when it is created.
            /// Used to indicate the security impersonation level and context tracking mode (dynamic or static).
            /// Currently, the InitializeObjectAttributes macro sets this member to NULL.
            /// </summary>
            public IntPtr SecurityQualityOfService;

            /// <summary>
            /// You ain't gonna need it but it's here anyway.
            /// </summary>
            /// <param name="name">Specifies the full path of the file.</param>
            /// <param name="attrs">Attributes for the file.</param>
            public OBJECT_ATTRIBUTES(string name, uint attrs)
            {
                Length = 0;
                RootDirectory = IntPtr.Zero;
                objectName = IntPtr.Zero;
                Attributes = attrs;
                SecurityDescriptor = IntPtr.Zero;
                SecurityQualityOfService = IntPtr.Zero;

                Length = Marshal.SizeOf(this);
                ObjectName = new UNICODE_STRING(name);
            }

            /// <summary>
            /// Gets or sets the file path of the files loaded in or out.
            /// </summary>
            public UNICODE_STRING ObjectName
            {
                get => (UNICODE_STRING)Marshal.PtrToStructure(objectName, typeof(UNICODE_STRING));

                set
                {
                    // Check if we need to delete old memory.
                    bool fDeleteOld = objectName != IntPtr.Zero;

                    // Allocates the necessary bytes for the string.
                    if (!fDeleteOld)
                        objectName = Marshal.AllocHGlobal(Marshal.SizeOf(value));

                    // Deallocate old string while writing the new one.
                    Marshal.StructureToPtr(value, objectName, fDeleteOld);
                }
            }

            /// <summary>
            /// Disposes of the actual object name (file name) in question.
            /// </summary>
            public void Dispose()
            {
                if (objectName != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(objectName, typeof(UNICODE_STRING));
                    Marshal.FreeHGlobal(objectName);
                    objectName = IntPtr.Zero;
                }
            }
        }


        /// <summary>
        /// Does this really need to be explained to you?
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING : IDisposable
        {
            public ushort Length;
            public ushort MaximumLength;
            private IntPtr buffer;

            public UNICODE_STRING(string s)
            {
                Length = (ushort)(s.Length * 2);
                MaximumLength = (ushort)(Length + 2);
                buffer = Marshal.StringToHGlobalUni(s);
            }

            /// <summary>
            /// Disposes of the current file name assigned to this Unicode String.
            /// </summary>
            public void Dispose()
            {
                Marshal.FreeHGlobal(buffer);
                buffer = IntPtr.Zero;
            }

            /// <summary>
            /// Returns a string with the contents
            /// </summary>
            /// <returns></returns>
            [HandleProcessCorruptedStateExceptions]
            public override string ToString()
            {
                try
                {
                    byte[] uniString = Program.GameProcess.ReadMemory(buffer, Length);
                    return Encoding.Unicode.GetString(uniString);
                }
                catch { return ""; }

            }
        }

        /// <summary>
        /// Defines the Sonic Heroes function used to load a spline for the current Sonic Heroes stage.
        /// </summary>
        /// <param name="splinePointerArray">
        ///     A pointer to a null pointer delimited list of pointers to the Spline structures.
        ///     C/C++: `Spline**`
        ///     
        ///     C#: ref Spline = Spline*
        ///     Note: Spline is a class, thus the actual instance stored in the array is a pointer, thus the parameter is Spline**.
        /// </param>
        /// <returns></returns>
        [ReloadedFunction(CallingConventions.Cdecl)]
        public delegate bool InitPath(ref Spline[] splinePointerArray);
    }
}
