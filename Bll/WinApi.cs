using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Bll
{
    public class WinApi
    {
        public const int WM_NCACTIVATE = 0x86;
        public const int WM_NCPAINT = 0x85;
        public const int WM_COPYDATA = 0x004A;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        public const int MSG_CUSTOM_INFO = 0x901;

        public struct COPYDATASTRUCT
        {
            /// <summary>
            /// 可以是任意值
            /// </summary>
            public IntPtr dwData;
            /// <summary>
            /// 指定lpData内存区域的字节数
            /// </summary>
            public int cbData;
            /// <summary>
            /// 发送给目标窗口所在进程的数据
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }


        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, ref COPYDATASTRUCT lParam);

        //消息发送API
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
           int Msg,            // 消息ID
            int wParam,         // 参数1
            int lParam            // 参数2
        );

        //异步消息发送API
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
           int Msg,            // 消息ID
            int wParam,         // 参数1
            ref COPYDATASTRUCT lParam  // 参数2
        );

        /// <summary>
        /// 这个函数检索处理顶级窗口的类名和窗口名称匹配指定的字符串。这个函数不搜索子窗口。
        /// </summary>
        /// <param name="lpClassName">指向一个以NULL字符结尾的、用来指定类名的字符串或一个可以确定类名字符串的原子。如果这个参数是一个原子，那么它必须是一个在调用此函数前已经通过GlobalAddAtom函数创建好的全局原子。这个原子（一个16bit的值），必须被放置在lpClassName的低位字节中，lpClassName的高位字节置零。如果该参数为null时，将会寻找任何与lpWindowName参数匹配的窗口。</param>
        /// <param name="lpWindowName">指向一个以NULL字符结尾的、用来指定窗口名（即窗口标题）的字符串。如果此参数为NULL，则匹配所有窗口名。</param>
        /// <returns></returns>
        [DllImport("user32 ")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 应用程序可以使用SetParent函数来设置弹出式窗口，层叠窗口或子窗口的父窗口。新的窗口与窗口必须属于同一应用程序。
        /// </summary>
        /// <param name="hWndChild">子窗口句柄。</param>
        /// <param name="hWndNewParent">新的父窗口句柄。如果该参数是NULL，则桌面窗口就成为新的父窗口。在WindowsNT5.0中，如果参数为HWND_MESSAGE,则子窗口成为消息窗口。</param>
        /// <returns></returns>
        [DllImport("user32 ")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 模拟鼠标操作
        /// </summary>
        /// <param name="dwFlags">鼠标操作的标志之一或它们的组合</param>
        /// <param name="dx">根据MOUSEEVENTF_ABSOLUTE标志,指定x,y方向的绝对位置或相对位置</param>
        /// <param name="dy"></param>
        /// <param name="cButtons">没有使用</param>
        /// <param name="dwExtraInfo">没有使用</param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);


        public const int MOUSEEVENTF_MOVE = 0x0001;//移动鼠标
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;//模拟鼠标左键按下
        public const int MOUSEEVENTF_LEFTUP = 0x0004;//模拟鼠标左键抬起
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;//模拟鼠标右键按下
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;//模拟鼠标右键抬起
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;//模拟鼠标中键按下
        public const int MOUSEEVENTF_MIDDLEFUP = 0x0040;//模拟鼠标中键抬起
        public const int MOUSEEVNETF_ABSOLUTE = 0x8000;//标示是否采用绝对坐标

        #region Pcomm

        #region 波特率
        public const int B50 = 0x00;
        public const int B75 = 0x01;
        public const int B110 = 0x02;
        public const int B134 = 0x03;
        public const int B150 = 0x04;
        public const int B300 = 0x05;
        public const int B600 = 0x06;
        public const int B1200 = 0x07;
        public const int B1800 = 0x08;
        public const int B2400 = 0x09;
        public const int B4800 = 0x0A;
        public const int B7200 = 0x0B;
        public const int B9600 = 0x0C;
        public const int B19200 = 0x0D;
        public const int B38400 = 0x0E;
        public const int B57600 = 0x0F;
        public const int B115200 = 0x10;
        public const int B230400 = 0x11;
        public const int B460800 = 0x12;
        public const int B921600 = 0x13;
        #endregion


        #region 停止位
        public const int STOP_1 = 0x00;
        public const int STOP_2 = 0x04;
        #endregion


        #region 数据位
        public const int BIT_5 = 0x00;
        public const int BIT_6 = 0x01;
        public const int BIT_7 = 0x02;
        public const int BIT_8 = 0x03;
        #endregion


        #region 检验位
        public const int p_NONE = 0x00;
        public const int p_ODD = 0x08;
        public const int p_SPC = 0x38;
        public const int p_MRK = 0x28;
        public const int p_EVEN = 0x18;
        #endregion


        /// <summary>
        /// 错误枚举
        /// </summary>
        public enum ErrorCode
        {
            /// <summary>
            /// 正确
            /// </summary>
            SIO_OK = 0,
            /// <summary>
            /// 没有此端口或端口未打开
            /// </summary>
            SIO_BADPORT = -1,
            /// <summary>
            /// 无法控制此板
            /// </summary>
            SIO_OUTCONTROL = -2,
            /// <summary>
            /// 没有数据代读取或没有缓冲区供写
            /// </summary>
            SIO_NODATA = -4,
            /// <summary>
            /// 没有此端口或端口已打开
            /// </summary>
            SIO_OPENFAIL = -5,
            /// <summary>
            /// 因为H/W流量控制而不能设置
            /// </summary>
            SIO_RTS_BY_HW = -6,
            /// <summary>
            /// 无效参数
            /// </summary>
            SIO_BADPARM = -7,
            /// <summary>
            /// 调用win32函数失败请调用GetLastError函数以获取错误代码
            /// </summary>
            SIO_WIN32FAIL = -8,
            /// <summary>
            /// 此版本不支持这个函数
            /// </summary>
            SIO_BOARDNOTSUPPORT = -9,
            /// <summary>
            /// PCOMM函数运行结果失败
            /// </summary>
            SIO_FAIL = -10,
            /// <summary>
            /// 写入已被锁定，用户已放弃写入
            /// </summary>
            SIO_ABORT_WRITE = -11,
            /// <summary>
            /// 已发生写入超时
            /// </summary>
            SIO_WRITETIMEOUT = -12,
        }


        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="port">端口的编号（1 2 3）</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_open(int port);

        /// <summary>
        /// 设置 波特率 数据位 停止位 模式
        /// </summary>
        /// <param name="port">端口的编号</param>
        /// <param name="baudrate">波特率</param>
        /// <param name="mode">数据位 停止位 模式</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_ioctl(int port, int baudrate, int mode);
        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <param name="port">端口的编号</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_close(int port);
        /// <summary>
        /// 读取缓冲区的数据
        /// </summary>
        /// <param name="port">端口的编号</param>
        /// <param name="data">数据集</param>
        /// <param name="len">数据集长度</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_read(int port, byte[] data, int len);
        /// <summary>
        /// 清除缓冲区的数据
        /// </summary>
        /// <param name="port">端口的编号</param>
        /// <param name="index">2 清除读写缓冲区的数据</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_flush(int port, int index);
        /// <summary>
        /// 向缓冲区写入数据
        /// </summary>
        /// <param name="port">端口的编号</param>
        /// <param name="by">数据集</param>
        /// <param name="len">数据集长度</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_write(int port, byte[] by, int len);
        /// <summary>
        /// 获取缓冲区的数据长度
        /// </summary>
        /// <param name="port">端口的编号</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_iqueue(int port);

        /// <summary>
        /// 设置串口写操作的超时
        /// </summary>
        /// <param name="port">端口的编号</param>
        /// <param name="totalTimeouts">超时时间</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_SetWriteTimeouts(int port, int totalTimeouts);

        /// <summary>
        /// 当端口接收到一个字体的数据时的回调方法
        /// </summary>
        /// <param name="port">端口的编号</param>
        /// <param name="function">回调方法</param>
        /// <param name="count">1</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_cnt_irq(int port, DataReceivedEventHandler function, int count);

        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="port"></param>
        public delegate void DataReceivedEventHandler(int port);

        #endregion Pcomm
    }
}
