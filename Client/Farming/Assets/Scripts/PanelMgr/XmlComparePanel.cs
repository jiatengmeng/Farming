using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using JetBrains.Annotations;
using System.IO;
using System;
using System.Threading;
using System.Text;

public class XmlComparePanel : MonoBehaviour
{
    private Button backButton;
    private Button BtnHowToUse;
    private Button CompareButton;
    //文件路径输入框
    private InputField rfInputField;
    private InputField tf1InputField;
    private InputField tf2InputField;
    //文件浏览按钮
    private Button fileBrowseBtn1;
    private Button fileBrowseBtn2;
    private Button fileBrowseBtn3;

    //选择框
    private Toggle chooseToggle;

    private Text resText;
    private GameObject tipPanel;
    private bool tipDisplay = false;
    private string tip = "";
    Thread thread;

    private void Awake()
    {
        backButton = this.transform.Find("BackButton").GetComponent<Button>();
        BtnHowToUse = this.transform.Find("BtnHowToUse").GetComponent<Button>();
        CompareButton = this.transform.Find("CompareButton").GetComponent<Button>();
        rfInputField = this.transform.Find("RFilePathInputField").GetComponent<InputField>();
        tf1InputField = this.transform.Find("T1FilePathInputField").GetComponent<InputField>();
        tf2InputField = this.transform.Find("T2FilePathInputField").GetComponent<InputField>();

        fileBrowseBtn1 = rfInputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn2 = tf1InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn3 = tf2InputField.transform.Find("FileBrowseButton").GetComponent<Button>();

        chooseToggle = tf2InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();

        resText = this.transform.Find("ResultTextBackground").Find("ResultText").GetComponent<Text>();

        tipPanel = this.transform.Find("TipPanel").gameObject;

        fileBrowseBtn1.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(rfInputField.text).ToString());
            if (!filename.Equals(""))
            {
                rfInputField.text = filename; 
                FilePathMgr.allPathData.Tables[0].Rows[6][1] = rfInputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn2.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf1InputField.text).ToString());
            Debug.Log(filename);
            if (!filename.Equals(""))
            {
                tf1InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[6][2] = tf1InputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn3.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseExcel(new StringBuilder(tf2InputField.text).ToString());
            Debug.Log(filename);
            if (!filename.Equals(""))
            {
                tf2InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[6][3] = tf2InputField.text;
                FilePathMgr.savedata();
            }
        });
        chooseToggle.onValueChanged.AddListener(delegate (bool ison) {
            fileBrowseBtn3.interactable = ison;
            tf2InputField.interactable = ison;
            if (!ison)
            {
                tf2InputField.text = "";
                FilePathMgr.allPathData.Tables[0].Rows[6][3] = tf2InputField.text;
                FilePathMgr.savedata();
            }
        });

        backButton.onClick.AddListener(OnBackBtnClick);
        BtnHowToUse.onClick.AddListener(delegate ()
        {
            string tipstr = "1.源文件：指需要同步的文件\n" +
            "2.目标文件：指被同步的文件\n（将源文件中的东西同步到目标文件中）" +
            "3.勾选框：勾选之后可以指定目标Excel表格中的key才会被同步到目标文件\n" +
            "4.浏览按钮：点击后进行对应文件浏览\n" +
            "5.KeyExcel格式：只有第一列，写入需要对比的key\n" +
            "6.对比结果：对比结果存储在MultLanguageMaintenanceTool\\CompareLogFolder中，其中Excel为对应时间生成的日志，i18n为对比后进行修改之后的结果，将其复制到对应的文件夹中即可";
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, tipstr,null);
        });
        CompareButton.onClick.AddListener(delegate () {
            thread = new Thread(new ThreadStart(delegate () {
                OnCompareBtnClick();
                tipDisplay = false;
            })); thread.Start();
        }/*OnSyncBtnClick*/);
    }

    // Start is called before the first frame update
    void Start()
    {
        rfInputField.text = FilePathMgr.allPathData.Tables[0].Rows[6][1].ToString();
        tf1InputField.text = FilePathMgr.allPathData.Tables[0].Rows[6][2].ToString();
        if(!FilePathMgr.allPathData.Tables[0].Rows[6][3].ToString().Equals(""))
        {
            tf2InputField.text = FilePathMgr.allPathData.Tables[0].Rows[6][3].ToString();
            chooseToggle.isOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        tipPanel.SetActive(tipDisplay);
        if (!tip.Equals("")) tipPanel.transform.Find("Text").GetComponent<Text>().text = tip;
    }

    private void OnDestroy()
    {
        
    }

    private void OnBackBtnClick()
    {
        tipDisplay = false;
        tipPanel.transform.Find("Text").GetComponent<Text>().text = "日志生成中请稍后";
        resText.text = "暂时没有结果";
        EventMgr.Broadcast(EventType.PanelBack);
    }

    private void OnCompareBtnClick()
    {
        if (rfInputField.text.Trim().Equals(""))
        {
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, "源文件为空！！",null);
            return;
        }
        if (tf1InputField.text.Trim().Equals(""))
        {
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, "目标文件为空！！",null);
            return;
        }
        HashSet<string> keyset = new HashSet<string>();
        if (chooseToggle.isOn&&tf2InputField.text.Trim().Equals(""))
        {
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, "KeyExcel文件路径为空！！",null);
            return;
        }
        else
        {
            DataSet dataSet = ExcelHandler.ReadByNPOI(tf2InputField.text.Trim());
            if (dataSet == null)
            {
                EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, "Excel为空！！",null);
                return;
            }
            for (int i = 1; i < dataSet.Tables[0].Rows.Count; i++)
            {
                if (keyset.Contains(dataSet.Tables[0].Rows[i][0].ToString())) continue;
                Debug.Log(dataSet.Tables[0].Rows[i][0].ToString());
                keyset.Add(dataSet.Tables[0].Rows[i][0].ToString());
            }
        }
        tipDisplay = true;
        //读取文件并进行对应的xml文件修改
        string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
        string logPath = FilePathMgr.compareLogFolder + "\\" + data + ".xlsx";
        DataSet dataSet_1 = new DataSet();
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("row");
        dataTable.Columns.Add("key");
        dataTable.Columns.Add("art");
        dataTable.Columns.Add("value");
        DataRow dataRow = dataTable.NewRow();
        dataRow[0] = "title";
        dataTable.Rows.Add(dataRow);
        dataTable.Rows[0][0] = "行数";
        dataTable.Rows[0][1] = "key";
        dataTable.Rows[0][2] = "属性名";
        dataTable.Rows[0][3] = "结果";
        dataSet_1.Tables.Add(dataTable);
        //获取两个xml的数据
        Dictionary<string, Dictionary<string, string>> _targetxml_1 =
            new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Dictionary<string, string>> _targetxml_2 =
            new Dictionary<string, Dictionary<string, string>>();
       _targetxml_1 = XmlHandler.XmlReadByText(rfInputField.text);//xml数据1
       _targetxml_2 = XmlHandler.XmlReadByText(tf1InputField.text);//xml数据1
        int tableRowNum = 1;
        int keyindex = 2;

        Debug.Log("开始对比");
        foreach (string key in _targetxml_1.Keys)
        {
            //Debug.Log(key);
            if (chooseToggle.isOn && !keyset.Contains(key)) continue;
            if(_targetxml_2.ContainsKey(key))
            {
                foreach(string art in _targetxml_1[key].Keys)
                {
                    if(_targetxml_2[key].ContainsKey(art)&&_targetxml_2[key][art].Equals(""))
                    {
                        if (!_targetxml_2[key][art].Equals(_targetxml_1[key][art]))
                        {
                            dataSet_1.Tables[0].Rows.Add(tableRowNum);
                            dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex;
                            dataSet_1.Tables[0].Rows[tableRowNum][1] = key;
                            dataSet_1.Tables[0].Rows[tableRowNum][2] = art;
                            dataSet_1.Tables[0].Rows[tableRowNum][3] = "<" + _targetxml_2[key][art] + ">已修改为<" + _targetxml_1[key][art] + ">";
                            tableRowNum++;
                            _targetxml_2[key][art] = _targetxml_1[key][art];
                        }
                       
                    }
                }
            }
            else
            {
                _targetxml_2[key] = _targetxml_1[key];
                dataSet_1.Tables[0].Rows.Add(tableRowNum);
                dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex;
                dataSet_1.Tables[0].Rows[tableRowNum][1] = key;
                dataSet_1.Tables[0].Rows[tableRowNum][2] = "all";
                dataSet_1.Tables[0].Rows[tableRowNum][3] = "插入成功";
                tableRowNum++;
            }
            keyindex++;
        }


        Debug.Log("对比结束");
        tip = "表格创建中请稍后...";
        ExcelHandler.CreateExcel(dataSet_1, logPath);
        string tipstr = "同步结束\n\n";
        if (_targetxml_1.Count > 0)
        {
            XmlHandler.TextSerialization(_targetxml_2, FilePathMgr.compareLogFolder + "\\i18n.xml");
            tipstr += tf1InputField.text + "同步已完成\n";
        }
        tipstr += "日志已保存到：" + logPath + "\n";
        EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, tipstr, FilePathMgr.compareLogFolder);
        tipDisplay = false;
        Thread.Sleep(100);
        thread.Abort();
    }
}

public class strCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        string str1 = XmlHandler.XmlString(x);
        string str2 = XmlHandler.XmlString(y);
        return string.CompareOrdinal(str1, str2);
    }
}
