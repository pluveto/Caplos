using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CapsLockSharpPrototype.Helper
{
    static class MouseConfig
    {
        /// <summary>  
        /// 鼠标速度  
        /// </summary>  
        public const UInt32 SPI_SETMOUSESPEED = 0x0071;
        /// <summary>  
        /// 滚轮速度  
        /// </summary>  
        public const UInt32 SPI_SETWHEELSCROLLLINES = 0x0069;

        //最大值为20，最小值为1  
        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(
        UInt32 uiAction,
        UInt32 uiParam,
        UInt32 pvParam,
        UInt32 fWinIni);

        /// <summary>  
        /// 设置鼠标速度  
        /// </summary>  
        /// <param name="speed">鼠标速度，最大值20，最小值1</param>  
        public static void SetMouseSpeed(uint speed)
        {
            uint mouseSpeed = speed;
            if (mouseSpeed < 1)
            {
                mouseSpeed = 1;
            }
            else if (mouseSpeed > 20)
            {
                mouseSpeed = 20;
            }
            SystemParametersInfo(SPI_SETMOUSESPEED, 0, mouseSpeed, 0);
        }

        /// <summary>  
        /// 获取当前鼠标速度  
        /// </summary>  
        /// <returns>鼠标速度</returns>  
        public static int GetMouseSpeed()
        {
            return System.Windows.Forms.SystemInformation.MouseSpeed;
        }

        /// <summary>  
        /// 设置鼠标滚轮滚动行数  
        /// </summary>  
        /// <param name="speed">滚轮每次滚动行数，最大值20，最小值1</param>  
        public static void SetMouseWheel(uint speed)
        {
            uint wheelSpeed = speed;
            if (wheelSpeed < 1)
            {
                wheelSpeed = 1;
            }
            else if (wheelSpeed > 20)
            {
                wheelSpeed = 20;
            }
            SystemParametersInfo(SPI_SETWHEELSCROLLLINES, wheelSpeed, 0, 0);
        }
        /// <summary>  
        /// 获取鼠标滚轮滚动行数  
        /// </summary>  
        /// <returns>滚轮行数</returns>  
        public static int GetMouseWheel()
        {
            return System.Windows.Forms.SystemInformation.MouseWheelScrollLines;
        }
    }
}
