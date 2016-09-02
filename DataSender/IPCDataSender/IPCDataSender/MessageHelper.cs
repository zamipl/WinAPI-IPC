﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataToSend;

namespace IPCDataSender
{
    public class MessageHelper
    {
        // The COPYDATASTRUCT describes the data that is passed.
        // The message is routed via the receiving process's window handle.
        // The first field, dwData, may contain anything the sender wishes; it is the equivalent of System.Object sender in an EventHandler.
        // The count of bytes is given in cbData.
        // And the data itself is pointed to by lpData.
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, IntPtr lParam);

        public const int WM_USER = 0x400;
        public const int WM_COPYDATA = 0x4A;

        /// <summary>
        /// Allocate a pointer to an arbitrary structure on the global heap.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IntPtr IntPtrAlloc<T>(T param)
        {
            IntPtr retval = Marshal.AllocHGlobal(Marshal.SizeOf(param));
            Marshal.StructureToPtr(param, retval, false);
            return retval;
        }

        /// <summary>
        /// Free a pointer to an arbitrary structure from the global heap.
        /// </summary>
        /// <param name="preAllocated"></param>
        public static void IntPtrFree(ref IntPtr preAllocated)
        {
            if (IntPtr.Zero == preAllocated)
                throw (new NullReferenceException("Go Home"));
            Marshal.FreeHGlobal(preAllocated);
            preAllocated = IntPtr.Zero;
        }

        public void SendData(int hWnd, string txtToSend)
        {
            MessagePacket singlePacket = new MessagePacket();
            singlePacket.MessageTitle = "Single message title : " + txtToSend;
            singlePacket.MessageDescription = "Single message description - can be long.";
            singlePacket.ErrorId = 0;
            singlePacket.ModuleId = 1;
            singlePacket.ValueAfter = 0.1;
            singlePacket.ValueBefore = 0.2;

            IntPtr buffer = IntPtrAlloc(singlePacket);
            COPYDATASTRUCT copyData = new COPYDATASTRUCT();
            copyData.dwData = IntPtr.Zero;
            copyData.lpData = buffer;
            copyData.cbData = Marshal.SizeOf(singlePacket);
            IntPtr copyDataBuff = IntPtrAlloc(copyData);
            SendMessage(hWnd, WM_COPYDATA, 0, copyDataBuff);
            IntPtrFree(ref copyDataBuff);
            IntPtrFree(ref buffer);
        }
    }
}
