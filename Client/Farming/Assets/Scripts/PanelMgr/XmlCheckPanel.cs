using JetBrains.Annotations;
using NPOI.OpenXmlFormats.Dml;
using NPOI.SS.UserModel;
using NPOI.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class XmlCheckPanel : MonoBehaviour
{
    public static string[] specialChar = { "lt;", "gt;", "amp;", "apos;", "quot;"};

    private Button BtnHowToUse;
    private Button xmlInspectBtn;
    private Button backBtn;
    private Button searchColor;
    //文件路径输入框
    private InputField tf1InputField;
    private InputField tf2InputField;
    private InputField tf3InputField;
    //文件浏览按钮
    private Button fileBrowseBtn1;
    private Button fileBrowseBtn2;
    private Button fileBrowseBtn3;
    //选择框
    private Toggle chooseToggle1;
    private Toggle chooseToggle2;
    private Toggle chooseToggle3;

    private Text checkResult;
    private Text fileName;

    //public static string checkfilenname = "";
    private Thread thread;

    private GameObject tipPanel;
    private bool tipDisplay = false;

    private GameObject errorPanel;
    private Text errorTipText;
    private bool errorDisplay = false;
    private string errorstr = "";

    private int num = 0;
    private int error1num = 0;
    private int error2num = 0;
    private int error3num = 0;
    private string res1 = "";
    private string res2 = "";
    private string res3 = "";
    int nowfile = 0;

    private string file1 = "";
    private string file2 = "";
    private string file3 = "";
    private bool ison1 = false;
    private bool ison2 = false;
    private bool ison3 = false;


    private void Awake()
    {
        BtnHowToUse = this.transform.Find("BtnHowToUse").GetComponent<Button>();
        xmlInspectBtn = this.transform.Find("XmlInspectButton").GetComponent<Button>();
        backBtn = this.transform.Find("BackButton").GetComponent<Button>();
        searchColor = this.transform.Find("SearchColor").GetComponent<Button>();

        tf1InputField = this.transform.Find("File_1_PathInputField").GetComponent<InputField>();
        tf2InputField = this.transform.Find("File_2_PathInputField").GetComponent<InputField>();
        tf3InputField = this.transform.Find("File_3_PathInputField").GetComponent<InputField>();

        fileBrowseBtn1 = tf1InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn2 = tf2InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn3 = tf3InputField.transform.Find("FileBrowseButton").GetComponent<Button>();

        chooseToggle1 = tf1InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();
        chooseToggle2 = tf2InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();
        chooseToggle3 = tf3InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();

        checkResult = this.transform.Find("CheckResult").Find("Text").GetComponent<Text>();
        fileName = this.transform.Find("FileName").GetComponent<Text>();
        tipPanel = this.transform.Find("TipPanel").gameObject;
        errorPanel = this.transform.Find("ErrorPanel").gameObject;
        errorTipText = errorPanel.transform.Find("Tiptext").GetComponent<Text>();

        xmlInspectBtn.onClick.AddListener(delegate () { thread = new Thread(new ThreadStart(OnXmlCheckBtnClick)); thread.Start(); }/*OnXmlCheckBtnClick*/);
        searchColor.onClick.AddListener(delegate () { thread = new Thread(new ThreadStart(OnSearchColorBtnClick)); thread.Start(); });
        backBtn.onClick.AddListener(delegate ()
        {
            checkResult.text = "";
            tipDisplay = false;
            errorDisplay = false;
            EventMgr.Broadcast(EventType.PanelBack);
        });
        fileBrowseBtn1.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf1InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf1InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[4][1] = tf1InputField.text;
                FilePathMgr.savedata();
            }

        });
        fileBrowseBtn2.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf2InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf2InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[4][2] = tf2InputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn3.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf3InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf3InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[4][3] = tf3InputField.text;
                FilePathMgr.savedata();
            }
        });
        BtnHowToUse.onClick.AddListener(delegate ()
        {
            string tipstr = "1.目标文件1,2,3：对应检查的文件，不勾不检查\n" +
            "2.浏览：打开文件浏览器选择文件\n" +
            "3.勾选框：不勾不检查，勾选才检查\n" +
            "4.普通检查：点击后对勾选的文件进行检查,主要进行括号匹配，xml特殊符号匹配，空格检查\n" +
            "5.检查结果框：显示对应的检查结果于此\n" +
            "6.检查结果：结果存储在MultLanguageMaintenanceTool\\XmlCheckLogFolder";
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, tipstr,null);
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        tf1InputField.text = FilePathMgr.allPathData.Tables[0].Rows[4][1].ToString();
        tf2InputField.text = FilePathMgr.allPathData.Tables[0].Rows[4][2].ToString();
        tf3InputField.text = FilePathMgr.allPathData.Tables[0].Rows[4][3].ToString();
        //Debug.Log("12");
        //string teststr = "&lt;font color=&apos;ui.grade_yellow&apos;&gt;{0}&lt;/font&gt; invites you to join &lt;font color=&apos;ui.grade_yellow&apos;&gt;{1}&lt;/font&gt;, do you join?";
        //Debug.Log(checkColor(teststr));
    }

    // Update is called once per frame
    void Update()
    {
        //if(!checkfilenname.Equals(""))
        //{
        //    fileName.text = checkfilenname;
        //}
        tipPanel.SetActive(tipDisplay);
        errorPanel.SetActive(errorDisplay);
        errorTipText.text = errorstr;
        if(nowfile==1)
        {
            res1 = "文件：" + tf1InputField.text.Trim() + "\n" +
            "已匹配：" + num + "\n" +
            "前后存在空格错误：" + error2num + "\n" +
            "错误编码&amp;：" + error3num + "\n" +
            "括号匹配错误数：" + error1num;
        }
        if(nowfile==2)
        {
            res2 = "文件：" + tf2InputField.text.Trim() + "\n" +
            "已匹配：" + num + "\n" +
            "前后存在空格错误：" + error2num + "\n" +
            "错误编码&amp;：" + error3num + "\n" +
            "括号匹配错误数：" + error1num;
        }
        if (nowfile == 3)
        {
            res3 = "文件：" + tf3InputField.text.Trim() + "\n" +
            "已匹配：" + num + "\n" +
            "前后存在空格错误：" + error2num + "\n" +
            "错误编码&amp;：" + error3num + "\n" +
            "括号匹配错误数：" + error1num;
        }
        checkResult.text = res1 + "\n" + res2 + "\n" + res3;
        file1 = tf1InputField.text;
        file2 = tf2InputField.text;
        file3 = tf3InputField.text;
        ison1 = chooseToggle1.isOn;
        ison2 = chooseToggle2.isOn;
        ison3 = chooseToggle3.isOn;
    }

    private void OnXmlCheckBtnClick()
    {
        tipDisplay = true;
        if (ison1&&!file1.Trim().Equals(""))
        {
            Debug.Log("1");
            nowfile = 1;
            num = 0;
            error1num = 0;
            error2num = 0;
            error3num = 0;
            int i = XmlQuatCheck(file1.Trim());
            if(i>=0)
            {
                errorstr = "文件：" + file1.Trim() + "\n" +
                    "第" + i + "行出现严重的引号匹配错误，请立即退出并进行修改";
                errorDisplay = true;
                Thread.Sleep(100);
                thread.Abort();
                return;
            }
            CheckXml(file1.Trim());
            Debug.Log("end");
        }
        if (ison2 && !file2.Trim().Equals(""))
        {
            nowfile = 2;
            num = 0;
            error1num = 0;
            error2num = 0;
            error3num = 0;
            int i = XmlQuatCheck(file2.Trim());
            if (i >= 0)
            {
                errorstr = "文件：" + file2.Trim() + "\n" +
                    "第" + i + "行出现严重的引号匹配错误，请立即退出并进行修改";
                errorDisplay = true;
                Thread.Sleep(100);
                thread.Abort();
                return;
            }
            CheckXml(file2.Trim());
        }
        if (ison3 && !file3.Trim().Equals(""))
        {
            nowfile = 3;
            num = 0;
            error1num = 0;
            error2num = 0;
            error3num = 0;
            int i = XmlQuatCheck(file3.Trim());
            if (i >= 0)
            {
                errorstr = "文件：" + file3.Trim() + "\n" +
                    "第" + i + "行出现严重的引号匹配错误，请立即退出并进行修改";
                errorDisplay = true;
                Thread.Sleep(100);
                thread.Abort();
                return;
            }
            CheckXml(file3.Trim());
        }
        tipDisplay = false;
        Debug.Log("检查完毕");
        Thread.Sleep(100);
        thread.Abort();
    }
    private void CheckXml(string filePath)
    {
        Debug.Log("2");
        XmlDocument xdoc = XmlHandler.XmlReader2(filePath);
        XmlNode xmlNode = xdoc.SelectSingleNode("root");
        string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute+"_"+ DateTime.Now.Second;
        string logPath = FilePathMgr.xmlcheckfilepath + "\\" + data + ".xlsx";
        DataSet dataSet = new DataSet();
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("key");
        dataTable.Columns.Add("row");
        dataTable.Columns.Add("art");
        dataTable.Columns.Add("result");
        dataTable.Columns.Add("value");
        dataTable.Columns.Add("chanegvalue");
        dataTable.Columns.Add("filepath");
        DataRow dataRow = dataTable.NewRow();
        dataRow[0] = "title";
        dataTable.Rows.Add(dataRow);
        dataTable.Rows[0][0] = "key";
        dataTable.Rows[0][1] = "行数";
        dataTable.Rows[0][2] = "属性名";
        dataTable.Rows[0][3] = "结果";
        dataTable.Rows[0][4] = "属性值";
        dataTable.Rows[0][5] = "修改值";
        dataTable.Rows[0][6] = "文件位置";
        dataSet.Tables.Add(dataTable);

        int excelRow = 1;
        for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
        {
            for (int j = 0; j < xmlNode.ChildNodes[i].Attributes.Count; j++)
            {
                num++;
                //Debug.Log("row:" + i + " col:" + j);
                string value = System.Security.SecurityElement.Escape(xmlNode.ChildNodes[i].Attributes[j].Value);
                if (value.IndexOf("&#x0A") >= 0) Debug.Log(i);
                if (value.Equals("")) continue;
                string result = checkModel(value);
                if (result.Equals(""))
                {
                    continue;
                }
                else if (result == "error1")
                {
                    dataSet.Tables[0].Rows.Add(excelRow);
                    dataSet.Tables[0].Rows[excelRow][0] = System.Security.SecurityElement.Escape(xmlNode.ChildNodes[i].Attributes[0].Value);
                    dataSet.Tables[0].Rows[excelRow][1] = (i + 3);
                    dataSet.Tables[0].Rows[excelRow][2] = System.Security.SecurityElement.Escape(xmlNode.ChildNodes[i].Attributes[j].Name);
                    dataSet.Tables[0].Rows[excelRow][3] = "括号匹配失败";
                    dataSet.Tables[0].Rows[excelRow][4] = "RED" + value;
                    dataSet.Tables[0].Rows[excelRow][5] = "请自行修改";
                    dataSet.Tables[0].Rows[excelRow][6] = xmlNode.ChildNodes[i].Attributes[xmlNode.ChildNodes[i].Attributes.Count - 1].Value;
                    excelRow++;
                    error1num++;
                }
                else if(result == "error2")
                {
                    
                    dataSet.Tables[0].Rows.Add(excelRow);
                    dataSet.Tables[0].Rows[excelRow][0] = System.Security.SecurityElement.Escape(xmlNode.ChildNodes[i].Attributes[0].Value);
                    dataSet.Tables[0].Rows[excelRow][1] = (i + 3);
                    dataSet.Tables[0].Rows[excelRow][2] = System.Security.SecurityElement.Escape(xmlNode.ChildNodes[i].Attributes[j].Name);
                    dataSet.Tables[0].Rows[excelRow][3] = "存在错误的编码（&amp;）";
                    dataSet.Tables[0].Rows[excelRow][4] = "RED"+value;
                    dataSet.Tables[0].Rows[excelRow][5] =  value.Replace("&amp;", "&") ;
                    dataSet.Tables[0].Rows[excelRow][6] = xmlNode.ChildNodes[i].Attributes[xmlNode.ChildNodes[i].Attributes.Count - 1].Value;
                    excelRow++;
                    error3num++;
                }
                else
                {
                    //xmlNode.ChildNodes[i].Attributes[j].Value = result;
                    dataSet.Tables[0].Rows.Add(excelRow);
                    dataSet.Tables[0].Rows[excelRow][0] = System.Security.SecurityElement.Escape(xmlNode.ChildNodes[i].Attributes[0].Value);
                    dataSet.Tables[0].Rows[excelRow][1] = (i + 3);
                    dataSet.Tables[0].Rows[excelRow][2] = System.Security.SecurityElement.Escape(xmlNode.ChildNodes[i].Attributes[j].Name);
                    dataSet.Tables[0].Rows[excelRow][3] = "前后存在空格请修改";
                    dataSet.Tables[0].Rows[excelRow][4] = "RED" + value;
                    dataSet.Tables[0].Rows[excelRow][5] = result;
                    dataSet.Tables[0].Rows[excelRow][6] = xmlNode.ChildNodes[i].Attributes[xmlNode.ChildNodes[i].Attributes.Count - 1].Value;
                    excelRow++;
                    error2num++;
                }
            }
        }

        //XmlHandler.XmlSave(xdoc, checkfilenname);
        ExcelHandler.CreateExcel(dataSet, logPath);
        EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, "检查已完成，日志已保存到：" + logPath, FilePathMgr.xmlcheckfilepath);
    }

    //检查xml
    private string checkModel(string str)
    {
        bool errorstr = false;
        if (str.Trim() != str)
        {
            str = str.Trim();
            errorstr = true;
        }
        if(str.IndexOf("&amp;")>=0)
        {
            for(int i =0;i<specialChar.Length;i++)
            {
                if(str.IndexOf("&amp;"+specialChar[i])>=0)
                {
                    return "error2";
                }
            }
        }
        Stack<char> stk = new Stack<char>();
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '{') stk.Push('}');
            else if (str[i] == '[') stk.Push(']');
            else if (str[i] == '}' || str[i] == ']')
            {
                if (stk.Count == 0) return "error1";
                if (stk.Peek() == str[i])
                {
                    stk.Pop();
                }
                else
                {
                    return "error1";
                }
            }
        }
        if (stk.Count > 0)
        {
            return "error1";
        }
        else
        {
            if (errorstr) return str;
            else return "";
        }
    }

    private void OnSearchColorBtnClick()
    {

        tipDisplay = true;
        if (ison1 && !file1.Trim().Equals(""))
        {
            nowfile = 1;
            num = 0;
            error1num = 0;
            error2num = 0;
            error3num = 0;
            xmlColorCheck(file1.Trim());
        }
        if (ison2 && !file2.Trim().Equals(""))
        {
            nowfile = 2;
            num = 0;
            error1num = 0;
            error2num = 0;
            error3num = 0;
            xmlColorCheck(file2.Trim());
        }
        if (ison3 && !file3.Trim().Equals(""))
        {
            nowfile = 3;
            num = 0;
            error1num = 0;
            error2num = 0;
            error3num = 0;
            xmlColorCheck(file3.Trim());
        }
        tipDisplay = false;
        Thread.Sleep(100);
        thread.Abort();
    }


    private void xmlColorCheck(string filepath)
    {
        tipDisplay = true;
        XmlDocument xdoc = XmlHandler.XmlReader2(filepath);
        XmlNode xmlNode = xdoc.SelectSingleNode("root");
        string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
        string logPath = FilePathMgr.xmlcheckfilepath + "\\" + data + ".xlsx";
        DataSet dataSet = new DataSet();
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("key");
        dataTable.Columns.Add("row");
        dataTable.Columns.Add("art");
        dataTable.Columns.Add("result");
        dataTable.Columns.Add("value");
        dataTable.Columns.Add("chanegvalue");
        dataTable.Columns.Add("filepath");
        DataRow dataRow = dataTable.NewRow();
        dataRow[0] = "title";
        dataTable.Rows.Add(dataRow);
        dataTable.Rows[0][0] = "key";
        dataTable.Rows[0][1] = "行数";
        dataTable.Rows[0][2] = "属性名";
        dataTable.Rows[0][3] = "结果";
        dataTable.Rows[0][4] = "属性值";
        dataTable.Rows[0][5] = "修改值";
        dataTable.Rows[0][6] = "文件位置";
        dataSet.Tables.Add(dataTable);

        int excelRow = 1;
        for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
        {
            for (int j = 0; j < xmlNode.ChildNodes[i].Attributes.Count; j++)
            {
                num++;
                //Debug.Log("row:" + i + " col:" + j);
                string value = System.Security.SecurityElement.Escape(xmlNode.ChildNodes[i].Attributes[j].Value);
                if (value.IndexOf("&#x0A") >= 0) Debug.Log(i);
                if (value.Equals("")) continue;
                string result = checkColor(value);
                if (result.Equals(value))
                {
                    continue;
                }
                else
                {
                    dataSet.Tables[0].Rows.Add(excelRow);
                    dataSet.Tables[0].Rows[excelRow][0] = xmlNode.ChildNodes[i].Attributes[0].Value;
                    dataSet.Tables[0].Rows[excelRow][1] = (i + 3);
                    dataSet.Tables[0].Rows[excelRow][2] = xmlNode.ChildNodes[i].Attributes[j].Name;
                    dataSet.Tables[0].Rows[excelRow][3] = "匹配到对应color";
                    dataSet.Tables[0].Rows[excelRow][4] = "RED" + xmlNode.ChildNodes[i].Attributes[j].Value;
                    dataSet.Tables[0].Rows[excelRow][5] = result;
                    dataSet.Tables[0].Rows[excelRow][6] = xmlNode.ChildNodes[i].Attributes[xmlNode.ChildNodes[i].Attributes.Count - 1].Value;
                    excelRow++;
                }
            }
        }
        //XmlHandler.XmlSave(xdoc, checkfilenname);
        ExcelHandler.CreateExcel(dataSet, logPath);
    }

    private string checkColor(string str)
    {
        Debug.Log(str);
        while (str.IndexOf("&lt;font color") >=0)//说明是一个颜色解释的字符串
        {
            //例如
            //&lt;font color=&apos;ui.grade_yellow&apos;&gt;{0}&lt;/font&gt; invites you to join &lt;font color=&apos;ui.grade_yellow&apos;&gt;{1}&lt;/font&gt;, do you join?
            //&lt; font color = &apos; ui.grade_yellow & apos; &gt;{ 0}&lt;/ font & gt;
            //解释：
            //在&lt;font color=&apos;ui.grade_yellow&apos;&gt;和&lt;/font&gt;之间的{0}即为正确的编码
            //正确字符串为{0} invites you to join {1}, do you join?
            int index1 = str.IndexOf("&lt;font color");//0
            string temp1 = str.Substring(index1 + 4, str.Length - index1 - 4);
            int index2 = temp1.IndexOf("&gt;");//39
            string temp2 = temp1.Substring(index2 + 4, temp1.Length - index2 - 4);
            int index3 = temp2.IndexOf("&lt;");//3
            string realstr = temp2.Substring(0, index3);
            int index4 = temp2.IndexOf("&gt;");//12
            int len = index1 + index2 + index4 + 9 + realstr.Length;
            if (len > str.Length) str = str.Substring(0, index1) + realstr;
            else str = str.Substring(0, index1) + realstr + str.Substring(len, str.Length - len);
            Debug.Log(str);
        }
        return str;
    }
    
    private int XmlQuatCheck(string path)
    {
        string[] infos = File.ReadAllLines(path);
        for(int i=0;i<infos.Length;i++)
        {
            if(!XmlCheckPanel.quatCheck(infos[i]))
            {
                return i;
            }
        }
        return -1;
    }


    public static bool quatCheck(string str)
    {
        Stack<char> stk = new Stack<char>();
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '\''|| str[i] == '\"')
            {
                if (stk.Count == 0) stk.Push(str[i]);
                else if (stk.Peek() == str[i])
                {
                    stk.Pop();
                }
                else
                {
                    stk.Push(str[i]);
                }
            }
        }
        if (stk.Count > 0)
        {
            return false;
        }
        return true;
    }


}
