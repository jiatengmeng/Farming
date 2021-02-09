using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;

public static class ChineseStringUtility
{

    /// <summary>
    /// 讲字符转换为繁体中文
    /// </summary>
    [DllImport("kernel32.dll", EntryPoint = "LCMapStringA")]
    public static extern int LCMapString(int Locale, int dwMapFlags, byte[] lpSrcStr, int cchSrc, byte[] lpDestStr, int cchDest);
    const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
    const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

    //转化方法
    public static string ToTraditional(string source)
    {
        byte[] srcByte2 = Encoding.Default.GetBytes(source);
        byte[] desByte2 = new byte[srcByte2.Length];
        LCMapString(2052, LCMAP_SIMPLIFIED_CHINESE, srcByte2, -1, desByte2, srcByte2.Length);
        string des2 = Encoding.Default.GetString(desByte2);
        return des2;
    }
}