using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using JetBrains.Annotations;
using System.Threading;
using System;
using System.Text;

public class ExcelsExportPanel : MonoBehaviour
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

    //private Dictionary<string, int> keyDic1 = new Dictionary<string, int>();
    //private Dictionary<string, int> keyDic2 = new Dictionary<string, int>();
    private Dictionary<string, int> langueDic = new Dictionary<string, int>();
    private Thread thread;
    string tiptext = "";

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
            string filename = OpenFileByWin32.BrowseExcel(new StringBuilder(rfInputField.text).ToString());
            if (!filename.Equals(""))
            {
                rfInputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[2][1] = rfInputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn2.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf1InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf1InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[2][2] = tf1InputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn3.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf2InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf2InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[2][3] = tf2InputField.text;
                FilePathMgr.savedata();
            }
        });
        backButton.onClick.AddListener(delegate () { EventMgr.Broadcast(EventType.PanelBack); });
        BtnHowToUse.onClick.AddListener(delegate ()
        {
            string tipstr = "1.源文件：是放有对应翻译的excel文件，文件格式为第一列为key，第二到第n-1列为对应翻译，第n列为trunk（用于判断是否trunk一致，非带入翻译的trunk）\n" +
            "2.目标文件：excel翻译导入的目标文件。\n" +
            "3.结果框：显示对应的结果。\n" +
            "4.开始同步按钮：点击开始导入Excel。\n" +
            "5.导出日志：日志存储于MultLanguageMaintenanceTool\\ExcelExprotFolder。";
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, tipstr,null);
        });
        syncBtn.onClick.AddListener(delegate () { thread = new Thread(new ThreadStart(delegate() {
            tipDisplay = true;
            OnSyncBtnClick();
            tipDisplay = false;
            Thread.Sleep(100);
            thread.Abort();
        })); thread.Start(); }/*OnSyncBtnClick*/);

    }

    // Start is called before the first frame update
    void Start()
    {
        rfInputField.text = FilePathMgr.allPathData.Tables[0].Rows[2][1].ToString();
        tf1InputField.text = FilePathMgr.allPathData.Tables[0].Rows[2][2].ToString();
        tf2InputField.text = FilePathMgr.allPathData.Tables[0].Rows[2][3].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(tiptext!="")
        {
            resText.text = tiptext;
        }
        tipPanel.SetActive(tipDisplay);
    }
    private void OnSyncBtnClick()
    {
        string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
        langueDic.Clear();
        string logFilePath = FilePathMgr.excelexprotfilepath + "\\" + data + ".xlsx";
        Dictionary<string, Dictionary<string, string>> _targetxml_1 = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Dictionary<string, string>> _targetxml_2 = new Dictionary<string, Dictionary<string, string>>();
        if (chooseToggle1.isOn&& !tf1InputField.text.Trim().Equals("")) _targetxml_1 = XmlHandler.XmlReadByText(tf1InputField.text);//xml数据1
        if (chooseToggle2.isOn&& !tf2InputField.text.Trim().Equals("")) _targetxml_2 = XmlHandler.XmlReadByText(tf2InputField.text);//xml数据2
        //先创建一个对应的日志表再对其进行对应的修改
        DataSet _dataSet = ExcelHandler.ReadByNPOI(rfInputField.text);//表格数据
        int columnNum = _dataSet.Tables[0].Columns.Count;
        if(columnNum<=2)
        {
            tiptext = "表格格式错误！！！\n请修改为key+各种语言的翻译+trunk（或者在最后一列加一个任意的都可以）";
            return;
        }
        int rowNum = _dataSet.Tables[0].Rows.Count;
        bool trunk = false;
        foreach(string xmlkey in _targetxml_1.Keys)
        {
            if(_targetxml_1[xmlkey].ContainsKey("trunk"))
            {
                trunk = true;
            }
            else
            {
                trunk = false;
            }
            break;
        }

        //从第一行第二个开始到倒数第二个为止为其对应的翻译语言
        //ExcelHandler.CreateExcel(_dataSet, logFilePath,langueDic);
        //_dataSet = ExcelHandler.ReadExcelSheet(logFilePath);
        _dataSet.Tables[0].Columns.Add("处理结果");
        _dataSet.Tables[0].Columns.Add("key");
        _dataSet.Tables[0].Columns.Add("trunk");
        _dataSet.Tables[0].Rows[0][columnNum] = "处理结果";
        _dataSet.Tables[0].Rows[0][columnNum + 1] = "key";
        _dataSet.Tables[0].Rows[0][columnNum + 2] = "trunk";
        for (int i = 0; i < columnNum-2; i++)
        {
            _dataSet.Tables[0].Columns.Add(_dataSet.Tables[0].Rows[0][1 + i].ToString() + "翻译");
            _dataSet.Tables[0].Rows[0][columnNum + 3 + i] = _dataSet.Tables[0].Rows[0][1 + i].ToString() + "翻译";
        }
        //key匹配
        int keydisptnum_1 = 0;
        int keydisptnum_2 = 0;
        for (int i = 1; i < rowNum; i++)
        {
            //key匹配成功
            string key = _dataSet.Tables[0].Rows[i][0].ToString();
            if (key.Equals("")) { keydisptnum_1++; keydisptnum_2++; continue; }
            if (_targetxml_1.Count>0)
            {
                if (_targetxml_1.ContainsKey(key))
                {
                    _dataSet.Tables[0].Rows[i][columnNum] = "√";
                    Debug.Log("key匹配成功");
                    //对于对应的列的语言对i18n进行更新
                    for (int j = 1; j <= columnNum-2; j++)
                    {
                        _targetxml_1[key][_dataSet.Tables[0].Rows[0][j].ToString()] = _dataSet.Tables[0].Rows[i][j].ToString();
                    }
                }
                else if(trunk)//key匹配失败
                {
                    keydisptnum_1++;
                    Debug.Log("key匹配失败");
                    //匹配trunk
                    bool flag = false;
                    string oldkey = "";
                    foreach(string xmlkey in _targetxml_1.Keys)
                    {
                        if(string.CompareOrdinal(xmlkey,key)>0)
                        {
                            if (_targetxml_1[oldkey]["trunk"] == _dataSet.Tables[0].Rows[0][columnNum - 1].ToString())
                            {
                                Debug.Log("trunk匹配成功");
                                flag = true;
                                //设置表对应行的属性值
                                _dataSet.Tables[0].Rows[i][columnNum] = "i18n_1有对应trunk，但是无对应key";
                                _dataSet.Tables[0].Rows[i][columnNum + 1] = oldkey;
                                _dataSet.Tables[0].Rows[0][columnNum + 2] = _targetxml_1[oldkey]["trunk"];
                                int k = 0;
                                foreach (string art in _targetxml_1[oldkey].Keys)
                                {
                                    if (!art.Equals("key"))
                                    {
                                        _dataSet.Tables[0].Rows[i][columnNum + 3 + k] = _targetxml_1[oldkey][art];
                                    }
                                    k++;
                                }
                            }
                            else if (_targetxml_1[xmlkey]["trunk"]== _dataSet.Tables[0].Rows[0][columnNum - 1].ToString())
                            {
                                Debug.Log("trunk匹配成功");
                                flag = true;
                                //设置表对应行的属性值
                                _dataSet.Tables[0].Rows[i][columnNum] = "i18n_1有对应trunk，但是无对应key";
                                _dataSet.Tables[0].Rows[i][columnNum + 1] = xmlkey;
                                _dataSet.Tables[0].Rows[0][columnNum + 2] = _targetxml_1[xmlkey]["trunk"];
                                int k = 0;
                                foreach(string art in _targetxml_1[xmlkey].Keys)
                                {
                                    if (!art.Equals("key"))
                                    {
                                        _dataSet.Tables[0].Rows[i][columnNum + 3 + k] = _targetxml_1[xmlkey][art];
                                    }
                                    k++;
                                }
                            }
                            break;
                        }
                        else
                        {
                            oldkey = xmlkey;
                        }
                        
                    }
                    //匹配失败说明没有对应的key也没有对应的trunk
                    //标注结果
                    if (!flag)
                    {
                        Debug.Log("trunk匹配失败");
                        _dataSet.Tables[0].Rows[i][columnNum] = "i18n没有对应trunk，也无对应key";
                    }
                }
                else
                {
                    keydisptnum_1++;
                }
            }
            if (_targetxml_2.Count > 0)
            {
                if (_targetxml_2.ContainsKey(key))
                {
                    _dataSet.Tables[0].Rows[i][columnNum] = "√";
                    Debug.Log("key匹配成功");
                    //对于对应的列的语言对i18n进行更新
                    for (int j = 1; j <= columnNum - 2; j++)
                    {
                        _targetxml_2[key][_dataSet.Tables[0].Rows[0][j].ToString()] = _dataSet.Tables[0].Rows[i][j].ToString();
                    }
                }
                else if (trunk)//key匹配失败
                {
                    keydisptnum_2++;
                    Debug.Log("key匹配失败");
                    //匹配trunk
                    bool flag = false;
                    string oldkey = "";
                    foreach (string xmlkey in _targetxml_2.Keys)
                    {
                        if (string.CompareOrdinal(xmlkey, key) > 0)
                        {
                            if (_targetxml_2[oldkey]["trunk"] == _dataSet.Tables[0].Rows[0][columnNum - 1].ToString())
                            {
                                Debug.Log("trunk匹配成功");
                                flag = true;
                                //设置表对应行的属性值
                                _dataSet.Tables[0].Rows[i][columnNum] = "i18n_1有对应trunk，但是无对应key";
                                _dataSet.Tables[0].Rows[i][columnNum + 1] = oldkey;
                                _dataSet.Tables[0].Rows[0][columnNum + 2] = _targetxml_2[oldkey]["trunk"];
                                int k = 0;
                                foreach (string art in _targetxml_2[oldkey].Keys)
                                {
                                    if (!art.Equals("key"))
                                    {
                                        _dataSet.Tables[0].Rows[i][columnNum + 3 + k] = _targetxml_2[oldkey][art];
                                    }
                                    k++;
                                }
                            }
                            else if (_targetxml_2[xmlkey]["trunk"] == _dataSet.Tables[0].Rows[0][columnNum - 1].ToString())
                            {
                                Debug.Log("trunk匹配成功");
                                flag = true;
                                //设置表对应行的属性值
                                _dataSet.Tables[0].Rows[i][columnNum] = "i18n_1有对应trunk，但是无对应key";
                                _dataSet.Tables[0].Rows[i][columnNum + 1] = xmlkey;
                                _dataSet.Tables[0].Rows[0][columnNum + 2] = _targetxml_2[xmlkey]["trunk"];
                                int k = 0;
                                foreach (string art in _targetxml_2[xmlkey].Keys)
                                {
                                    if (!art.Equals("key"))
                                    {
                                        _dataSet.Tables[0].Rows[i][columnNum + 3 + k] = _targetxml_2[xmlkey][art];
                                    }
                                    k++;
                                }
                            }
                            break;
                        }
                        else
                        {
                            oldkey = xmlkey;
                        }

                    }
                    //匹配失败说明没有对应的key也没有对应的trunk
                    //标注结果
                    if (!flag)
                    {
                        Debug.Log("trunk匹配失败");
                        _dataSet.Tables[0].Rows[i][columnNum] = "i18n没有对应trunk，也无对应key";
                    }
                }
                else
                {
                    keydisptnum_2++;
                }
            }
        }

        Debug.Log(_targetxml_2.Count);
        if (_targetxml_1.Count>0)
        { 
            tiptext = tf1InputField.text + "匹配完毕," + keydisptnum_1 + "个key未匹配到";
            XmlHandler.TextSerialization(_targetxml_1,tf1InputField.text);
        }
        if (_targetxml_2.Count>0)
        { 
            tiptext += "\n" + tf2InputField.text + "匹配完毕," + keydisptnum_2 + "个key未匹配到";
            XmlHandler.TextSerialization(_targetxml_2, tf2InputField.text);
        }
        //匹配完毕新建表
        //resText.text += "\n匹配完毕";
        ExcelHandler.CreateExcel(_dataSet, logFilePath);
        EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, "导入已完成，日志已保存到：\n" + logFilePath, FilePathMgr.excelexprotfilepath);
    }



    //private void OnSyncBtnClick()
    //{
    //    string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
    //    tipDisplay = true;
    //    langueDic.Clear();
    //    string logFilePath = FilePathMgr.excelexprotfilepath + "\\" + data + ".xlsx";
    //    XmlDocument _targetxml_1 = new XmlDocument();
    //    XmlDocument _targetxml_2 = new XmlDocument();
    //    if (chooseToggle1.isOn) _targetxml_1 = XmlHandler.XmlReader2(tf1InputField.text);//xml数据1
    //    if (chooseToggle2.isOn) _targetxml_2 = XmlHandler.XmlReader2(tf2InputField.text);//xml数据2
    //    XmlNode root_1 = _targetxml_1.SelectSingleNode("root");
    //    XmlNode root_2 = _targetxml_2.SelectSingleNode("root");
    //    //由于读取出来的表格是只能修改数据而不能增加数据
    //    //先创建一个对应的日志表再对其进行对应的修改
    //    DataSet _dataSet = ExcelHandler.ReadByNPOI(rfInputField.text);//表格数据
    //    int columnNum = _dataSet.Tables[0].Columns.Count;
    //    int rowNum = _dataSet.Tables[0].Rows.Count;
    //    int trunkID = -1;

    //    //从第一行第二个开始到倒数第二个为止为其对应的翻译语言
    //    for (int i = 1; i < columnNum - 1; i++)
    //    {
    //        if (root_1 != null)
    //        {
    //            for (int j = 0; j < root_1.ChildNodes[0].Attributes.Count; j++)
    //            {
    //                if (root_1.ChildNodes[0].Attributes[j].Name == _dataSet.Tables[0].Rows[0][i].ToString())
    //                {
    //                    langueDic.Add(_dataSet.Tables[0].Rows[0][i].ToString(), j);
    //                }
    //                if (root_1.ChildNodes[0].Attributes[j].Name == "trunk")
    //                {
    //                    trunkID = j;
    //                }
    //            }
    //        }
    //        else if (root_2 != null)
    //        {
    //            for (int j = 0; j < root_2.ChildNodes[0].Attributes.Count; j++)
    //            {
    //                if (root_2.ChildNodes[0].Attributes[j].Name == _dataSet.Tables[0].Rows[0][i].ToString())
    //                {
    //                    langueDic.Add(_dataSet.Tables[0].Rows[0][i].ToString(), j);
    //                }
    //                if (root_2.ChildNodes[0].Attributes[j].Name == "trunk")
    //                {
    //                    trunkID = j;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("是否选择了xml？");
    //            return;
    //        }
    //    }
    //    //ExcelHandler.CreateExcel(_dataSet, logFilePath,langueDic);
    //    //_dataSet = ExcelHandler.ReadExcelSheet(logFilePath);
    //    if (_dataSet == null)
    //    {
    //        Debug.Log("表格数据为空？？？");
    //        tipDisplay = true;
    //        return;
    //    }
    //    _dataSet.Tables[0].Columns.Add("处理结果");
    //    _dataSet.Tables[0].Columns.Add("key");
    //    _dataSet.Tables[0].Columns.Add("trunk");
    //    _dataSet.Tables[0].Rows[0][columnNum] = "处理结果";
    //    _dataSet.Tables[0].Rows[0][columnNum + 1] = "key";
    //    _dataSet.Tables[0].Rows[0][columnNum + 2] = "trunk";
    //    for (int i = 0; i < langueDic.Count; i++)
    //    {
    //        _dataSet.Tables[0].Columns.Add(_dataSet.Tables[0].Rows[0][1 + i].ToString() + "翻译");
    //        _dataSet.Tables[0].Rows[0][columnNum + 3 + i] = _dataSet.Tables[0].Rows[0][1 + i].ToString() + "翻译";
    //    }
    //    //key匹配
    //    int keydisptnum_1 = 0;
    //    int keydisptnum_2 = 0;
    //    for (int i = 1; i < rowNum; i++)
    //    {
    //        //key匹配成功
    //        string key = _dataSet.Tables[0].Rows[i][0].ToString();
    //        if (key.Equals("")) {keydisptnum_1++; keydisptnum_2++; continue; }
    //        if (root_1 != null)
    //        {
    //            int keyIndex_1 = XmlHandler.XmlKeySearch(root_1, key);

    //            if (keyIndex_1 >= 0)
    //            {
    //                _dataSet.Tables[0].Rows[i][columnNum] = "√";
    //                Debug.Log("key匹配成功");
    //                //对于对应的列的语言对i18n进行更新
    //                for (int j = 1; j < langueDic.Count; j++)
    //                {
    //                    root_1.ChildNodes[keyIndex_1].Attributes[langueDic[_dataSet.Tables[0].Rows[0][j].ToString()]].Value = _dataSet.Tables[0].Rows[i][j].ToString();
    //                }
    //            }
    //            else//key匹配失败
    //            {
    //                keydisptnum_1++;
    //                Debug.Log("key匹配失败");
    //                //匹配trunk
    //                bool flag = false;
    //                keyIndex_1 = XmlHandler.XmlKeyInsertIndexSearch(root_1, key);
    //                for (int j = keyIndex_1; j < 2; j++)
    //                {
    //                    //匹配成功
    //                    if (root_1.ChildNodes[j].Attributes[trunkID].Value == _dataSet.Tables[0].Rows[0][columnNum - 1].ToString())
    //                    {
    //                        Debug.Log("trunk匹配成功");
    //                        flag = true;
    //                        //设置表对应行的属性值
    //                        _dataSet.Tables[0].Rows[i][columnNum] = "i18n_1有对应trunk，但是无对应key";
    //                        _dataSet.Tables[0].Rows[i][columnNum + 1] = root_1.ChildNodes[j].Attributes[0].Value;
    //                        _dataSet.Tables[0].Rows[0][columnNum + 2] = root_1.ChildNodes[j].Attributes[trunkID].Value;
    //                        for (int k = 0; k < langueDic.Count; k++)
    //                        {
    //                            //设置对应的列的值
    //                            _dataSet.Tables[0].Rows[i][columnNum + 3 + k] = root_1.ChildNodes[j].Attributes[langueDic[_dataSet.Tables[0].Rows[0][k + 1].ToString()]].Value;
    //                        }
    //                        break;
    //                    }
    //                }
    //                //匹配失败说明没有对应的key也没有对应的trunk
    //                //标注结果
    //                if (!flag)
    //                {
    //                    Debug.Log("trunk匹配失败");
    //                    _dataSet.Tables[0].Rows[i][columnNum] = "i18n_1没有对应trunk，也无对应key";
    //                }
    //            }
    //        }
    //        if (root_2 != null)
    //        {//root_2

    //            int keyIndex_2 = XmlHandler.XmlKeySearch(root_2, key);
    //            if (keyIndex_2 >= 0)
    //            {
    //                _dataSet.Tables[0].Rows[i][columnNum] = "√";
    //                Debug.Log("key匹配成功");
    //                //对于对应的列的语言对i18n进行更新
    //                for (int j = 1; j < langueDic.Count; j++)
    //                {
    //                    root_2.ChildNodes[keyIndex_2].Attributes[langueDic[_dataSet.Tables[0].Rows[0][j].ToString()]].Value = _dataSet.Tables[0].Rows[i][j].ToString();
    //                }
    //            }
    //            else//key匹配失败
    //            {
    //                keydisptnum_2++;
    //                Debug.Log("key匹配失败");
    //                //匹配trunk
    //                bool flag = false;
    //                keyIndex_2 = XmlHandler.XmlKeyInsertIndexSearch(root_2, key);
    //                for (int j = keyIndex_2; j < 2; j++)
    //                {
    //                    //匹配成功
    //                    if (root_2.ChildNodes[j].Attributes[trunkID].Value == _dataSet.Tables[0].Rows[0][columnNum - 1].ToString())
    //                    {
    //                        Debug.Log("trunk匹配成功");
    //                        flag = true;
    //                        //设置表对应行的属性值
    //                        _dataSet.Tables[0].Rows[i][columnNum] += "\ni18n_2有对应trunk，但是无对应key";
    //                        _dataSet.Tables[0].Rows[i][columnNum + 1] += "\n" + root_2.ChildNodes[j].Attributes[0].Value;
    //                        _dataSet.Tables[0].Rows[0][columnNum + 2] += "\n" + root_2.ChildNodes[j].Attributes[trunkID].Value;
    //                        for (int k = 0; k < langueDic.Count; k++)
    //                        {
    //                            //设置对应的列的值
    //                            _dataSet.Tables[0].Rows[i][columnNum + 3 + k] += "\n" + root_2.ChildNodes[j].Attributes[langueDic[_dataSet.Tables[0].Rows[0][k + 1].ToString()]].Value;
    //                        }
    //                        break;
    //                    }
    //                }
    //                //匹配失败说明没有对应的key也没有对应的trunk
    //                //标注结果
    //                if (!flag)
    //                {
    //                    Debug.Log("trunk匹配失败");
    //                    _dataSet.Tables[0].Rows[i][columnNum] += "\n" + "i18n_2没有对应trunk，也无对应key";
    //                }
    //            }
    //        }
    //    }
    //    if (root_1 != null)
    //    { tiptext = tf1InputField.text + "匹配完毕," + keydisptnum_1 + "个key未匹配到"; }
    //    else if (root_2 != null)
    //    { tiptext += "\n" + tf2InputField.text + "匹配完毕," + keydisptnum_2 + "个key未匹配到"; }
    //    //匹配完毕新建表
    //    //resText.text += "\n匹配完毕";
    //    ExcelHandler.CreateExcel(_dataSet, logFilePath);
    //    tipDisplay = false;
    //    Thread.Sleep(100);
    //    thread.Abort();
    //}
}
