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

    /// <summary>
    /// 检查是否有中文
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool CheckChinese(this string str)
    {
        char[] ch = str.ToCharArray();
        if (str != null)
        {
            for (int i = 0; i < ch.Length; i++)
            {
                if(ch[i] >= 0x4E00 && ch[i] <= 0x9FA5)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static string GetDataType(this string str)
    {
        if (str[0] == 'n') return "1";
        if (str[0] == 's') return "2";
        if (str[0] == 'f') return "3";
        if (str[0] == 'l') return "4";
        return "0";
    }

    public static string GetDataTypeName(this string str)
    {
        switch(str)
        {
            case "1":
                return "int32";
            case "2":
                return "string";
            case "3":
                return "float";
            case "4":
                return "int64";
            default:
                return "None";
        }    
    }
}
