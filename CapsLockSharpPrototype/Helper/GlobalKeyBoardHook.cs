using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CapsLockSharpPrototype.Helper
{
    /*
     下列代码最初来自：
        https://stackoverflow.com/questions/604410/global-keyboard-capture-in-c-sharp-application
     作者：Siarhei Kuchuk
     经过了一些修改
    */
    public class GlobalKeyboardHookEventArgs : HandledEventArgs
    {
        public GlobalKeyboardHook.KeyboardState KeyboardState { get; private set; }
        public GlobalKeyboardHook.LowLevelKeyboardInputEvent KeyboardData { get; private set; }

        public GlobalKeyboardHookEventArgs(
            GlobalKeyboardHook.LowLevelKeyboardInputEvent keyboardData,
            GlobalKeyboardHook.KeyboardState keyboardState)
        {
            KeyboardData = keyboardData;
            KeyboardState = keyboardState;
        }
    }

    //Based on https://gist.github.com/Stasonix
    public class GlobalKeyboardHook : IDisposable
    {
        // 参考的代码写的是 KeyboardPressed，其实不准确，因为还包含了 Release 的动作，因此稍有修改。
        public event EventHandler<GlobalKeyboardHookEventArgs> KeyboardEvent;

        public GlobalKeyboardHook()
        {
            _windowsHookHandle = IntPtr.Zero;
            _user32LibraryHandle = IntPtr.Zero;
            _hookProc = LowLevelKeyboardProc; // _hookProc 私有字段保存 LowLevelKeyboardProc，不然会被 GC 干掉


            _user32LibraryHandle = LoadLibrary("User32");
            if (_user32LibraryHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to load library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }



            _windowsHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0);
            if (_windowsHookHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to adjust keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _windowsHookHandle == IntPtr.Zero)
            {
                // because we can unhook only in the same thread, not in garbage collector thread
                if (!UnhookWindowsHookEx(_windowsHookHandle))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode, $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                }
                _windowsHookHandle = IntPtr.Zero;
                // ReSharper disable once DelegateSubtraction
                _hookProc -= LowLevelKeyboardProc;
            }
            // 避免重复释放，上同
            if (_user32LibraryHandle == IntPtr.Zero) return;
            if (!FreeLibrary(_user32LibraryHandle))
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }
            _user32LibraryHandle = IntPtr.Zero;
        }

        ~GlobalKeyboardHook()
        {
            // 利用析构函数自动释放。虽然实现了 IDisposable 接口，但是C#并不会帮你释放。
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private IntPtr _windowsHookHandle;
        private IntPtr _user32LibraryHandle;
        private HookProc _hookProc;

        delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
        /// You would install a hook procedure to monitor the system for certain types of events. These events are
        /// associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">hook type</param>
        /// <param name="lpfn">hook procedure</param>
        /// <param name="hMod">handle to application instance</param>
        /// <param name="dwThreadId">thread identifier</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport("USER32", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hHook">handle to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("USER32", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hHook);

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        /// A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hHook">handle to current hook</param>
        /// <param name="code">hook code passed to hook procedure</param>
        /// <param name="wParam">value passed to hook procedure</param>
        /// <param name="lParam">value passed to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("USER32", SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct LowLevelKeyboardInputEvent
        {
            /// <summary>
            /// A virtual-key code. The code must be a value in the range 1 to 254.
            /// </summary>
            public int VirtualCode;

            /// <summary>
            /// A hardware scan code for the key. 
            /// </summary>
            public int HardwareScanCode;

            /// <summary>
            /// The extended-key flag, event-injected Flags, context code, and transition-state flag. This member is specified as follows. An application can use the following values to test the keystroke Flags. Testing LLKHF_INJECTED (bit 4) will tell you whether the event was injected. If it was, then testing LLKHF_LOWER_IL_INJECTED (bit 1) will tell you whether or not the event was injected from a process running at lower integrity level.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The time stamp stamp for this message, equivalent to what GetMessageTime would return for this message.
            /// </summary>
            public int TimeStamp;

            /// <summary>
            /// Additional information associated with the message. 
            /// </summary>
            public IntPtr AdditionalInformation;
        }

        // ReSharper disable once InconsistentNaming
        public const int WH_KEYBOARD_LL = 13;
        //const int HC_ACTION = 0;

        public enum KeyboardState
        {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            SysKeyDown = 0x0104,
            SysKeyUp = 0x0105
        }
        /// <summary>
        /// Enumeration for virtual keys.
        /// </summary>

        public const int VkSnapshot = 0x2c;

        // 详见：https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        // 或者：https://docs.microsoft.com/en-us/uwp/api/windows.system.virtualkey
        //const int VkLwin = 0x5b;
        //const int VkRwin = 0x5c;
        //const int VkTab = 0x09;
        //const int VkEscape = 0x18;
        //const int VkControl = 0x11;
        const int KfAltdown = 0x2000;
        public const int LlkhfAltdown = (KfAltdown >> 8);

        // 该回调函数负责处理键盘事件
        // 参考：http://tech.sina.com.cn/s/2006-07-19/09091044360.shtml
        public IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool fEatKeyStroke = false;

            var wparamTyped = wParam.ToInt32();
            if (Enum.IsDefined(typeof(KeyboardState), wparamTyped))
            {
                object o = Marshal.PtrToStructure(lParam, typeof(LowLevelKeyboardInputEvent));
                LowLevelKeyboardInputEvent p = (LowLevelKeyboardInputEvent)o;

                var eventArguments = new GlobalKeyboardHookEventArgs(p, (KeyboardState)wparamTyped);
                // 不要在事件处理放入耗时长的操作，会导致阻断失效。
                var handler = KeyboardEvent;
                handler?.Invoke(this, eventArguments);
                fEatKeyStroke = eventArguments.Handled;
            }
            // 如果 Handled 为真，那么会返回一个哑值 1。
            return fEatKeyStroke ? (IntPtr)1 : CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            //return (IntPtr)1;
        }
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum VirtualKey
        {
            LeftButton = 0x01,
            RightButton = 0x02,
            Cancel = 0x03,
            MiddleButton = 0x04,
            ExtraButton1 = 0x05,
            ExtraButton2 = 0x06,
            Back = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Return = 0x0D,
            Shift = 0x10,
            Control = 0x11,

            Menu = 0x12,

            Pause = 0x13,

            CapsLock = 0x14,

            Kana = 0x15,

            Hangeul = 0x15,

            Hangul = 0x15,

            Junja = 0x17,

            Final = 0x18,

            Hanja = 0x19,

            Kanji = 0x19,

            Escape = 0x1B,

            Convert = 0x1C,

            NonConvert = 0x1D,

            Accept = 0x1E,

            ModeChange = 0x1F,

            Space = 0x20,

            Prior = 0x21,

            Next = 0x22,

            End = 0x23,

            Home = 0x24,

            Left = 0x25,

            Up = 0x26,

            Right = 0x27,

            Down = 0x28,

            Select = 0x29,

            Print = 0x2A,

            Execute = 0x2B,

            Snapshot = 0x2C,

            Insert = 0x2D,

            Delete = 0x2E,

            Help = 0x2F,

            N0 = 0x30,

            N1 = 0x31,

            N2 = 0x32,

            N3 = 0x33,

            N4 = 0x34,

            N5 = 0x35,

            N6 = 0x36,

            N7 = 0x37,

            N8 = 0x38,

            N9 = 0x39,

            A = 0x41,

            B = 0x42,

            C = 0x43,

            D = 0x44,

            E = 0x45,

            F = 0x46,

            G = 0x47,

            H = 0x48,

            I = 0x49,

            J = 0x4A,

            K = 0x4B,

            L = 0x4C,

            M = 0x4D,

            N = 0x4E,

            O = 0x4F,

            P = 0x50,

            Q = 0x51,

            R = 0x52,

            S = 0x53,

            T = 0x54,

            U = 0x55,

            V = 0x56,

            W = 0x57,

            X = 0x58,

            Y = 0x59,

            Z = 0x5A,

            LeftWindows = 0x5B,

            RightWindows = 0x5C,

            Application = 0x5D,

            Sleep = 0x5F,

            Numpad0 = 0x60,

            Numpad1 = 0x61,

            Numpad2 = 0x62,

            Numpad3 = 0x63,

            Numpad4 = 0x64,

            Numpad5 = 0x65,

            Numpad6 = 0x66,

            Numpad7 = 0x67,

            Numpad8 = 0x68,

            Numpad9 = 0x69,

            Multiply = 0x6A,

            Add = 0x6B,

            Separator = 0x6C,

            Subtract = 0x6D,

            Decimal = 0x6E,

            Divide = 0x6F,

            F1 = 0x70,

            F2 = 0x71,

            F3 = 0x72,

            F4 = 0x73,

            F5 = 0x74,

            F6 = 0x75,

            F7 = 0x76,

            F8 = 0x77,

            F9 = 0x78,

            F10 = 0x79,

            F11 = 0x7A,

            F12 = 0x7B,

            F13 = 0x7C,

            F14 = 0x7D,

            F15 = 0x7E,

            F16 = 0x7F,

            F17 = 0x80,

            F18 = 0x81,

            F19 = 0x82,

            F20 = 0x83,

            F21 = 0x84,

            F22 = 0x85,

            F23 = 0x86,

            F24 = 0x87,

            NumLock = 0x90,

            ScrollLock = 0x91,

            NEC_Equal = 0x92,

            Fujitsu_Jisho = 0x92,

            Fujitsu_Masshou = 0x93,

            Fujitsu_Touroku = 0x94,

            Fujitsu_Loya = 0x95,

            Fujitsu_Roya = 0x96,

            LeftShift = 0xA0,

            RightShift = 0xA1,

            LeftControl = 0xA2,

            RightControl = 0xA3,

            LeftMenu = 0xA4,

            RightMenu = 0xA5,

            BrowserBack = 0xA6,

            BrowserForward = 0xA7,

            BrowserRefresh = 0xA8,

            BrowserStop = 0xA9,

            BrowserSearch = 0xAA,

            BrowserFavorites = 0xAB,

            BrowserHome = 0xAC,

            VolumeMute = 0xAD,

            VolumeDown = 0xAE,

            VolumeUp = 0xAF,

            MediaNextTrack = 0xB0,

            MediaPrevTrack = 0xB1,

            MediaStop = 0xB2,

            MediaPlayPause = 0xB3,

            LaunchMail = 0xB4,

            LaunchMediaSelect = 0xB5,

            LaunchApplication1 = 0xB6,

            LaunchApplication2 = 0xB7,

            OEM1 = 0xBA,

            OEMPlus = 0xBB,

            OEMComma = 0xBC,

            OEMMinus = 0xBD,

            OEMPeriod = 0xBE,

            OEM2 = 0xBF,

            OEM3 = 0xC0,

            OEM4 = 0xDB,

            OEM5 = 0xDC,

            OEM6 = 0xDD,

            OEM7 = 0xDE,

            OEM8 = 0xDF,

            OEMAX = 0xE1,

            OEM102 = 0xE2,

            ICOHelp = 0xE3,

            ICO00 = 0xE4,

            ProcessKey = 0xE5,

            ICOClear = 0xE6,

            Packet = 0xE7,

            OEMReset = 0xE9,

            OEMJump = 0xEA,

            OEMPA1 = 0xEB,

            OEMPA2 = 0xEC,

            OEMPA3 = 0xED,

            OEMWSCtrl = 0xEE,

            OEMCUSel = 0xEF,

            OEMATTN = 0xF0,

            OEMFinish = 0xF1,

            OEMCopy = 0xF2,

            OEMAuto = 0xF3,

            OEMENLW = 0xF4,

            OEMBackTab = 0xF5,

            ATTN = 0xF6,

            CRSel = 0xF7,

            EXSel = 0xF8,

            EREOF = 0xF9,

            Play = 0xFA,

            Zoom = 0xFB,

            Noname = 0xFC,

            PA1 = 0xFD,

            OEMClear = 0xFE
        }
    }
}
