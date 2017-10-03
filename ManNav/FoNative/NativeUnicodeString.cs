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

namespace ManNav.FoNative {
    public class NativeUnicodeString : NativePointer {
        public NativeUnicodeString(IntPtr src) : base(src, 1) {
            UpdateBoundsFromFieldUshort(2);
            Buffer = new NativePointer(GetFieldPtr(4, IntPtr.Size), MaximumLength * 2);
        }

        public unsafe NativeUnicodeString(ushort length) : base(4 + IntPtr.Size + length * 2) {
            *((IntPtr*) ToPtr(4, IntPtr.Size)) = new IntPtr(ToPtr(4 + IntPtr.Size, IntPtr.Size));
            MaximumLength = Length = length;
            Buffer = new NativePointer(this, (IntPtr) 4 + IntPtr.Size, length * 2);
        }


        public unsafe ushort Length {
            get => *((ushort*) ToPtr(0, 2));
            set => *((ushort*) ToPtr(0, 2)) = value;
        }

        public unsafe ushort MaximumLength {
            get => *((ushort*) ToPtr(2, 2));
            set => *((ushort*) ToPtr(2, 2)) = value;
        }

        public NativePointer Buffer { get; }

        public unsafe char this[int index] {
            get => *((char*) Buffer.ToPtr(2 * index, 2));
            set => *((char*) Buffer.ToPtr(2 * index, 2)) = value;
        }

        public static NativeUnicodeString FromManaged(string str) {
            var ret = new NativeUnicodeString(checked((ushort) str.Length));
            for (var i = 0; i < str.Length; i++)
                ret[i] = str[i];

            return ret;
        }
    }
}