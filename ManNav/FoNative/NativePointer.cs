/***************************************************************
*  Copyright 2017 David Garica
*
*   Licensed under the Apache License, Version 2.0 (the "License");
*   you may not use this file except in compliance with the License.
*   You may obtain a copy of the License at
*
*       http://www.apache.org/licenses/LICENSE-2.0
*      
*   Unless required by applicable law or agreed to in writing, software
*   distributed under the License is distributed on an "AS IS" BASIS,
*   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*   See the License for the specific language governing permissions and
*   limitations under the License.
*
*   See Also:
* https://raw.githubusercontent.com/dx-prog/ManNavCompiler/master/LICENSE
* *************************************************************/
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace ManNav.FoNative {
    /// <summary>
    /// Place holder tool for manpulating data stored in a native buffer
    /// </summary>
    public class NativePointer : CriticalFinalizerObject, IDisposable {
        private readonly object _parent;
        private IntPtr _edge;
        private IntPtr _ptr;


        public NativePointer(IntPtr extrnal, int size) {
            StructureSize = size;
            Count = 1;
            _parent = _ptr = extrnal;
            _edge = extrnal + size;
        }

        public unsafe NativePointer(NativePointer parent, IntPtr offset, int size) {
            _parent = parent;
            _ptr = new IntPtr(parent.ToPtr(offset.ToInt32(), size));
            StructureSize = size;
            Count = 1;
            _edge = parent._edge;
        }

        public unsafe NativePointer(NativePointer parent, int offset, int size) {
            _parent = parent;
            _ptr = new IntPtr(parent.ToPtr(offset, size));
            StructureSize = size;
            Count = 1;
            _edge = parent._edge;
        }


        public NativePointer(int structureSize, int count = 1) {
            if (count < 1)
                throw new InvalidOperationException();

            StructureSize = structureSize;
            Count = count;
            _ptr = Marshal.AllocHGlobal((IntPtr) (StructureSize * Count));
            _edge = _ptr + StructureSize * Count;
        }

        public int StructureSize { get; protected set; }
        public int Count { get; }

        protected unsafe byte Byte {
            get => *((byte*) ToPtr(0, 1));
            set => *((byte*) ToPtr(0, 1)) = value;
        }

        protected unsafe char Char {
            get => *((char*) ToPtr(0, 2));
            set => *((char*) ToPtr(0, 2)) = value;
        }

        protected unsafe short Short {
            get => *((short*) ToPtr(0, 2));
            set => *((short*) ToPtr(0, 2)) = value;
        }

        protected unsafe long Long {
            get => *((long*) ToPtr(0, 4));
            set => *((long*) ToPtr(0, 4)) = value;
        }

        protected unsafe float Float {
            get => *((float*) ToPtr(0, 4));
            set => *((float*) ToPtr(0, 4)) = value;
        }

        protected unsafe double Double {
            get => *((double*) ToPtr(0, 8));
            set => *((double*) ToPtr(0, 8)) = value;
        }

        protected unsafe Guid Guid {
            get => *((Guid*) ToPtr(0, 16));
            set => *((Guid*) ToPtr(0, 16)) = value;
        }

        public void Dispose() {
            if (null != _parent)
                return;

            var ptr = Interlocked.CompareExchange(ref _ptr, IntPtr.Zero, _ptr);
            if (IntPtr.Zero != ptr)
                Marshal.FreeHGlobal(ptr);
        }

        public NativePointer Get(int position, int fieldSize) {
            var fieldLocation = GetFieldPtr(position, fieldSize);
            return new NativePointer(this, fieldLocation, fieldSize);
        }

        public unsafe IntPtr GetFieldPtr(int position, int fieldSize) {
            return new IntPtr(ToPtr(position, fieldSize));
        }

        protected internal unsafe void* ToPtr(int offSet, int sizePromise) {
            if (Volatile.Read(ref _ptr) == IntPtr.Zero)
                throw new AccessViolationException();

            var edge = _ptr + offSet + sizePromise;
            if (edge.ToInt64() > _edge.ToInt64() ||
                edge.ToInt64() < _ptr.ToInt64())
                throw new AccessViolationException();

            return ToPtrNoBoundsCheck(offSet);
        }

        protected unsafe void* ToPtrNoBoundsCheck(int offSet) {
            return (void*) (_ptr + offSet);
        }

        protected unsafe void UpdateBoundsFromFieldByte(int i) {
            StructureSize = *((byte*) ToPtrNoBoundsCheck(i));
            _edge = _ptr + StructureSize;
        }

        protected unsafe void UpdateBoundsFromFieldUshort(int i) {
            StructureSize = *((ushort*) ToPtrNoBoundsCheck(i));
            _edge = _ptr + StructureSize;
        }
    }
}