using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
//using UnityEngine.Windows;
using System.IO;
using System.Text;
using System;
using NPOI.XSSF.Streaming.Values;

public class XmlHandler
{
    private static XmlDocument Xdoc = null;
    public static XmlDocument _Xdoc = null;
    private static string TempXmlFilePath = UnityEngine.Application.streamingAssetsPath + "/" + "Temp.xml";//临时文件保存位置
    public static string RealXmlFilePath = "";

    //public static Dictionary<string, string> language = new Dictionary<string, string>() { { "Korean","kr" } };
    //读取Xml文件，保存到对应的对象中
    public static void XmlReader0(string XmlFilePath)
    {
        Xdoc = new XmlDocument();
        _Xdoc = new XmlDocument();
        if (XmlFilePath.Equals(""))
        {
            Debug.LogError("Empty file！");
            return;
        }
        RealXmlFilePath = XmlFilePath;
        Xdoc.Load(XmlFilePath);
        _Xdoc = Xdoc;
        XmlCreate();
    }
    public static XmlDocument XmlReader2(string XmlFilePath)
    {
        XmlDocument xdoc = new XmlDocument();
        if (XmlFilePath.Equals(""))
        {
            Debug.LogError("Empty file！");
            return null;
        }
        xdoc.Load(XmlFilePath);
        return xdoc;
    }
    public static XmlDocument XmlReader3(string XmlFilePath)
    {
        XmlDocument xdoc = new XmlDocument();
        if (XmlFilePath.Equals(""))
        {
            Debug.LogError("Empty file！");
            return null;
        }
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.CheckCharacters = true;
        XmlReader xr = XmlReader.Create(XmlFilePath, settings);
        xdoc.Load(xr);
        return xdoc;
    }
    //修改Xml（保存到临时文件）
    public static void XmlUpdate(XmlData xmlData)
    {
        XmlNode root = _Xdoc.SelectSingleNode("root");
        XmlElement xel = (XmlElement)root.ChildNodes[xmlData.NodeID];
        xel.SetAttribute(xmlData.AttributeKey, xmlData.AttributeValue);
        _Xdoc.Save(TempXmlFilePath);//保存xml到临时路径位置
    }
    //xml创建
    public static void XmlCreate()
    {
        XmlDocument xml = new XmlDocument();
        XmlDeclaration xmldecl = xml.CreateXmlDeclaration("1.0", "UTF-8", "yes");//设置xml文件编码格式为UTF-8
        XmlElement root = xml.CreateElement("root");//创建根节点
        xml.AppendChild(root);
        xml.Save(TempXmlFilePath);//保存xml到路径位置
        Debug.Log("创建XML成功！");
    }
    public static void XmlInsert(string keyValue)
    {
        XmlNode root = _Xdoc.SelectSingleNode("root");//获取根节点
        XmlElement newTans = _Xdoc.CreateElement("trans");//创建新的子节点

        foreach (XmlAttribute art in root.ChildNodes[0].Attributes)
        {
            if (art.Name == "key")
            {
                newTans.SetAttribute("key", keyValue);
            }
            else
            {
                newTans.SetAttribute(art.Name, "");
            }
        }
        root.AppendChild(newTans);//添加到root
        _Xdoc.AppendChild(root);
        _Xdoc.Save(TempXmlFilePath);//保存xml到临时路径位置
        Debug.Log("插入Xml成功！");
    }

    public static int XmlKeySearch(XmlNode xmlNode, string key)
    {
        Debug.Log(key);
        if (null == xmlNode) return -1;
        int start = 0;
        int end = xmlNode.ChildNodes.Count-1;
        while (start<=end)
        {
            int mid = start + ((end-start)/2);
            if (string.CompareOrdinal(xmlNode.ChildNodes[mid].Attributes[0].Value,key)==0)
            {
                return mid;
            }
            else if(string.CompareOrdinal(xmlNode.ChildNodes[mid].Attributes[0].Value, key) > 0)
            {
                end = mid-1;
            }
            else if(string.CompareOrdinal(xmlNode.ChildNodes[mid].Attributes[0].Value, key) < 0)
            {
                start = mid+1;
            }
        }
        return -1;
    }


    public static int XmlKeyInsertIndexSearch(XmlNode xmlNode, string key)
    {
        if (null == xmlNode) return -1;
        int start = 0;
        int end = xmlNode.ChildNodes.Count - 1;
        if (string.CompareOrdinal(xmlNode.ChildNodes[start].Attributes[0].Value, key) > 0) return 0;
        if (string.CompareOrdinal(xmlNode.ChildNodes[end].Attributes[0].Value, key) < 0) return end;
        while (start <= end)
        {
            int mid = start + ((end - start) / 2);
            if (string.CompareOrdinal(xmlNode.ChildNodes[mid].Attributes[0].Value, key) > 0)
            {
                if (string.CompareOrdinal(xmlNode.ChildNodes[mid - 1].Attributes[0].Value, key) < 0)
                {
                    return mid - 1;
                }
                else
                {
                    end = mid - 1;
                }
            }
            else if (string.CompareOrdinal(xmlNode.ChildNodes[mid].Attributes[0].Value, key) < 0)
            {
                if (string.CompareOrdinal(xmlNode.ChildNodes[mid + 1].Attributes[0].Value, key) > 0)
                {
                    return mid;
                }
                else
                {
                    start = mid + 1;
                }
            }
        }
        return end;
    }

    //保存修改
    public static void XmlSave()
    {
        Xdoc = _Xdoc;
        Xdoc.Save(RealXmlFilePath);
    }

    public static void XmlSave(XmlDocument xml,string path)
    {
        //FileStream stream = new FileStream(path,FileMode.OpenOrCreate, FileAccess.Write);
        //xml.PreserveWhitespace = true;
        //xml.Save(stream);
        //stream.Close();
        //xml.Save(path);
        //StreamWriter streamWriter = System.IO.File.CreateText(path);

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "    ";
        settings.CheckCharacters = true;
        settings.DoNotEscapeUriAttributes = true;
        settings.NewLineChars = Environment.NewLine;
        settings.ConformanceLevel = ConformanceLevel.Auto;
        XmlWriter xw = XmlWriter.Create(path, settings);
        //xw.WriteCharEntity('\'');
        //XmlCheck(ref xml);
        xml.Save(xw);
        xw.Close();
    }
    public static void XmlCheck(ref XmlDocument xml)
    {
        XmlNode root = xml.SelectSingleNode("root");
        for (int i = 0; i < root.ChildNodes.Count; i++)
        {
            Debug.Log(root.ChildNodes[i].Attributes[0].Value);
            for (int j = 0; j < root.ChildNodes[i].Attributes.Count; j++)
            {
                
                root.ChildNodes[i].Attributes[j].Value = System.Security.SecurityElement.Escape(root.ChildNodes[i].Attributes[j].Value);
            }
            Debug.Log(root.ChildNodes[i].Attributes[0].Value);
        }
    }

    public static Dictionary<string,Dictionary<string, string>> XmlReadByText(string path)
    {
        Dictionary<string, Dictionary<string, string>> xmldata = new Dictionary<string, Dictionary<string, string>>();
        string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
        string errorpath = FilePathMgr.errorLogFolder + "\\" + data + "error.txt";
        string[] info = File.ReadAllLines(path);
        List<string> errorinfo = new List<string>();
        for (int i = 2; i < info.Length - 1; i++)
        {
            Dictionary<string, string> artdic = TextDeserialization(info[i]);
            if (artdic.ContainsKey("key"))
            {
                string strkey = XmlString(artdic["key"]);
                if(xmldata.ContainsKey(strkey)|| xmldata.ContainsKey(artdic["key"])) errorinfo.Add(info[i]);
                else xmldata.Add(artdic["key"], artdic);
            }
            else
            {
                errorinfo.Add(info[i]);
            }
        }
        if(errorinfo.Count>0)
        {
            File.WriteAllLines(errorpath, errorinfo);
        }
        return xmldata;
    }

    public static SortedDictionary<string, Dictionary<string, string>> XmlReadByTextToSort(string path,IComparer<string> comparer)
    {
        SortedDictionary<string, Dictionary<string, string>> xmldata = new SortedDictionary<string, Dictionary<string, string>>(comparer);
        string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
        string errorpath = FilePathMgr.errorLogFolder + "\\" + data + "error.txt";
        string[] info = File.ReadAllLines(path);
        List<string> errorinfo = new List<string>();
        for (int i = 2; i < info.Length - 1; i++)
        {
            Dictionary<string, string> artdic = TextDeserialization(info[i]);
            if (artdic.ContainsKey("key"))
            {
                //string strkey = XmlString(artdic["key"]);
                if (xmldata.ContainsKey(artdic["key"])) errorinfo.Add(info[i]);
                else xmldata.Add(artdic["key"], artdic);
            }
            else
            {
                errorinfo.Add(info[i]);
            }
        }
        if (errorinfo.Count > 0)
        {
            File.WriteAllLines(errorpath, errorinfo);
        }
        Debug.Log("XmlReadByTextToSort Over 读取结束");
        return xmldata;
    }

    public static Dictionary<string, string> TextDeserialization(string text)
    {

        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        string[] vs = text.Split(new char[] { '\"','\'' });
        for (int i = 0; i < vs.Length; i++)
        {
            //Debug.Log(vs[i]);
            int indexOfequl = vs[i].IndexOf('=');
            if (indexOfequl > 0)
            {
                try
                {
                    int indexOfSpace = vs[i].LastIndexOf(' ');
                    string key = vs[i].Substring(indexOfSpace + 1, indexOfequl - indexOfSpace - 1);
                    string value = vs[i + 1];
                    i++;
                    keyValuePairs.Add(key, value);
                }
                catch
                {
                    Debug.Log("内容有单引号,请将内容的单引号修改为&apos;");
                    return new Dictionary<string, string>();
                }
            }
        }
        return keyValuePairs;
    }

    public static void TextSerialization(Dictionary<string, Dictionary<string, string>> xmldata,string path)
    {
        List<string> data = new List<string>();
        string head = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?>";
        string root1 = "<root>";
        data.Add(head);
        data.Add(root1);
        foreach(string key in xmldata.Keys)
        {
            string str = "    <trans ";
            foreach(string art in xmldata[key].Keys)
            {
                if ((xmldata[key][art].IndexOf("&quot;") >= 0))
                {
                    str += art + "=\'" + xmldata[key][art] + "\' ";
                }
                else
                {
                    str += art + "=\"" + xmldata[key][art] + "\" ";
                }
            }
            str += "/>";
            data.Add(str);
        }
        string rootend = "</root>";
        data.Add(rootend);
        File.WriteAllLines(path,data);
    }
    public static void TextSerialization(SortedDictionary<string, Dictionary<string, string>> xmldata, string path)
    {
        List<string> data = new List<string>();
        string head = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?>";
        string root1 = "<root>";
        data.Add(head);
        data.Add(root1);
        foreach (string key in xmldata.Keys)
        {
            string str = "    <trans ";
            foreach (string art in xmldata[key].Keys)
            {
                if (!(xmldata[key][art].IndexOf("&quot;") < 0))
                {
                    str += art + "=\'" + xmldata[key][art] + "\' ";
                }
                else
                {
                    str += art + "=\"" + xmldata[key][art] + "\" ";
                }
            }
            str += "/>";
            data.Add(str);
        }
        string rootend = "</root>";
        data.Add(rootend);
        File.WriteAllLines(path, data);
    }
    public static string XmlString(string text, bool isAttribute = false)
    {
        string str = text;
        if (str.IndexOf("&lt;") >= 0)
        {
            str = str.Replace("&lt;", "<");
            str = XmlString(str, isAttribute);
        }
        else if (str.IndexOf("&gt;") >= 0)
        {
            str = str.Replace("&gt;", ">");
            str = XmlString(str, isAttribute);
        }
        else if (str.IndexOf("&#x0A;") >= 0)
        {
            str = str.Replace("&#x0A;", "\n");
            str = XmlString(str, isAttribute);
        }
        else if (str.IndexOf("&#xD;") >= 0)
        {
            str = str.Replace("&#xD;", "\r");
            str = XmlString(str, isAttribute);
        }
        else if (str.IndexOf("&#x9;") >= 0)
        {
            str = str.Replace("&#x9;", "\t");
            str = XmlString(str, isAttribute);
        }
        else if (str.IndexOf("&quot;") >= 0 && isAttribute)
        {
            str = str.Replace("&quot;", "\"");
            str = XmlString(str, isAttribute);
        }
        else if (str.IndexOf("&apos;") >= 0 && isAttribute)
        {
            str = str.Replace("&apos;", "\'");
            str = XmlString(str, isAttribute);
        }
        else if (str.IndexOf("&amp;") >= 0)
        {
            str = str.Replace("&amp;", "&");
            str = XmlString(str, isAttribute);
        }
        return str;
    }
}

//修改对应的数据
public class XmlData
{
    public int NodeID;
    public string AttributeKey;
    public string AttributeValue;
    public XmlData(int ID, string artkey, string artValue) { NodeID = ID;AttributeKey = artkey;AttributeValue = artValue; }
}
