﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using CapsLockSharpPrototype.Helper;
using static CapsLockSharpPrototype.Helper.GlobalKeyboardHook;
using static CapsLockSharpPrototype.Helper.KeyDefRuntime;
using trit = System.Int16;
namespace CapsLockSharpPrototype.Runtime
{
    // 越改越像屎山了，有时间再重构吧
    public class Controller
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void SendKeyEvent(int bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);        
        struct Status
        {
            public trit Replacing;//0:未开始，1:已开始 2:已结束
            public bool PressingCapsLock;
            public bool PressingCapsLockSimulated;
            public bool ReleasingCapsLockSimulated;
        }
        private Status _status = new Status();
        private readonly GlobalKeyboardHook _globalKeyboardHook = new GlobalKeyboardHook();
        public void SetupKeyboardHooks(Action<object,EventArgs> keyCapsLockChangedAct)
        {
            _globalKeyboardHook.KeyboardEvent += (object sender, GlobalKeyboardHookEventArgs e) =>
            {
                if ((VirtualKey)e.KeyboardData.VirtualCode != VirtualKey.CapsLock) {
                    OnKeyExceptCapsLockChanged(e);
                    return;
                }
                OnKeyCapsLockChanged(e);
                keyCapsLockChangedAct?.Invoke(sender, e);
            };
        }
        private void OnKeyCapsLockChanged(GlobalKeyboardHookEventArgs e)
        {
            if ((_status.PressingCapsLockSimulated && e.KeyboardState == KeyboardState.KeyDown) ||
                (e.KeyboardState == KeyboardState.KeyUp && _status.ReleasingCapsLockSimulated)) return;
            if (e.KeyboardState == KeyboardState.KeyDown) _status.PressingCapsLock = true;
            else
            {
                if (_status.Replacing == 2) _status.Replacing = 0;
                else
                {
                    SendKeySimulatedAndSetStatus(ref _status.PressingCapsLockSimulated, true, VirtualKey.CapsLock);
                    SendKeySimulatedAndSetStatus(ref _status.ReleasingCapsLockSimulated, false, VirtualKey.CapsLock);                    
                }
                _status.PressingCapsLock = false;
            }
            e.Handled = true;
        }
        private void OnKeyExceptCapsLockChanged(GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardData.VirtualCode == (int)VirtualKey.LeftShift && _status.Replacing == 1) e.Handled = true;
            #region Capslock+Space 实现 左Ctrl+Space
            if ((VirtualKey)e.KeyboardData.VirtualCode == VirtualKey.Space)
            {
                if (e.KeyboardState == KeyboardState.KeyUp || !_status.PressingCapsLock) return;
                _status.Replacing = 1;
                SendKeySimulatedAndSetStatus(ref _status.ReleasingCapsLockSimulated, false, VirtualKey.CapsLock);
                SendKeyEvent((int)VirtualKey.LeftControl, 0, 0, UIntPtr.Zero);
                SendKeyEvent((int)VirtualKey.Space, 0, 0, UIntPtr.Zero);
                SendKeyEvent((int)VirtualKey.LeftControl, 0, 2, UIntPtr.Zero);
                _status.Replacing = 2;
                e.Handled = true;
                return;
            }
            #endregion
            var keyDef = KeyDefs.FirstOrDefault(x => x.AdditionKey == (VirtualKey)e.KeyboardData.VirtualCode);
            if (keyDef.Equals(default(KeyDef)) || e.KeyboardState == KeyboardState.KeyUp || !_status.PressingCapsLock) return;
            _status.Replacing = 1;
            SendKeySimulatedAndSetStatus(ref _status.ReleasingCapsLockSimulated, false, VirtualKey.CapsLock);
            SendKeyEvent((int)keyDef.AdditionKey, 0, 2, UIntPtr.Zero);
            SendKeyEvent((int)keyDef.ReplacingKey, 0, 0, UIntPtr.Zero);
            _status.Replacing = 2;
            e.Handled = true;
        }
        private void SendKeySimulatedAndSetStatus(ref bool doingMark, bool press, VirtualKey key)
        {
            doingMark = true;
            SendKeyEvent((int)key, 0, (uint)(press ? 0 : 2), UIntPtr.Zero);
            doingMark = false;
        }
    }
}
