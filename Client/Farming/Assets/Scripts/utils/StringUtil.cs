using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtil
{
    /// <summary>
    /// 字符转int
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int ToInt(this string str)
    {
        int temp = 0;
        int.TryParse(str, out temp);
        return temp;
    }
}
