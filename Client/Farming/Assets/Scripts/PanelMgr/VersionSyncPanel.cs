using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Text.RegularExpressions;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.Util;
using System.Data;
using System.Text;

public class VersionSyncPanel : MonoBehaviour
{
	public GameObject LogText;
    #region UI控件声明
    private Button versionCompareBtn;
    private Button BtnHowToUse;
	private Button backBtn;

    private Text tipText;
	//private Text fileText;

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

	private GameObject versionContent;
	private GridLayoutGroup contentGrid;
    private GameObject tipPanel;
    private bool tipDisplay = false;

    private InputField versionInput;
    private InputField versionTimeInputField;
    #endregion

    Thread thread;
	public static string syncFilePath = "";
	public static string tf1FilePath = "";
	public static string tf2FilePath = "";
	public static string outPath = "";

	public static object locker1 = new object();
	public static object locker2 = new object();
	public static string version = "";
	public static string data = "";
	private bool isover = false;
	private bool isover2 = false;

	private string logPath = "";
    public static string logFileName = "";

    private void Awake()
    {
        #region 获取UI
        versionCompareBtn = this.transform.Find("VersionCompareButton").GetComponent<Button>();
        BtnHowToUse = this.transform.Find("BtnHowToUse").GetComponent<Button>();
		backBtn = this.transform.Find("BackButton").GetComponent<Button>();

        tf1InputField = this.transform.Find("File_1_PathInputField").GetComponent<InputField>();
		tf2InputField = this.transform.Find("File_2_PathInputField").GetComponent<InputField>();
		tf3InputField = this.transform.Find("File_3_PathInputField").GetComponent<InputField>();

		fileBrowseBtn1 = tf1InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
		fileBrowseBtn2 = tf2InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
		fileBrowseBtn3 = tf3InputField.transform.Find("FileBrowseButton").GetComponent<Button>();

		chooseToggle1 = tf1InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();
		chooseToggle2 = tf2InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();
		chooseToggle3 = tf3InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();

		versionInput = this.transform.Find("VersionInputField").GetComponent<InputField>();
		versionTimeInputField = this.transform.Find("VersionTimeInputField").GetComponent<InputField>();

        versionContent = this.transform.Find("VersionScrollView").GetComponent<ScrollRect>().content.gameObject;
        tipPanel = this.transform.Find("TipPanel").gameObject;

        contentGrid = versionContent.GetComponent<GridLayoutGroup>();

		tipText = this.transform.Find("TipText").GetComponent<Text>();
		//fileText = versionInput.transform.Find("FileText").GetComponent<Text>();
        #endregion

        #region 添加UI事件
        versionCompareBtn.onClick.AddListener(OnVersionCompareBtnClick);
		backBtn.onClick.AddListener(OnBackClick);
		fileBrowseBtn1.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf1InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf1InputField.text = filename;
                if (chooseToggle1.isOn) CreateLog(tf1InputField.text);
                FilePathMgr.allPathData.Tables[0].Rows[3][1] = tf1InputField.text;
                FilePathMgr.savedata();
            }
		});
		fileBrowseBtn2.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf2InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf2InputField.text = filename;
                if (chooseToggle2.isOn) CreateLog(tf2InputField.text);
                FilePathMgr.allPathData.Tables[0].Rows[3][2] = tf2InputField.text;
                FilePathMgr.savedata();
            }
		});
		fileBrowseBtn3.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf3InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf3InputField.text = filename;
                if (chooseToggle3.isOn) CreateLog(tf3InputField.text);
                FilePathMgr.allPathData.Tables[0].Rows[3][3] = tf3InputField.text;
                FilePathMgr.savedata();
            }
        });

		chooseToggle1.onValueChanged.AddListener(delegate (bool valueOn)
		{
			if (valueOn)
			{
				chooseToggle2.isOn = false;
				chooseToggle3.isOn = false;
				CreateLog(tf1InputField.text);
			}
		});
		chooseToggle2.onValueChanged.AddListener(delegate (bool valueOn)
		{
			if (valueOn)
			{
				chooseToggle1.isOn = false;
				chooseToggle3.isOn = false;
				CreateLog(tf2InputField.text);
			}
		});
		chooseToggle3.onValueChanged.AddListener(delegate (bool valueOn)
		{
			if (valueOn)
			{
				chooseToggle1.isOn = false;
				chooseToggle2.isOn = false;
				CreateLog(tf3InputField.text);
			}
		});
        BtnHowToUse.onClick.AddListener(delegate ()
        {
            string tipstr = "1.目标文件：指获取改动日志的文件，会将勾选了的文件改动同步到未勾选的文件中去\n" +
            "2.浏览按钮：点击后进行对应文件浏览\n" +
            "3.版本号：输入下方日志框中的版本号，将会获取该版本进行的改动日志，输入格式包括：①单个版本号，获取该版本的改变②版本号1:版本号2，获取两个版本的不同\n" +
            "4.进行版本对比按钮：点击后根据输入的版本号进行版本对比并进去xml同步页面\n" +
            "5.时间(天):当默认时间（3天）内没有改动时会导致日志页面不显示对应的日志，此时可以在这里面进行输入扩大时间范围(2021-02-04加)。\n" +
            "6.日志：日志在MultLanguageMaintenanceTool\\SyncVersionLogFolder(svn log)、MultLanguageMaintenanceTool\\SyncTextLogFolder（版本修改日志）、MultLanguageMaintenanceTool\\SyncExcelLogFolder（版本修改日志）";
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, tipstr,null);
        });
        chooseToggle1.isOn = true;
		chooseToggle2.isOn = false;
		chooseToggle3.isOn = false;
        //contentGrid.cellSize = new Vector2(versionContent.GetComponent<RectTransform>().rect.size.x, 30);
        #endregion

        versionTimeInputField.onEndEdit.AddListener((string timestr) =>
        {
            CreateLog(syncFilePath);
        }
        );
	}
    // Start is called before the first frame update
    void Start()
    {
		tf1InputField.text = FilePathMgr.allPathData.Tables[0].Rows[3][1].ToString();
		tf2InputField.text = FilePathMgr.allPathData.Tables[0].Rows[3][2].ToString();
		tf3InputField.text = FilePathMgr.allPathData.Tables[0].Rows[3][3].ToString(); 
		if (chooseToggle1.isOn)
		{
			CreateLog(tf1InputField.text);
		}
		else if (chooseToggle2.isOn)
		{
			CreateLog(tf2InputField.text);
		}
		else if (chooseToggle3.isOn)
		{
			CreateLog(tf3InputField.text);
		}
		
	}

    // Update is called once per frame
    void Update()
	{
        tipPanel.SetActive(tipDisplay);
        if (chooseToggle1.isOn)
		{
			syncFilePath = tf1InputField.text;
			tf1FilePath = tf2InputField.text;
			tf2FilePath = tf3InputField.text;
		}
		else if (chooseToggle2.isOn)
		{
			syncFilePath = tf2InputField.text;
			tf1FilePath = tf1InputField.text;
			tf2FilePath = tf3InputField.text;
		}
		else if (chooseToggle3.isOn)
		{
			syncFilePath = tf3InputField.text;
			tf1FilePath = tf1InputField.text;
			tf2FilePath = tf2InputField.text;
		}
		if (!thread.IsAlive && isover)
		{
			try
			{
				isover = false;
				Thread.Sleep(100);
                tipText.text = "获取改动日志中..";
                tipDisplay = true;
                string[] info = File.ReadAllLines(logPath, Encoding.GetEncoding("GBK"));
                tipText.text = "创建文字";
                for (int i = 0; i < info.Length; i++)
                {
                    if (info[i].Trim().Equals("") || info[i][0] == '-') continue;
                    GameObject versionlog = Instantiate(LogText, versionContent.transform);
                    if (info[i].IndexOf('|') >= 0)
                    {
                        string[] split = info[i].Split('|');
                        versionlog.GetComponent<Text>().text = "版本：  " + split[0] + "\t修改者：" + split[1] + "备注：";
                    }
                    else
                    {
                        versionlog.GetComponent<Text>().text = info[i];
                    }
                }
                tipDisplay = false;
            }
			catch(Exception e)
            {
                UnityEngine.Debug.Log(e);
				isover = true;
            }
		}
		if (!thread.IsAlive && isover2)
		{
			try
            {
                tipDisplay = true;
                isover2 = false;
				tipText.text = "日志已输出到" + outPath;
                string[] info = File.ReadAllLines(VersionSyncPanel.outPath);
                //UnityEngine.Debug.Log("修改日志长度"+info.Length);
                if (info.Length <= 0)
                {

                    tipText.text = "没有改动信息，是否输入了错误的版本号？";
                    tipDisplay = false;
                    return;
                }
                CreateExcelByText(info);
                tipDisplay = false;
                EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, "版本对比完成!\n" + tipText.text, FilePathMgr.synctextfilepath);
                EventMgr.Broadcast(EventType.XmlSyncPanelDisplay);
                EventMgr.Broadcast(EventType.VersionToXmlSync);
			}
			catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
                isover2 = true;
			}

		}
	}

	private void CreateLog(string path)
	{
		syncFilePath = path;
		if (path.Trim().Equals("")) return;
		for (int i = 0; i < versionContent.transform.childCount; i++)
		{
			Destroy(versionContent.transform.GetChild(i).gameObject);
		}
		//svn log -r {2020-11-07}:{2020-11-11} "E:\Work\config\localization\i18n.xml">"D:\Users\Administrator\Desktop\2.txt"
		data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
        int timeLast = 3;
        if (!versionTimeInputField.text.Equals(""))
        {
            timeLast = int.Parse(versionTimeInputField.text);
        }
        UnityEngine.Debug.Log(timeLast);
		DateTime forwarddt = DateTime.Now.AddDays(-timeLast+1).Date;
		DateTime latedt = DateTime.Now.AddDays(1).Date;
		logPath = FilePathMgr.syncVersionLogFolder + "\\" + data + ".txt";
		thread = new Thread(new ThreadStart(delegate ()
        {
            string cmdstr = "svn log -r {" + latedt.Year + "-" + latedt.Month + "-" + latedt.Day +
            "}:{" + forwarddt.Year + "-" + forwarddt.Month + "-" + forwarddt.Day + "} \"" + path + "\">\"" + logPath + "\"";
            UnityEngine.Debug.Log(cmdstr);
            ProcessCommand("cmd.exe", cmdstr);
			thread.Abort();
        }));
        thread.Start();
        isover = true;
        Thread.Sleep(300);
    }


	private void OnVersionCompareBtnClick()
    {
		data = DateTime.Now.Year +""+ DateTime.Now.Month+"" + DateTime.Now.Day +"_"+ DateTime.Now.Hour +"_"+ DateTime.Now.Minute;
        try
        {
            if (versionInput.text.IndexOf(":") >= 0)
            {
                version = versionInput.text.Split(':')[0] + "_" + versionInput.text.Split(':')[1];
                Convert.ToInt32(versionInput.text.Split(':')[0]);
                Convert.ToInt32(versionInput.text.Split(':')[1]);
            }
            else
            {
                version = versionInput.text;
                Convert.ToInt32(versionInput.text);
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log(e);
            versionInput.text = "";
            tipText.text = "<color=red>请输入正确的版本号</color>";
            return;
        }

        
		outPath = FilePathMgr.synctextfilepath+"\\"+version+"_"+data + ".txt";
		//outPath = outPath.Replace(" ", "");
		thread = new Thread(new ThreadStart(CmdCtr));
        thread.Start();
        isover2 = true;
    }
    //通过输出日志的txt文件建立对应的excel文件
    private void CreateExcelByText(string[] info)
    {
        //被删除的key对应的元素
        Dictionary<string, Dictionary<string, string>> deleteKeyDic = new Dictionary<string, Dictionary<string, string>>();
        //被添加的key对应的元素
        Dictionary<string, Dictionary<string, string>> addKeyDic = new Dictionary<string, Dictionary<string, string>>();
        for (int i = 4; i < info.Length; i++)
        {
            if (info[i][0] == '-')
            {
                //说明此行为删除行
                //解析此行对应的元素
                Dictionary<string, string> keyValuePairs = TextDeserialization(info[i]);
                if (keyValuePairs.ContainsKey("key")&&!deleteKeyDic.ContainsKey(keyValuePairs["key"]))
                {
                    deleteKeyDic[keyValuePairs["key"]]= keyValuePairs;
                }
            }
            else if (info[i][0] == '+')
            {
                //解析此行对应的元素
                Dictionary<string, string> keyValuePairs = TextDeserialization(info[i]);
                if (keyValuePairs.ContainsKey("key") && !addKeyDic.ContainsKey(keyValuePairs["key"]))
                {
                    addKeyDic[keyValuePairs["key"]] = keyValuePairs;
                }
            }
        }
        logFileName = VersionSyncPanel.outPath.Substring(VersionSyncPanel.outPath.LastIndexOf('\\') + 1, VersionSyncPanel.outPath.LastIndexOf('.') - VersionSyncPanel.outPath.LastIndexOf('\\') - 1);
        string logFilePath = FilePathMgr.xyncexcelfilepath + "\\" + logFileName + ".xlsx";
        DataSet dataSet = new DataSet();
        DataTable dataTable = new DataTable();
        DataTable dataTable1 = new DataTable();

        dataTable.Columns.Add("key");
        dataTable.Columns.Add("result");
        DataRow dataRow = dataTable.NewRow();
        dataRow[0] = "title";
        dataTable.Rows.Add(dataRow);
        dataSet.Tables.Add(dataTable);
        dataTable.Rows[0][0] = "key";
        dataTable.Rows[0][1] = "result";
        dataTable1.Columns.Add("1");
        dataTable1.Columns.Add("2");
        dataTable1.Columns.Add("3");
        dataTable1.Columns.Add("4");
        dataTable1.Columns.Add("5");
        dataTable1.Columns.Add("6");
        dataTable1.Columns.Add("7");
        dataTable1.Columns.Add("8");
        dataTable1.Columns.Add("9");
        dataTable1.Columns.Add("10");
        dataTable1.Columns.Add("11");
        dataTable1.Columns.Add("12");
        DataRow dataRow1 = dataTable1.NewRow();
        dataRow1[0] = "title";
        dataTable1.Rows.Add(dataRow1);
        dataSet.Tables.Add(dataTable1);
        dataTable1.Rows[0][0] = "KEY";
        dataTable1.Rows[0][1] = "UI文件名";
        dataTable1.Rows[0][2] = "接口截图 / 截图连结\nScreenshot Image / URL";
        dataTable1.Rows[0][3] = "推荐/预先/临时翻译 需严格校对\nSuggested text";
        dataTable1.Rows[0][4] = "接口中文\nChinese Text";
        dataTable1.Rows[0][5] = "翻译\nSG Trans";
        dataTable1.Rows[0][6] = "字符数\nChar Count";
        dataTable1.Rows[0][7] = "翻译部备注\nDev / SG Notes";
        dataTable1.Rows[0][8] = "校对版\nUS Final";
        dataTable1.Rows[0][9] = "字符数\nChar Count";
        dataTable1.Rows[0][10] = "字符限制\nChar Limit";
        dataTable1.Rows[0][11] = "编辑部备注\nUS Notes";


        if (deleteKeyDic.Count != 0)
        {
            foreach (string key in deleteKeyDic.Keys)
            {
                int i = 1;
                foreach (string key2 in deleteKeyDic[key].Keys)
                {
                    if (!key2.Equals("key"))
                    {
                        dataSet.Tables[0].Columns.Add(key2);
                        dataSet.Tables[0].Rows[0][1 + i] = key2;
                        i++;
                    }
                }
                break;
            }
        }
        else if (addKeyDic.Count != 0)
        {
            foreach (string key in addKeyDic.Keys)
            {
                int i = 1;
                foreach (string key2 in addKeyDic[key].Keys)
                {
                    if (!key2.Equals("key"))
                    {
                        dataSet.Tables[0].Columns.Add(key2);
                        dataSet.Tables[0].Rows[0][1 + i] = key2;
                        i++;
                    }
                }
                break;
            }
        }


        //对于每个删除列表里面的列表
        //对于key如果在增加列表里面有找到
        //那么即为修改项
        //如果未找到，即为删除项
        int k = 1;
        int k1 = 1;
        foreach (string key in deleteKeyDic.Keys)
        {
            if (addKeyDic.ContainsKey(key))
            {
                int j = 1;
                dataTable.Rows.Add(key);
                dataSet.Tables[0].Rows[k][0] = key;
                dataSet.Tables[0].Rows[k][1] = 1;
                dataTable1.Rows.Add(key);
                dataSet.Tables[1].Rows[k1][0] = key;
                if (deleteKeyDic[key].ContainsKey("trunk"))
                {
                    dataSet.Tables[1].Rows[k1][4] = deleteKeyDic[key]["trunk"];
                }
                foreach (string key2 in deleteKeyDic[key].Keys)
                {
                    if (!key2.Equals("key"))
                    {
                        if(!addKeyDic[key].ContainsKey(key2))
                        {
                            UnityEngine.Debug.Log("错误的信息:"+key+"\t"+key2);
                            return;
                        }
                        if (!deleteKeyDic[key][key2].Equals(addKeyDic[key][key2]))
                        {
                            dataSet.Tables[0].Rows[k][1 + j] = "RED<" + deleteKeyDic[key][key2] + ">" + addKeyDic[key][key2];
                        }
                        j++;
                    }
                }
                k1++;
                addKeyDic.Remove(key);
            }
            else
            {
                if (deleteKeyDic[key].ContainsKey("trunk"))
                {
                    //如果trunk相同，那么key被改了
                    //也加入表中
                    string iskeychange = "";
                    foreach (string akey in addKeyDic.Keys)
                    {
                        if(addKeyDic[akey].ContainsKey("trunk"))
                        {
                            continue;
                        }
                        if (deleteKeyDic[key]["trunk"].Equals(addKeyDic[akey]["trunk"]))
                        {
                            iskeychange = akey;
                            int n = 1;
                            dataTable.Rows.Add(akey);
                            dataSet.Tables[0].Rows[k][0] = "RED<" + key + ">" + akey;
                            dataSet.Tables[0].Rows[k][1] = 4;
                            dataTable1.Rows.Add(akey);
                            dataSet.Tables[1].Rows[k1][0] = akey;
                            dataSet.Tables[1].Rows[k1][4] = deleteKeyDic[akey]["trunk"];
                            k1++;
                            foreach (string key2 in deleteKeyDic[key].Keys)
                            {
                                if (!key2.Equals("key"))
                                {
                                    dataSet.Tables[0].Rows[k][1 + n] = deleteKeyDic[key][key2];
                                    n++;
                                }
                            }
                            break;
                        }
                    }
                    if (!iskeychange.Equals(""))
                    {
                        addKeyDic.Remove(iskeychange);
                        continue;
                    }
                }
                int j = 1;
                dataTable.Rows.Add(key);
                dataSet.Tables[0].Rows[k][0] = key;
                dataSet.Tables[0].Rows[k][1] = 2;
                foreach (string key2 in deleteKeyDic[key].Keys)
                {
                    if (!key2.Equals("key"))
                    {
                        dataSet.Tables[0].Rows[k][1 + j] = "<" + deleteKeyDic[key][key2] + ">null";
                        j++;
                    }
                }
            }
            k++;
        }
        foreach (string key in addKeyDic.Keys)
        {
            int j = 1;
            dataTable.Rows.Add(key);
            dataSet.Tables[0].Rows[k][0] = key;
            dataSet.Tables[0].Rows[k][1] = 3;
            foreach (string key2 in addKeyDic[key].Keys)
            {
                if (!key2.Equals("key"))
                {
                    dataSet.Tables[0].Rows[k][1 + j] = "<null>" + addKeyDic[key][key2];
                    j++;
                }
            }
            k++;
        }
        ExcelHandler.CreateExcel(dataSet, logFilePath);
    }

    private Dictionary<string, string> TextDeserialization(string text)
    {
        //text = System.Security.SecurityElement.Escape(text);
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        string[] vs = text.Split(new char[] { '\"', '\'' });
        for (int i = 0; i < vs.Length; i++)
        {
            int indexOfequl = vs[i].IndexOf('=');
            if (indexOfequl > 0)
            {
                try
                {
                    int indexOfSpace = vs[i].LastIndexOf(' ');
                    if(indexOfequl - indexOfSpace - 1<=0)
                    {
                        UnityEngine.Debug.Log("error xml data info:"+text);
                        return new Dictionary<string, string>();
                    }
                    string key = vs[i].Substring(indexOfSpace + 1, indexOfequl - indexOfSpace - 1);
                    string value = vs[i + 1];
                    i++;
                    if(keyValuePairs.ContainsKey(key))
                    {
                        UnityEngine.Debug.Log("error xml data info:" + text);
                        return new Dictionary<string, string>();
                    }
                    else
                    {
                        keyValuePairs.Add(key, value);
                    }
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.Log(e);
                    tipDisplay = true;
                    tipPanel.transform.Find("Text").GetComponent<Text>().text = "内容有单引号,请将内容的单引号修改为<color=red>&apos;</color>:" + text;
                    return new Dictionary<string, string>();
                }
            }
        }
        return keyValuePairs;
    }

    private void OnBackClick()
    {
        isover = false;
        isover2 = false;
		versionInput.text = "";
		tipText.text = "请输入版本号来进行版本比对";
        tipDisplay = false;
        tipPanel.transform.Find("Text").GetComponent<Text>().text = "日志生成中请稍后";
        outPath = "";
		syncFilePath = "";
		EventMgr.Broadcast(EventType.PanelBack);
    }

    private void CmdCtr()
    {
		//svn log -r {2020-11-07}:{2020-11-11} "E:\Work\config\localization\i18n.xml">"D:\Users\Administrator\Desktop\2.txt"
		if (versionInput.text.Equals(""))
		{
			ProcessCommand("cmd.exe", "svn diff " + " \"" + syncFilePath + "\">\"" + outPath + "\"");
		}
		else if(versionInput.text.IndexOf(":")>0)
		{
			ProcessCommand("cmd.exe", "svn diff -r " + versionInput.text + " \"" + syncFilePath + "\">\"" + outPath + "\"");
		}
		else
        {
			string oldversion = (Convert.ToInt32(versionInput.text)-1).ToString();
			UnityEngine.Debug.Log(oldversion);

			ProcessCommand("cmd.exe", "svn diff -r " + oldversion + ":" + versionInput.text + " \"" + syncFilePath + "\">\"" + outPath + "\"");
		}
		thread.Abort();
		//ProcessCommand("cmd.exe", @"svn diff -r 33059 E:\Work\config\localization\temp\all_key.xml > D:\Users\Administrator\Desktop\svndiff.xml");
		//ProcessCommand("cmd.exe", "/command:cd..");
		//Process.Start("C:\\Windows\\System32\\cmd.exe", "svn diff -r 33059 E:\\Work\\config\\localization\\temp > D:\\Users\\Administrator\\Desktop\\svndiff.txt");
	}

    public static void ProcessCommand(string command, string argument)
	{
		Process process;
		lock (locker2)
		{//UnityEngine.Debug.Log(argument);
			ProcessStartInfo info = new ProcessStartInfo(command);
			//启动应用程序时要使用的一组命令行参数。
			//但是对于cmd来说好像是无效的，可能是因为UseShellExecute的值设置为false了
			//但是对于svn的程序TortoiseProc.exe是可以使用的一个参数
			//info.Arguments = argument;
			//是否弹窗
			info.CreateNoWindow = true;
			//获取或设置指示不能启动进程时是否向用户显示错误对话框的值。
			info.ErrorDialog = true;
			//获取或设置指示是否使用操作系统 shell 启动进程的值。
			info.UseShellExecute = false;

			if (info.UseShellExecute)
			{
				info.RedirectStandardOutput = false;
				info.RedirectStandardError = false;
				info.RedirectStandardInput = false;
			}
			else
			{
				info.RedirectStandardOutput = true; //获取或设置指示是否将应用程序的错误输出写入 StandardError 流中的值。
				info.RedirectStandardError = true; //获取或设置指示是否将应用程序的错误输出写入 StandardError 流中的值。
				info.RedirectStandardInput = true;//获取或设置指示应用程序的输入是否从 StandardInput 流中读取的值。
				info.StandardOutputEncoding = System.Text.Encoding.UTF8;
				info.StandardErrorEncoding = System.Text.Encoding.UTF8;
			}
			//启动(或重用)此 Process 组件的 StartInfo 属性指定的进程资源，并将其与该组件关联。
			process = Process.Start(info);
			//StandardInput：获取用于写入应用程序输入的流。
			//将字符数组写入文本流，后跟行终止符。
			process.StandardInput.WriteLine(argument);
			//获取或设置一个值，该值指示 StreamWriter 在每次调用 Write(Char) 之后是否都将其缓冲区刷新到基础流。
			process.StandardInput.AutoFlush = true;

			if (!info.UseShellExecute)
			{
				UnityEngine.Debug.Log(process.StandardOutput);
				UnityEngine.Debug.Log(process.StandardError);
			}
		}
		//等待关闭
		//process.WaitForExit();
		//关闭
		process.Close();
	}

}
