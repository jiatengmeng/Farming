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
    string objPath = "";

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

        objPath = Application.dataPath;
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
        #region proto创建
        //proto创建
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
        attributeList.AppendFormat("    required {0} {1} = {2};\n", "l_crc_code".GetDataType().GetDataTypeName(), "l_crc_code", index++);
        foreach (string key in excelData.artDic.Keys)
        {
            attributeList.AppendFormat("    required {0} {1} = {2};\n", key.GetDataType().GetDataTypeName(), key, index++);
        }
        protoContent.Replace("${ATTRIBUTE_LIST}", attributeList.ToString());
        File.WriteAllText(tf2InputField.text+"/"+excelData.tableName+".proto", protoContent.ToString());
        #endregion

        #region BinaryWriter创建
        StringBuilder binaryWriterContent = new StringBuilder(FileTemplate.Instance.binaryFileWriter);
        binaryWriterContent.Replace("${RECORD_NAME}", excelData.tableName);
        string tablesName = excelData.tableName.ToUpper()[0] + excelData.tableName.ToLower().Substring(1);
        Debug.Log(tablesName);
        binaryWriterContent.Replace("${TABLE_NAME_FIRST_UPPER}", tablesName);

        //属性
        attributeList.Clear();
        foreach (string key in excelData.artDic.Keys)
        {
            attributeList.Append("        {\n");
            attributeList.AppendFormat("            if (attributeDic.ContainsKey(\"{0}\")\n", key);
            attributeList.Append("            {\n");
            switch(key.GetDataType())
            {
                //int
                case "1":
                    {
                        attributeList.AppendFormat("            records.{0} = int32.Parse(attributeDic[\"{1}\"].Attributes[\"Value\"].Value);\n", EveryHeadCharUpper(key), key);
                    }
                    break;
                //string
                case "2":
                    {
                        attributeList.AppendFormat("            records.{0} = attributeDic[\"{1}\"].Attributes[\"Value\"].Value;\n", EveryHeadCharUpper(key), key);
                    }
                    break;
                //float
                case "3":
                    {
                        attributeList.AppendFormat("            records.{0} = float.Parse(attributeDic[\"{1}\"].Attributes[\"Value\"].Value);\n", EveryHeadCharUpper(key), key);
                    }
                    break;
                //long
                case "4":
                    {
                        attributeList.AppendFormat("            records.{0} = long.Parse(attributeDic[\"{1}\"].Attributes[\"Value\"].Value);\n", EveryHeadCharUpper(key), key);
                    }
                    break;
                default:
                    Debug.Log("错误类型"+key);
                    break;

            }
            attributeList.Append("            }\n");
            attributeList.Append("        }\n");
        }
        binaryWriterContent.Replace("${ATTRIBUTE_SETTER}", attributeList.ToString());

        //内容
        recordList.Clear();
        foreach (int key in excelData.keyColDic.Keys)
        {
            switch (excelData.keyColDic[key].GetDataType())
            {
                //int
                case "1":
                    {
                        recordList.AppendFormat("            record.{0} = node.Attributes[\"{1}\"].ToString().ToInt();\n", EveryHeadCharUpper(excelData.keyColDic[key]), excelData.keyColDic[key]);
                    }
                    break;
                //string
                case "2":
                    {
                        recordList.AppendFormat("            record.{0} = node.Attributes[\"{1}\"].ToString();\n", EveryHeadCharUpper(excelData.keyColDic[key]), excelData.keyColDic[key]);
                    }
                    break;
                //float
                case "3":
                    {
                        recordList.AppendFormat("            record.{0} = float.Parse(node.Attributes[\"{1}\"].ToString());\n", EveryHeadCharUpper(excelData.keyColDic[key]), excelData.keyColDic[key]);
                    }
                    break;
                //long
                case "4":
                    {
                        recordList.AppendFormat("            record.{0} = long.Parse(node.Attributes[\"{1}\"].ToString());\n", EveryHeadCharUpper(excelData.keyColDic[key]), excelData.keyColDic[key]);
                    }
                    break;
                default:
                    Debug.Log("错误类型" + key);
                    break;

            }
        }


        binaryWriterContent.Replace("${RECORD_PROPERTY_SETTER}", recordList.ToString());

        File.WriteAllText(objPath + "/Scripts/Protobuf/BinaryWriter/" + excelData.tableName + "Writer.cs", binaryWriterContent.ToString());
        #endregion
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

    private string EveryHeadCharUpper(string str)
    {
        string res = "";
        string[] strs = str.Split('_');
        for(int i=0;i<strs.Length;i++)
        {
            res += strs[i][0].ToString().ToUpper() + strs[i].Substring(1);
        }
        return res;
    }
}
