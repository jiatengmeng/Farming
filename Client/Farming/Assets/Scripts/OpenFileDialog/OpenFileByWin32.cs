using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class OpenFileByWin32
{
    public static string OpenFile(string filter,string defExt,string initialDir)
    {
        //根据传入的filter和defext来确定打开的文件类型
        FileOpenDialog dialog = new FileOpenDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        dialog.filter = filter;

        dialog.file = initialDir.Equals("") ? new string(new char[256]): initialDir;

        dialog.maxFile = 256;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = initialDir.Equals("") ? UnityEngine.Application.dataPath : null;  //默认路径
        
        dialog.title = "Open File Dialog";

        dialog.defExt = defExt;//显示文件的类型
        //注意一下项目不一定要全选 但是0x00000008项不要缺少
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (DialogShow.GetOpenFileName(dialog))
        {
            return dialog.file;
        }
        return "";
    }

    public void SaveXmlFile()
    {
        XmlHandler.XmlSave();
    }

    public static string BrowseXml(string dir)
    {
        string filter = "(*.xml)|\0*.xml\0\0";
        string defExt = "xml";
        string filePath = OpenFileByWin32.OpenFile(filter, defExt, dir);
        return filePath;
    }

    public static string BrowseExcel(string dir)
    {
        string filter = "(*.xls;*.xlsx)|\0*.xls;*.xlsx\0\0";
        string defExt = "xls;xlsx";
        string filePath = OpenFileByWin32.OpenFile(filter, defExt, dir);
        return filePath;
    }
    public static string Browse(string dir)
    {
        try
        {
            OpenDialogDir ofn2 = new OpenDialogDir();
            ofn2.pszDisplayName = new string(new char[2048]); // 存放目录路径缓冲区  
            ofn2.lpszTitle = "Open File dir"; // 标题  
            ofn2.ulFlags = 0x00000040; // 新的样式,带编辑框  
            IntPtr pidlPtr = DialogShow.SHBrowseForFolder(ofn2);

            char[] charArray = new char[2048];

            for (int i = 0; i < 2048; i++)
            {
                charArray[i] = '\0';
            }

            DialogShow.SHGetPathFromIDList(pidlPtr, charArray);
            string res = new string(charArray);
            res = res.Substring(0, res.IndexOf('\0'));
            return res;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return string.Empty;

    }

    public static void OpenFolder(string path)
    {
        System.Diagnostics.Process.Start("explorer.exe", path);
    }

}
