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

public class XlsxToXmlPanel : MonoBehaviour
{
    private Button backButton;
    private Button BtnHowToUse;
    private Button transButton;
    //文件路径输入框
    private InputField rfInputField;
    private InputField tf1InputField;
    private InputField tf2InputField;
    //文件浏览按钮
    private Button fileBrowseBtn1;
    private Button fileBrowseBtn2;
    private Button fileBrowseBtn3;

    private Text resText;
    private GameObject tipPanel;
    private bool tipDisplay = false;
    private string tip = "";
    Thread thread;

    private void Awake()
    {
        backButton = this.transform.Find("BackButton").GetComponent<Button>();
        BtnHowToUse = this.transform.Find("BtnHowToUse").GetComponent<Button>();
        transButton = this.transform.Find("transButton").GetComponent<Button>();
        rfInputField = this.transform.Find("RFilePathInputField").GetComponent<InputField>();
        tf1InputField = this.transform.Find("T1FilePathInputField").GetComponent<InputField>();
        tf2InputField = this.transform.Find("T2FilePathInputField").GetComponent<InputField>();

        fileBrowseBtn1 = rfInputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn2 = tf1InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn3 = tf2InputField.transform.Find("FileBrowseButton").GetComponent<Button>();

        resText = this.transform.Find("ResultTextBackground").Find("ResultText").GetComponent<Text>();

        tipPanel = this.transform.Find("TipPanel").gameObject;

        fileBrowseBtn1.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.Browse(new StringBuilder(rfInputField.text).ToString());
            if (!filename.Equals(""))
            {
                rfInputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[7][1] = rfInputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn2.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.Browse(new StringBuilder(tf1InputField.text).ToString());
            Debug.Log(filename);
            if (!filename.Equals(""))
            {
                tf1InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[7][2] = tf1InputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn3.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.Browse(new StringBuilder(tf2InputField.text).ToString());
            Debug.Log(filename);
            if (!filename.Equals(""))
            {
                tf2InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[7][3] = tf2InputField.text;
                FilePathMgr.savedata();
            }
        });
        backButton.onClick.AddListener(OnBackBtnClick);
        BtnHowToUse.onClick.AddListener(delegate ()
        {
            string tipstr = "1.表格文件夹：指需要转换的excel存储的文件夹位置\n" +
            "2.xml文件夹：值转换后的xml文件夹位置\n" +
            "3.浏览按钮：点击后进行对应文件夹选择\n";
            EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, tipstr, null);
        });
        transButton.onClick.AddListener(delegate () {
            thread = new Thread(new ThreadStart(delegate () {
                OntransBtnClick();
                tipDisplay = false;
            })); thread.Start();
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        rfInputField.text = FilePathMgr.allPathData.Tables[0].Rows[7][1].ToString();
        tf1InputField.text = FilePathMgr.allPathData.Tables[0].Rows[7][2].ToString();
        tf2InputField.text = FilePathMgr.allPathData.Tables[0].Rows[7][3].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        tipPanel.SetActive(tipDisplay);
        if (!tip.Equals(""))
        {
            tipPanel.transform.Find("Text").GetComponent<Text>().text = tip;
            resText.text = tip;
        }
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

    private void OntransBtnClick()
    {
        if (rfInputField.text.Trim().Equals(""))
        {
            EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, "excel文件夹为名空！！", null);
            return;
        }
        if (tf1InputField.text.Trim().Equals(""))
        {
            EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, "xml文件夹名为空！！", null);
            return;
        }
        if (tf2InputField.text.Trim().Equals(""))
        {
            EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, "proto文件夹名为空！！", null);
            return;
        }
        tipDisplay = true;

        DirectoryInfo directoryInfo = new DirectoryInfo(rfInputField.text);
        FileInfo[] files = directoryInfo.GetFiles("*.xlsx");
        for (int i = 0; i < files.Length; i++)
        {
            tip += files[i].Name + "转化中\n";
            XlsxToXml(files[i]);
            tip += files[i].Name + "转化完成\n";
        }


        tipDisplay = false;
        Thread.Sleep(100);
        thread.Abort();
    }

    private void XlsxToXml(FileInfo file)
    {
        DataSet exceldata = ExcelHandler.ReadByNPOI(file.FullName);
        XmlDocument xml = new XmlDocument();
        xml.CreateXmlDeclaration("1.0", "UTF-8", "yes");//设置xml文件编码格式为UTF-8
        XmlNode root = xml.CreateElement("ExData");//创建根节点
        XmlElement table = xml.CreateElement("Table");//创建Table节点
        XmlElement columnList = xml.CreateElement("ColumnList");//创建ColumnList节点
        int rowCount = exceldata.Tables[0].Rows.Count;
        ExcelData excelData = new ExcelData();
        bool artFlag = false;
        for (int i = 0;i<rowCount;i++)
        {
            string rowHead = exceldata.Tables[0].Rows[i][0].ToString();
            if (rowHead.Equals(""))
            {
                if (artFlag)
                {
                    string artName = exceldata.Tables[0].Rows[i][1].ToString();
                    string artValue = exceldata.Tables[0].Rows[i][2].ToString();
                    if (artName.Equals("")) continue;
                    if (artName[0] == '#') continue;
                    excelData.artDic.Add(artName, artValue);
                    XmlElement attribute = xml.CreateElement("Attribute");//创建Attribute属性
                    attribute.SetAttribute("Name", artName);
                    attribute.SetAttribute("Value", artValue);
                    attribute.SetAttribute("DataType", artValue.GetDataType().ToString());
                    table.AppendChild(attribute);
                }
                continue;
            }
            else if (rowHead[0] == '#') continue;
            else if(rowHead.IndexOf("TABLE")>=0)
            {
                excelData.tableName = exceldata.Tables[0].Rows[i][1].ToString();
                table.SetAttribute("Name", excelData.tableName);
            }
            else if(rowHead.IndexOf("ATTRIBUTE")>=0)
            {
                artFlag = true;
                string artName = exceldata.Tables[0].Rows[i][1].ToString();
                string artValue = exceldata.Tables[0].Rows[i][2].ToString();
                if (artName[0] == '#') continue;
                excelData.artDic.Add(artName,artValue);
                XmlElement attribute = xml.CreateElement("Attribute");//创建Attribute属性
                attribute.SetAttribute("Name", artName);
                attribute.SetAttribute("Value", artValue);
                attribute.SetAttribute("DataType", artValue.GetDataType().ToString());
                table.AppendChild(attribute);
            } 
            else
            {
                if (rowHead.CheckChinese()) continue;
                if (excelData.keyColDic.Count == 0)
                {
                    for (int j = 0; j < exceldata.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        string keyName = exceldata.Tables[0].Rows[i][j].ToString();
                        if (keyName.Equals("")) break;
                        if (keyName[0] == '#') continue;
                        excelData.keyColDic.Add(j, keyName);
                        excelData.dataDic.Add(keyName, new List<string>());
                        XmlElement column = xml.CreateElement("Column");//创建Column属性
                        column.SetAttribute("Name", keyName);
                        column.SetAttribute("DataType", keyName.GetDataType());
                        columnList.AppendChild(column);
                    }
                }
                else
                {
                    XmlElement record = xml.CreateElement("Record");//创建Record属性
                    for (int j = 0; j < exceldata.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        string data = exceldata.Tables[0].Rows[i][j].ToString();
                        if (data.Equals("")) break;
                        if (excelData.keyColDic.ContainsKey(j))
                        {
                            excelData.dataDic[excelData.keyColDic[j]].Add(data);
                            record.SetAttribute(excelData.keyColDic[j], data);
                        }
                    }
                    table.AppendChild(record);
                }
            }
        }
        StringBuilder protoContent = new StringBuilder(FileTemplate.Instance.protoTemplate);
        protoContent.Replace("${TABLE_NAME}", excelData.tableName+"Set");
        protoContent.Replace("${RECORD_NAME}", excelData.tableName);
        protoContent.Replace("${RECORDS_VAR_NAME}", excelData.tableName.ToLower()+"s");

        int index = 1;
        StringBuilder recordList = new StringBuilder();
        foreach (int key in excelData.keyColDic.Keys)
        {
            recordList.AppendFormat("        required {0} {1} = {2};\n", excelData.keyColDic[key].GetDataType().GetDataTypeName(), excelData.keyColDic[key], index++);
        }
        protoContent.Replace("${RECORD_LIST}", recordList.ToString());

        index = 2;
        StringBuilder attributeList = new StringBuilder();
        foreach (string key in excelData.artDic.Keys)
        {
            attributeList.AppendFormat("    required {0} {1} = {2};\n", key.GetDataType().GetDataTypeName(), key, index++);
        }
        protoContent.Replace("${ATTRIBUTE_LIST}", attributeList.ToString());
        File.WriteAllText(tf2InputField.text+"/"+excelData.tableName+".proto", protoContent.ToString());

        XmlElement Attribute = xml.CreateElement("Attribute");//创建Attribute属性
        Attribute.SetAttribute("Name", "l_crc_code");
        Attribute.SetAttribute("Value", DateTime.Now.Ticks.ToString());
        Attribute.SetAttribute("DataType", "4");
        table.AppendChild(columnList);
        table.AppendChild(Attribute);
        root.AppendChild(table);
        xml.AppendChild(root);
        xml.Save(tf1InputField.text + "/" + excelData.tableName + ".xml");
    }
}
