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

public class XmlSyncPanelMgr : MonoBehaviour
{

    private Button backButton;
    private Button BtnHowToUse;
    private Button syncBtn;
    //文件路径输入框
    private InputField rfInputField;
    private InputField tf1InputField;
    private InputField tf2InputField;
    //文件浏览按钮
    private Button fileBrowseBtn1;
    private Button fileBrowseBtn2;
    private Button fileBrowseBtn3;
    //选择框
    private Toggle chooseToggle1;
    private Toggle chooseToggle2;

    private Text resText;
    private GameObject tipPanel;
    private bool tipDisplay = false;
    private string tip = "";

    private Dictionary<string, int> keyDic1 = new Dictionary<string, int>();
    private Dictionary<string, int> keyDic2 = new Dictionary<string, int>();
    private Dictionary<string, int> langueDic = new Dictionary<string, int>();
    Thread thread;

    private void Awake()
    {
        backButton = this.transform.Find("BackButton").GetComponent<Button>();
        BtnHowToUse = this.transform.Find("BtnHowToUse").GetComponent<Button>();
        syncBtn = this.transform.Find("SyncButton").GetComponent<Button>();
        rfInputField = this.transform.Find("RFilePathInputField").GetComponent<InputField>();
        tf1InputField = this.transform.Find("T1FilePathInputField").GetComponent<InputField>();
        tf2InputField = this.transform.Find("T2FilePathInputField").GetComponent<InputField>();

        fileBrowseBtn1 = rfInputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn2 = tf1InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn3 = tf2InputField.transform.Find("FileBrowseButton").GetComponent<Button>();

        chooseToggle1 = tf1InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();
        chooseToggle2 = tf2InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();

        resText = this.transform.Find("ResultTextBackground").Find("ResultText").GetComponent<Text>();

        tipPanel = this.transform.Find("TipPanel").gameObject;

        fileBrowseBtn1.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(rfInputField.text).ToString());
            if (!filename.Equals(""))
            {
                rfInputField.text = filename;
            }
        });
        fileBrowseBtn2.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf1InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf1InputField.text = filename;
            }
        });
        fileBrowseBtn3.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf2InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf2InputField.text = filename;
            }
        });
        backButton.onClick.AddListener(OnBackBtnClick);
        BtnHowToUse.onClick.AddListener(delegate ()
        {
            string tipstr = "1.源文件：修改了的文件，会自动从版本同步页面到达此页。\n" +
            "2.两个目标文件：勾选后表示会将源文件的对应版本修改同步于此文件。\n" +
            "3.结果框：结果显示于此处。\n" +
            "4.开始同步：点击后将会将源文件修改同步到选择的目标文件中。\n" +
            "5.同步日志：同版本同步工具。\n";
            EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, tipstr, null);
        });
        syncBtn.onClick.AddListener(delegate() {thread = new Thread(new ThreadStart(delegate() {
            OnSyncBtnClick();
            tipDisplay = false;
        }));thread.Start();}/*OnSyncBtnClick*/);
        EventMgr.AddListener(EventType.VersionToXmlSync, VersionToXmlSync);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tipPanel.SetActive(tipDisplay);
        if (!tip.Equals("")) tipPanel.transform.Find("Text").GetComponent<Text>().text = tip;
    }

    private void OnDestroy()
    {
        EventMgr.RemoveListener(EventType.VersionToXmlSync, VersionToXmlSync);
    }

    private void OnBackBtnClick()
    {
        tipDisplay = false;
        tipPanel.transform.Find("Text").GetComponent<Text>().text = "日志生成中请稍后";
        resText.text = "暂时没有结果";
        EventMgr.Broadcast(EventType.PanelBack);
    }

    private void VersionToXmlSync()
    {
        rfInputField.text = VersionSyncPanel.syncFilePath;
        tf1InputField.text = VersionSyncPanel.tf1FilePath;
        tf2InputField.text = VersionSyncPanel.tf2FilePath;
    }
    private void OnSyncBtnClick()
    {
        tipDisplay = true;
        //读取文件并进行对应的xml文件修改
        DataSet dataSet = ExcelHandler.ReadByNPOI(FilePathMgr.xyncexcelfilepath + "\\" + VersionSyncPanel.logFileName + ".xlsx");
        if (dataSet == null) return;
        string updatapath = FilePathMgr.xmlupdatafilepath + "\\" + VersionSyncPanel.logFileName + ".xlsx";
        DataSet dataSet_1 = new DataSet();
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("row");
        dataTable.Columns.Add("key");
        dataTable.Columns.Add("value");
        dataTable.Columns.Add("filepath");
        DataRow dataRow = dataTable.NewRow();
        dataRow[0] = "title";
        dataTable.Rows.Add(dataRow);
        dataTable.Rows[0][0] = "行数";
        dataTable.Rows[0][1] = "属性名";
        dataTable.Rows[0][2] = "结果";
        dataTable.Rows[0][3] = "文件位置";
        dataSet_1.Tables.Add(dataTable);
        //获取两个xml的数据
        Dictionary<string, Dictionary<string, string>> _targetxml_1 = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Dictionary<string, string>> _targetxml_2 = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, int> keydic1 = new Dictionary<string, int>();
        Dictionary<string, int> keydic2 = new Dictionary<string, int>();
        if (chooseToggle1.isOn && !tf1InputField.text.Trim().Equals("")) _targetxml_1 = XmlHandler.XmlReadByText(tf1InputField.text);//xml数据1
        if (chooseToggle2.isOn && !tf2InputField.text.Trim().Equals("")) _targetxml_2 = XmlHandler.XmlReadByText(tf2InputField.text);//xml数据2

        if(_targetxml_1.Count>0)
        {
            int i = 1;
            foreach(string key in _targetxml_1.Keys)
            {
                keydic1.Add(key, i);
                i++;
            }
        }
        if (_targetxml_2.Count > 0)
        {
            int i = 1;
            foreach (string key in _targetxml_2.Keys)
            {
                keydic2.Add(key, i);
                i++;
            }
        }
        //对于dataSet里面的数据
        int columnNum = dataSet.Tables[0].Columns.Count;
        int rowNum = dataSet.Tables[0].Rows.Count;
        int tableRowNum = 1;
        for (int i = 1; i < rowNum; i++)
        {
            string key = dataSet.Tables[0].Rows[i][0].ToString();
            Debug.Log(dataSet.Tables[0].Rows[i][1]);
            int result = Convert.ToInt32(dataSet.Tables[0].Rows[i][1]);
            switch (result)
            {
                case 1:
                    {
                        //修改后的值，对应的j为第art+1个元素
                        //查找对应key的位置
                        for (int j = 2; j < columnNum; j++)
                        {
                            string value = dataSet.Tables[0].Rows[i][j].ToString();
                            Debug.Log(value);
                            string art = dataSet.Tables[0].Rows[0][j].ToString();
                            if (!value.Equals(""))
                            {
                                if (_targetxml_1.ContainsKey(key))
                                {
                                    dataSet_1.Tables[0].Rows.Add(tableRowNum);
                                    dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic1[key] + 2;
                                    dataSet_1.Tables[0].Rows[tableRowNum][1] = art;
                                    dataSet_1.Tables[0].Rows[tableRowNum][2] = _targetxml_1[key][art] + "已修改为" + value.Split('>')[1];
                                    dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
                                    tableRowNum++;
                                    _targetxml_1[key][art] = value.Split('>')[1];
                                }
                                if (_targetxml_2.ContainsKey(key))
                                {
                                    dataSet_1.Tables[0].Rows.Add(tableRowNum);
                                    dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic2[key] + 2;
                                    dataSet_1.Tables[0].Rows[tableRowNum][1] = art;
                                    dataSet_1.Tables[0].Rows[tableRowNum][2] = _targetxml_2[key][art] + "已修改为" + value.Split('>')[1];
                                    dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
                                    tableRowNum++;
                                    _targetxml_2[key][art] = value.Split('>')[1];
                                }
                            }
                        }
                    }
                    break;
                case 2://删除项
                    {
                        if (_targetxml_1.ContainsKey(key))
                        {
                            _targetxml_1.Remove(key);
                            dataSet_1.Tables[0].Rows.Add(tableRowNum);
                            dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic1[key] + 2;
                            dataSet_1.Tables[0].Rows[tableRowNum][1] = key;
                            dataSet_1.Tables[0].Rows[tableRowNum][2] = "删除";
                            dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
                            tableRowNum++;
                            //root_1.RemoveChild(root_1.ChildNodes[keyindex_1]);
                        }
                        if (_targetxml_2.ContainsKey(key))
                        {
                            _targetxml_2.Remove(key);
                            dataSet_1.Tables[0].Rows.Add(tableRowNum);
                            dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic2[key] + 2;
                            dataSet_1.Tables[0].Rows[tableRowNum][1] = key;
                            dataSet_1.Tables[0].Rows[tableRowNum][2] = "删除";
                            dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
                            tableRowNum++;
                            //root_1.RemoveChild(root_1.ChildNodes[keyindex_1]);
                        }
                    }
                    break;
                case 3://增加项
                    {
                        if (_targetxml_1.ContainsKey(key))
                        {
                            //如果找到了，那么就要修改该项对应值
                            for (int j = 2; j < columnNum; j++)
                            {
                                string art = dataSet.Tables[0].Rows[0][j].ToString();
                                dataSet_1.Tables[0].Rows.Add(tableRowNum);
                                dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic1[key] + 2;
                                dataSet_1.Tables[0].Rows[tableRowNum][1] = art;
                                dataSet_1.Tables[0].Rows[tableRowNum][2] = _targetxml_1[key][art] + "已修改为" + dataSet.Tables[0].Rows[i][j].ToString().Split('>')[1];
                                dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
                                tableRowNum++;
                                _targetxml_1[key][art] = dataSet.Tables[0].Rows[i][j].ToString().Split('>')[1];
                            }
                        }
                        else
                        {
                            Dictionary<string, string> newNode = new Dictionary<string, string>();
                            newNode.Add("key", key);
                            for (int j = 2; j < columnNum; j++)
                            {
                                string art = dataSet.Tables[0].Rows[0][j].ToString();
                                string artvalue = dataSet.Tables[0].Rows[i][j].ToString().Split('>')[1];
                                dataSet_1.Tables[0].Rows.Add(tableRowNum);
                                dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic1[key] + 2;
                                dataSet_1.Tables[0].Rows[tableRowNum][1] = art;
                                dataSet_1.Tables[0].Rows[tableRowNum][2] = "修改值为" + artvalue;
                                dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
                                tableRowNum++;
                                newNode.Add(art, artvalue);
                            }
                            _targetxml_1.Add(key, newNode);
                            keydic1.Add(key, keydic1.Count + 1);
                        }
                        if (_targetxml_2.ContainsKey(key))
                        {
                            //如果找到了，那么就要修改该项对应值
                            for (int j = 2; j < columnNum; j++)
                            {
                                string art = dataSet.Tables[0].Rows[0][j].ToString();
                                dataSet_1.Tables[0].Rows.Add(tableRowNum);
                                dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic2[key] + 2;
                                dataSet_1.Tables[0].Rows[tableRowNum][1] = art;
                                dataSet_1.Tables[0].Rows[tableRowNum][2] = _targetxml_2[key][art] + "已修改为" + dataSet.Tables[0].Rows[i][j].ToString().Split('>')[1];
                                dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
                                tableRowNum++;
                                _targetxml_2[key][art] = dataSet.Tables[0].Rows[i][j].ToString().Split('>')[1];
                            }
                        }
                        else
                        {
                            Dictionary<string, string> newNode = new Dictionary<string, string>();
                            newNode.Add("key", key);
                            for (int j = 2; j < columnNum; j++)
                            {
                                string art = dataSet.Tables[0].Rows[0][j].ToString();
                                string artvalue = dataSet.Tables[0].Rows[i][j].ToString().Split('>')[1];
                                dataSet_1.Tables[0].Rows.Add(tableRowNum);
                                dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic2[key] + 2;
                                dataSet_1.Tables[0].Rows[tableRowNum][1] = art;
                                dataSet_1.Tables[0].Rows[tableRowNum][2] = "修改值为" + artvalue;
                                dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
                                tableRowNum++;
                                newNode.Add(art, artvalue);
                            }
                            _targetxml_2.Add(key, newNode);
                            keydic2.Add(key, keydic2.Count + 1);
                        }
                    }
                    break;
                case 4://key更改项
                    {
                        string oldkey = key.Split('>')[0].Split('<')[1];
                        string newkey = key.Split('>')[1];
                        if (_targetxml_1.ContainsKey(key))
                        {
                            _targetxml_1[key]["key"] = newkey;
                            dataSet_1.Tables[0].Rows.Add(tableRowNum);
                            dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic1[key]+2;
                            dataSet_1.Tables[0].Rows[tableRowNum][1] = "key";
                            dataSet_1.Tables[0].Rows[tableRowNum][2] = oldkey + "已修改为" + newkey;
                            dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
                            tableRowNum++;
                        }
                        if (_targetxml_2.ContainsKey(key))
                        {
                            _targetxml_2[key]["key"] = newkey;
                            dataSet_1.Tables[0].Rows.Add(tableRowNum);
                            dataSet_1.Tables[0].Rows[tableRowNum][0] = keydic2[key] + 2;
                            dataSet_1.Tables[0].Rows[tableRowNum][1] = "key";
                            dataSet_1.Tables[0].Rows[tableRowNum][2] = oldkey + "已修改为" + newkey;
                            dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
                            tableRowNum++;
                        }
                    }
                    break;
                default:
                    Debug.LogError("错误的日志类型，是否修改过表格！！");
                    break;
            }

        }

        Debug.Log("同步结束");
        tip = "表格创建中请稍后...";
        ExcelHandler.CreateExcel(dataSet_1, updatapath);
        string tipstr = "同步结束\n\n";
        if (_targetxml_1.Count>0)
        {
            XmlHandler.TextSerialization(_targetxml_1, tf1InputField.text);
            tipstr += tf1InputField.text + "同步已完成\n";
        }
        if (_targetxml_2.Count > 0)
        {
            XmlHandler.TextSerialization(_targetxml_2, tf2InputField.text);
            tipstr += tf2InputField.text + "同步已完成\n";
        }
        tipstr +=  "日志已保存到：" + updatapath + "\n";
        EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, tipstr, FilePathMgr.xmlupdatafilepath);
        tipDisplay = false;
        Thread.Sleep(100);
        thread.Abort();
    }



    //通过表格文件进行对应的xml修改
    //private void OnSyncBtnClick()
    //{
    //    tipDisplay = true;
    //    //读取文件并进行对应的xml文件修改
    //    DataSet dataSet = ExcelHandler.ReadByNPOI(FilePathMgr.xyncexcelfilepath + "\\" + logFileName + ".xlsx");
    //    if (dataSet == null) return;
    //    string updatapath = FilePathMgr.xmlupdatafilepath + "\\" + logFileName + ".xlsx";
    //    DataSet dataSet_1 = new DataSet();
    //    DataTable dataTable = new DataTable();
    //    dataTable.Columns.Add("row");
    //    dataTable.Columns.Add("key");
    //    dataTable.Columns.Add("value");
    //    dataTable.Columns.Add("filepath");
    //    DataRow dataRow = dataTable.NewRow();
    //    dataRow[0] = "title";
    //    dataTable.Rows.Add(dataRow);
    //    dataTable.Rows[0][0] = "行数";
    //    dataTable.Rows[0][1] = "属性名";
    //    dataTable.Rows[0][2] = "结果";
    //    dataTable.Rows[0][3] = "文件位置";
    //    dataSet_1.Tables.Add(dataTable);
    //    //获取两个xml的数据
    //    XmlDocument _targetxml_1 = new XmlDocument();
    //    XmlDocument _targetxml_2 = new XmlDocument();
    //    XmlNode root_1 = null;
    //    XmlNode root_2 = null;
    //    //xml数据1
    //    if (chooseToggle1.isOn) { _targetxml_1 = XmlHandler.XmlReader2(tf1InputField.text); root_1 = _targetxml_1.SelectSingleNode("root"); }
    //    //xml数据2
    //    if (chooseToggle2.isOn) {_targetxml_2 = XmlHandler.XmlReader2(tf2InputField.text); root_2 = _targetxml_2.SelectSingleNode("root"); }

    //    //对于dataSet里面的数据
    //     int columnNum = dataSet.Tables[0].Columns.Count;
    //    int rowNum = dataSet.Tables[0].Rows.Count;
    //    int tableRowNum = 1;
    //    for (int i = 1; i < rowNum; i++)
    //    {
    //        string key = dataSet.Tables[0].Rows[i][0].ToString();
    //        Debug.Log(dataSet.Tables[0].Rows[i][1]);
    //        int result = Convert.ToInt32(dataSet.Tables[0].Rows[i][1]);
    //        switch (result)
    //        {
    //            case 1:
    //                {
    //                    //修改后的值，对应的j为第art+1个元素
    //                    //查找对应key的位置
    //                    int keyindex_1 = XmlHandler.XmlKeySearch(root_1, key);
    //                    int keyindex_2 = XmlHandler.XmlKeySearch(root_2, key);
    //                    for (int j = 2; j < columnNum; j++)
    //                    {
    //                        string value = System.Security.SecurityElement.Escape(dataSet.Tables[0].Rows[i][j].ToString());
    //                        if (!value.Equals(""))
    //                        {

    //                            if (keyindex_1 >= 0)
    //                            {
    //                                dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                                dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_1 + 3;
    //                                dataSet_1.Tables[0].Rows[tableRowNum][1] = root_1.ChildNodes[keyindex_1].Attributes[j - 1].Name;
    //                                dataSet_1.Tables[0].Rows[tableRowNum][2] = root_1.ChildNodes[keyindex_1].Attributes[j - 1].Value+ "已修改为" + value.Split(':')[1];
    //                                dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
    //                                tableRowNum++;
    //                                root_1.ChildNodes[keyindex_1].Attributes[j - 1].Value = value.Split(':')[1];
    //                            }
    //                            if (keyindex_2 >= 0)
    //                            {
    //                                dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                                dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_2 + 3;
    //                                dataSet_1.Tables[0].Rows[tableRowNum][1] = root_2.ChildNodes[keyindex_2].Attributes[j - 1].Name;
    //                                dataSet_1.Tables[0].Rows[tableRowNum][2] = root_2.ChildNodes[keyindex_2].Attributes[j - 1].Value + "已修改为" + value.Split(':')[1];
    //                                dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
    //                                tableRowNum++;
    //                                root_2.ChildNodes[keyindex_2].Attributes[j - 1].Value = value.Split(':')[1];
    //                            }
    //                        }
    //                    }
    //                }
    //                break;
    //            case 2://删除项
    //                {
    //                    Debug.Log("开始查找");
    //                    //查找是否有该项
    //                    int keyindex_1 = XmlHandler.XmlKeySearch(root_1, key);
    //                    //Debug.Log(keyindex_1);
    //                    int keyindex_2 = XmlHandler.XmlKeySearch(root_2, key);
    //                    if (keyindex_1 >= 0)
    //                    {
    //                        XmlNodeList nodeList = root_1.ChildNodes;
    //                        nodeList[keyindex_1].ParentNode.RemoveChild(nodeList[keyindex_1]);
    //                        dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                        dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_1 + 3;
    //                        dataSet_1.Tables[0].Rows[tableRowNum][1] = key;
    //                        dataSet_1.Tables[0].Rows[tableRowNum][2] = "删除";
    //                        dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
    //                        tableRowNum++;
    //                        //root_1.RemoveChild(root_1.ChildNodes[keyindex_1]);
    //                    }
    //                    if (keyindex_2 >= 0)
    //                    {
    //                        XmlNodeList nodeList = root_2.ChildNodes;
    //                        nodeList[keyindex_2].ParentNode.RemoveChild(nodeList[keyindex_2]);
    //                        dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                        dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_2 + 3;
    //                        dataSet_1.Tables[0].Rows[tableRowNum][1] = key;
    //                        dataSet_1.Tables[0].Rows[tableRowNum][2] = "删除";
    //                        dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
    //                        tableRowNum++;
    //                        //root_2.RemoveChild(root_2.ChildNodes[keyindex_2]);
    //                    }
    //                }
    //                break;
    //            case 3://增加项
    //                {
    //                    //查找是否有该项
    //                    int keyindex_1 = XmlHandler.XmlKeySearch(root_1, key);
    //                    int keyindex_2 = XmlHandler.XmlKeySearch(root_2, key);
    //                    if (keyindex_1 >= 0)
    //                    {
    //                        //如果找到了，那么就要修改该项对应值
    //                        for (int j = 2; j < columnNum; j++)
    //                        {
    //                            dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                            dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_1 + 3;
    //                            dataSet_1.Tables[0].Rows[tableRowNum][1] = root_1.ChildNodes[keyindex_1].Attributes[j - 1].Name;
    //                            dataSet_1.Tables[0].Rows[tableRowNum][2] = root_1.ChildNodes[keyindex_1].Attributes[j - 1].Value + "已修改为" + dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1];
    //                            dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
    //                            tableRowNum++;
    //                            root_1.ChildNodes[keyindex_1].Attributes[j - 1].Value = System.Security.SecurityElement.Escape(dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1]);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        keyindex_1 = XmlHandler.XmlKeyInsertIndexSearch(root_1, key);
    //                        if (keyindex_1 >= 0)
    //                        //否则
    //                        {
    //                            if (string.CompareOrdinal(root_1.ChildNodes[keyindex_1].Attributes[0].Value, key) < 0)
    //                            {
    //                                XmlElement newTans = _targetxml_1.CreateElement("trans");
    //                                newTans.SetAttribute("key", System.Security.SecurityElement.Escape(dataSet.Tables[0].Rows[i][0].ToString()));
    //                                //如果找到了，那么就要修改该项对应值
    //                                for (int j = 2; j < columnNum; j++)
    //                                {
    //                                    newTans.SetAttribute(dataSet.Tables[0].Rows[0][j].ToString(), System.Security.SecurityElement.Escape(dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1]));
    //                                    dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_1 + 4;
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][1] = dataSet.Tables[0].Rows[0][j].ToString();
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][2] = "修改值为"+ dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1];
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
    //                                    tableRowNum++;
    //                                }
    //                                //XmlNodeList nodeList = root_1.ChildNodes;
    //                                //nodeList[keyindex_1].InsertAfter(newTans,nodeList[keyindex_1]);
    //                                root_1.InsertAfter(newTans, root_1.ChildNodes[keyindex_1]);
    //                            }
    //                            else
    //                            {
    //                                XmlElement newTans = _targetxml_1.CreateElement("trans");
    //                                newTans.SetAttribute("key", dataSet.Tables[0].Rows[i][0].ToString());
    //                                //如果找到了，那么就要修改该项对应值
    //                                for (int j = 2; j < columnNum; j++)
    //                                {
    //                                    newTans.SetAttribute(dataSet.Tables[0].Rows[0][j].ToString(), System.Security.SecurityElement.Escape(dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1]));
    //                                    dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_1 + 3;
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][1] = dataSet.Tables[0].Rows[0][j].ToString();
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][2] = "修改值为" + dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1];
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
    //                                    tableRowNum++;
    //                                }
    //                                //XmlNodeList nodeList = root_1.ChildNodes;
    //                                //nodeList[keyindex_1].InsertBefore(newTans, nodeList[keyindex_1]);
    //                                root_1.InsertBefore(newTans, root_1.ChildNodes[keyindex_1]);
    //                            }
    //                        }
    //                    }
    //                    if (keyindex_2 >= 0)
    //                    {
    //                        for (int j = 2; j < columnNum; j++)
    //                        {
    //                            dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                            dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_2 + 3;
    //                            dataSet_1.Tables[0].Rows[tableRowNum][1] = root_2.ChildNodes[keyindex_2].Attributes[j - 1].Name;
    //                            dataSet_1.Tables[0].Rows[tableRowNum][2] = root_2.ChildNodes[keyindex_2].Attributes[j - 1].Value + "已修改为" + dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1];
    //                            dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
    //                            tableRowNum++;
    //                            root_2.ChildNodes[keyindex_2].Attributes[j - 1].Value = System.Security.SecurityElement.Escape(dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1]);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        keyindex_2 = XmlHandler.XmlKeyInsertIndexSearch(root_2, key);
    //                        if (keyindex_2 >= 0)
    //                        {
    //                            //否则
    //                            if (string.CompareOrdinal(root_2.ChildNodes[keyindex_2].Attributes[0].Value, key) < 0)
    //                            {
    //                                XmlElement newTans = _targetxml_1.CreateElement("trans");
    //                                newTans.SetAttribute("key", dataSet.Tables[0].Rows[i][0].ToString());
    //                                //如果找到了，那么就要修改该项对应值
    //                                for (int j = 2; j < columnNum; j++)
    //                                {
    //                                    newTans.SetAttribute(dataSet.Tables[0].Rows[0][j].ToString(), System.Security.SecurityElement.Escape(dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1]));
    //                                    dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_2 + 4;
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][1] = dataSet.Tables[0].Rows[0][j].ToString();
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][2] = "修改值为" + dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1];
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
    //                                    tableRowNum++;
    //                                }
    //                                //XmlNodeList nodeList = root_2.ChildNodes;
    //                                //nodeList[keyindex_2].ParentNode.InsertAfter(newTans, nodeList[keyindex_2]);
    //                                root_2.InsertAfter(newTans, root_2.ChildNodes[keyindex_2]);
    //                            }
    //                            else
    //                            {
    //                                XmlElement newTans = _targetxml_1.CreateElement("trans");
    //                                newTans.SetAttribute("key", dataSet.Tables[0].Rows[i][0].ToString());
    //                                //如果找到了，那么就要修改该项对应值
    //                                for (int j = 2; j < columnNum; j++)
    //                                {
    //                                    newTans.SetAttribute(dataSet.Tables[0].Rows[0][j].ToString(), System.Security.SecurityElement.Escape(dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1]));
    //                                    dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_2 + 3;
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][1] = dataSet.Tables[0].Rows[0][j].ToString();
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][2] = "修改值为" + dataSet.Tables[0].Rows[i][j].ToString().Split(':')[1];
    //                                    dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
    //                                    tableRowNum++;
    //                                }
    //                                //XmlNodeList nodeList = root_2.ChildNodes;
    //                                //nodeList[keyindex_2].ParentNode.InsertBefore(newTans, nodeList[keyindex_2]);
    //                                root_2.InsertBefore(newTans, root_2.ChildNodes[keyindex_2]);
    //                            }
    //                        }
    //                    }
    //                }
    //                break;
    //            case 4://key更改项
    //                {
    //                    string oldkey = System.Security.SecurityElement.Escape(key.Split(':')[0]);
    //                    string newkey = System.Security.SecurityElement.Escape(key.Split(':')[1]);
    //                    int keyindex_1 = XmlHandler.XmlKeySearch(root_1, oldkey);
    //                    int keyindex_2 = XmlHandler.XmlKeySearch(root_2, oldkey);
    //                    if (keyindex_1 >= 0)
    //                    {
    //                        root_1.ChildNodes[keyindex_1].Attributes[0].Value = newkey;
    //                        dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                        dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_1 + 3;
    //                        dataSet_1.Tables[0].Rows[tableRowNum][1] = "key";
    //                        dataSet_1.Tables[0].Rows[tableRowNum][2] = oldkey + "已修改为" + newkey;
    //                        dataSet_1.Tables[0].Rows[tableRowNum][3] = tf1InputField.text;
    //                        tableRowNum++;
    //                    }
    //                    if (keyindex_2 >= 0)
    //                    {
    //                        root_2.ChildNodes[keyindex_2].Attributes[0].Value = newkey;
    //                        dataSet_1.Tables[0].Rows.Add(tableRowNum);
    //                        dataSet_1.Tables[0].Rows[tableRowNum][0] = keyindex_2 + 3;
    //                        dataSet_1.Tables[0].Rows[tableRowNum][1] = "key";
    //                        dataSet_1.Tables[0].Rows[tableRowNum][2] = oldkey + "已修改为" + newkey;
    //                        dataSet_1.Tables[0].Rows[tableRowNum][3] = tf2InputField.text;
    //                        tableRowNum++;
    //                    }
    //                }
    //                break;
    //            default:
    //                Debug.LogError("错误的日志类型，是否修改过表格！！");
    //                break;
    //        }

    //    }

    //    Debug.Log("同步结束");
    //    ExcelHandler.CreateExcel(dataSet_1, updatapath);
    //    if (root_1 != null)
    //    {
    //        XmlHandler.XmlSave(_targetxml_1, tf1InputField.text);
    //    }
    //    if (root_2 != null)
    //    {
    //        XmlHandler.XmlSave(_targetxml_2, tf2InputField.text);
    //    }
    //    tipDisplay = false;
    //    Thread.Sleep(100);
    //    thread.Abort();
    //}

    //必须源文件是key有多的文件
    //若源文件中的key被删除，将会造成之后的所有的都无法继续匹配
    //private void OnSyncBtnClick()
    //{
    //    keyDic1.Clear();
    //    keyDic2.Clear();
    //    langueDic.Clear();
    //    string logFilePath = rfInputField.text.Split('.')[0] + "日志报告.xlsx";
    //    XmlDocument rfxml = XmlHandler.XmlReader2(rfInputField.text);
    //    XmlNode root = rfxml.SelectSingleNode("root");
    //    XmlDocument _targetxml_1 = new XmlDocument();
    //    XmlDocument _targetxml_2 = new XmlDocument();
    //    if (chooseToggle1.isOn) _targetxml_1 = XmlHandler.XmlReader2(tf1InputField.text);//xml数据1
    //    if (chooseToggle2.isOn) _targetxml_2 = XmlHandler.XmlReader2(tf2InputField.text);//xml数据2
    //    XmlNode root_1 = _targetxml_1.SelectSingleNode("root");
    //    XmlNode root_2 = _targetxml_2.SelectSingleNode("root");
    //    int index_1 = 0;
    //    int index_2 = 0;
    //    DataSet dataSet = new DataSet();
    //    DataTable dataTable = new DataTable();

    //    //dataSet.Tables[0].Rows.Add("title");
    //    dataTable.Columns.Add("key");
    //    dataTable.Columns.Add("index");
    //    dataTable.Columns.Add("result");
    //    DataRow dataRow = dataTable.NewRow();
    //    dataRow[0] = "title";
    //    dataTable.Rows.Add(dataRow);
    //    dataSet.Tables.Add(dataTable);
    //    dataTable.Rows[0][0] = "key";
    //    dataTable.Rows[0][1] = "index";
    //    dataTable.Rows[0][2] = "result";
    //    for (int i=1;i<root.ChildNodes[0].Attributes.Count;i++)
    //    {
    //        dataSet.Tables[0].Columns.Add(root.ChildNodes[0].Attributes[i].Name);
    //        dataSet.Tables[0].Rows[0][2+i] = root.ChildNodes[0].Attributes[i].Name;
    //    }
    //    int dataindex = 1;
    //    //表格内容
    //    //key，源文件第几行，处理结果（目标文件1修改成功，目标文件1新增，目标文件1有而源文件没有-》已删除，），对应的语言及其结果如果修改两种结果之间用:隔开
    //    //i18n中的key是严格按照unicode来进行排序的
    //    for (int i=0; root_1 != null && i <root.ChildNodes.Count;)
    //    {
    //        //匹配成功则将index_1加一
    //        int campareValue = string.CompareOrdinal(root.ChildNodes[i].Attributes[0].Value, root_1.ChildNodes[index_1].Attributes[0].Value);
    //        if (campareValue == 0)
    //        {
    //            for (int j = 1; j < root.ChildNodes[0].Attributes.Count; j++)
    //            {
    //                if (root.ChildNodes[i].Attributes[j].Value != root_1.ChildNodes[index_1].Attributes[j].Value)
    //                {
    //                    dataSet.Tables[0].Rows.Add(dataindex);
    //                    dataSet.Tables[0].Rows[dataindex][0] = root.ChildNodes[i].Attributes[0].Value;
    //                    dataSet.Tables[0].Rows[dataindex][1] = "" + (i + 3) + "行";
    //                    dataSet.Tables[0].Rows[dataindex][2] = "目标文件1第" + index_1 + "行修改";
    //                    for (int k = 1; k < root.ChildNodes[0].Attributes.Count; k++)
    //                    {
    //                        dataSet.Tables[0].Rows[dataindex][2 + k] = root.ChildNodes[i].Attributes[k].Value + ":" + root_1.ChildNodes[index_1].Attributes[k].Value;
    //                    }
    //                    dataindex++;
    //                    break;
    //                }
    //            }
    //            index_1++;
    //            i++;
    //        }
    //        //如果此时的源文件的key小于目标文件1的key值，说明index_1不需要增加，可能下一个源文件的key会等于它，那么就是说需要将源文件的key+1,也就是说此时的数据应该插入进目标文件中
    //        //如果此时源文件的key大于目标文件1的key值，那么此时需要将index_1加一，则需要删除此数据
    //        else if (campareValue < 0)
    //        {
    //            dataSet.Tables[0].Rows.Add(dataindex);
    //            dataSet.Tables[0].Rows[dataindex][0] = root.ChildNodes[i].Attributes[0].Value;
    //            dataSet.Tables[0].Rows[dataindex][1] = "" + i + 3 + "行";
    //            dataSet.Tables[0].Rows[dataindex][2] = "目标文件1新增数据";
    //            for (int k = 1; k < root.ChildNodes[0].Attributes.Count; k++)
    //            {
    //                dataSet.Tables[0].Rows[dataindex][2 + k] = root.ChildNodes[i].Attributes[k].Value + ": null";
    //            }
    //            dataindex++;
    //            i++;
    //        }
    //        else if (campareValue > 0) 
    //        {
    //            dataSet.Tables[0].Rows.Add(dataindex);
    //            dataSet.Tables[0].Rows[dataindex][0] = root.ChildNodes[i].Attributes[0].Value;
    //            dataSet.Tables[0].Rows[dataindex][1] = "" + i + 3 + "行";
    //            dataSet.Tables[0].Rows[dataindex][2] = "目标文件1删除数据";
    //            for (int k = 1; k < root.ChildNodes[0].Attributes.Count; k++)
    //            {
    //                dataSet.Tables[0].Rows[dataindex][2 + k] =  "null : " + root_1.ChildNodes[index_1].Attributes[k].Value;
    //            }
    //            dataindex++;
    //            index_1++; 
    //        }
    //    }
    //    for (int i = 0; root_2!=null&& i < root.ChildNodes.Count;)
    //    {
    //        //匹配成功则将index_1加一
    //        int campareValue = string.CompareOrdinal(root.ChildNodes[i].Attributes[0].Value, root_2.ChildNodes[index_2].Attributes[0].Value);
    //        if (campareValue == 0)
    //        {
    //            for (int j = 1; j < root.ChildNodes[0].Attributes.Count; j++)
    //            {
    //                if (root.ChildNodes[i].Attributes[j].Value != root_2.ChildNodes[index_2].Attributes[j].Value)
    //                {
    //                    dataSet.Tables[0].Rows.Add(dataindex);
    //                    dataSet.Tables[0].Rows[dataindex][0] = root.ChildNodes[i].Attributes[0].Value;
    //                    dataSet.Tables[0].Rows[dataindex][1] = "" + i + 3 + "行";
    //                    dataSet.Tables[0].Rows[dataindex][2] = "目标文件2第" + index_2 + "行修改";
    //                    for (int k = 1; k < root.ChildNodes[0].Attributes.Count; k++)
    //                    {
    //                        dataSet.Tables[0].Rows[dataindex][2 + k] = root.ChildNodes[i].Attributes[k].Value + ":" + root_2.ChildNodes[index_2].Attributes[k].Value;
    //                    }
    //                    dataindex++;
    //                    break;
    //                }
    //            }
    //            index_2++;
    //            i++;
    //        }
    //        //如果此时的源文件的key小于目标文件1的key值，说明index_1不需要增加，可能下一个源文件的key会等于它，那么就是说需要将源文件的key+1,也就是说此时的数据应该插入进目标文件中
    //        //如果此时源文件的key大于目标文件1的key值，那么此时需要将index_1加一，则需要删除此数据
    //        else if (campareValue < 0)
    //        {
    //            dataSet.Tables[0].Rows.Add(dataindex);
    //            dataSet.Tables[0].Rows[dataindex][0] = root.ChildNodes[i].Attributes[0].Value;
    //            dataSet.Tables[0].Rows[dataindex][1] = "" + i + 3 + "行";
    //            dataSet.Tables[0].Rows[dataindex][2] = "目标文件2新增数据";
    //            for (int k = 1; k < root.ChildNodes[0].Attributes.Count; k++)
    //            {
    //                dataSet.Tables[0].Rows[dataindex][2 + k] = root.ChildNodes[i].Attributes[k].Value + ": null";
    //            }
    //            dataindex++;
    //            i++;
    //        }
    //        else if (campareValue > 0)
    //        {
    //            dataSet.Tables[0].Rows.Add(dataindex);
    //            dataSet.Tables[0].Rows[dataindex][0] = root.ChildNodes[i].Attributes[0].Value;
    //            dataSet.Tables[0].Rows[dataindex][1] = "" + i + 3 + "行";
    //            dataSet.Tables[0].Rows[dataindex][2] = "目标文件2删除数据";
    //            for (int k = 1; k < root.ChildNodes[0].Attributes.Count; k++)
    //            {
    //                dataSet.Tables[0].Rows[dataindex][2 + k] = "null : " + root_2.ChildNodes[index_2].Attributes[k].Value;
    //            }
    //            dataindex++;
    //            index_2++;
    //        }

    //    }

    //    //匹配完毕新建表
    //    resText.text = "更新日志成功！";
    //    ExcelHandler.CreateExcel(dataSet, logFilePath);

    //    //_dataSet.Tables[0].AcceptChanges();

    //    //ISheet sheet = wk.GetSheetAt(0);
    //    //int col = sheet.GetRow(0).Cells.Count;
    //    //sheet.GetRow(0).CreateCell(col).SetCellValue("处理结果");
    //    //for(int i=0;i<col;i++)
    //    //{
    //    //    sheet.GetRow(0).CreateCell(col + i + 1).SetCellValue(sheet.GetRow(0).GetCell(i).ToString());
    //    //}
    //    //ExcelHandler.CreateExcel(wk,logFilePath);
    //}
}
