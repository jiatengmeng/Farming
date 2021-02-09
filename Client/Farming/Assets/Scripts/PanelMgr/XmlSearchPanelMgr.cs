using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class XmlSearchPanelMgr : MonoBehaviour
{
    public GameObject languageToggle;

    private GameObject content;
    private GameObject resTextContent;
    private GridLayoutGroup contentGlg;
    private InputField keyIF;
    //private Text resText;
    private Text resultText;
    private Button searchBtn;
    private Button BtnHowToUse;
    private Button backBtn;

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

    private GameObject tipPanel;
    private bool tipDisplay = false;
    private Thread thread;

    private Dictionary<string, bool> languageDic = new Dictionary<string, bool>();
    private Dictionary<string, int> artDic = new Dictionary<string, int>();

    private int childCount = 0;
    private bool isCreatetoggle = false;

    string tiptext = "";

    private XmlDocument xDoc_1 = new XmlDocument();
    private XmlDocument xDoc_2 = new XmlDocument();
    private XmlDocument xDoc_3 = new XmlDocument();

    private string file1 = "";
    private string file2 = "";
    private string file3 = "";
    private bool ison1 = false;
    private bool ison2 = false;
    private bool ison3 = false;

    float textH = 0.0f;

    string excelPath;
    //private Button testbtn;
    private void Awake()
    {
        resTextContent = this.transform.Find("ResultScroll").GetComponent<ScrollRect>().content.gameObject;
        content = this.transform.Find("LanguageToggleScrollView").GetComponent<ScrollRect>().content.gameObject;
        contentGlg = content.GetComponent<GridLayoutGroup>();
        keyIF = this.transform.Find("KeyInputField").GetComponent<InputField>();
        //resText = this.transform.Find("ResultTextBackground").Find("ResultText").GetComponent<Text>();
        resultText = resTextContent.transform.Find("ResultText").GetComponent<Text>();
        searchBtn = this.transform.Find("SearchButton").GetComponent<Button>();
        BtnHowToUse = this.transform.Find("BtnHowToUse").GetComponent<Button>();
        backBtn = this.transform.Find("BackButton").GetComponent<Button>();
        tipPanel = this.transform.Find("TipPanel").gameObject;

        tf1InputField = this.transform.Find("File_1_PathInputField").GetComponent<InputField>();
        tf2InputField = this.transform.Find("File_2_PathInputField").GetComponent<InputField>();
        tf3InputField = this.transform.Find("File_3_PathInputField").GetComponent<InputField>();

        fileBrowseBtn1 = tf1InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn2 = tf2InputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn3 = tf3InputField.transform.Find("FileBrowseButton").GetComponent<Button>();

        chooseToggle1 = tf1InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();
        chooseToggle2 = tf2InputField.transform.Find("ChooseToggle").GetComponent<Toggle>();
        chooseToggle3 = tf3InputField.transform.Find("ChooseToggle").GetComponent<Toggle>(); 
        fileBrowseBtn1.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf1InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf1InputField.text = filename;
                isCreatetoggle = false;
                FilePathMgr.allPathData.Tables[0].Rows[1][1] = tf1InputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn2.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf2InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf2InputField.text = filename;
                isCreatetoggle = false;
                FilePathMgr.allPathData.Tables[0].Rows[1][2] = tf2InputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn3.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.BrowseXml(new StringBuilder(tf3InputField.text).ToString());
            if (!filename.Equals(""))
            {
                tf3InputField.text = filename;
                isCreatetoggle = false;
                FilePathMgr.allPathData.Tables[0].Rows[1][3] = tf3InputField.text;
                FilePathMgr.savedata();
            }
        });

        searchBtn.onClick.AddListener(delegate () { thread = new Thread(new ThreadStart(OnSearchBtnClick)); thread.Start(); }/*OnSearchBtnClick*/);
        backBtn.onClick.AddListener(OnBackButtonClick);
        BtnHowToUse.onClick.AddListener(delegate ()
        {
            string tipstr = "1.目标文件：对应的检索文件\n" +
            "2语言选择框：在选择完对应的文件后，会生成对应的语言选择，勾选后对其进行对应的检索\n" +
            "3.key输入框：输入需要检索的key关键字（多个key用英文的逗号隔开），来进行检索\n" +
            "4.结果框：会生成对应的结果在其中\n" +
            "5.开始检索：点击后对对应的文件中的对应的key的对应的属性进行检索，并导出结果\n" +
            "6.导出结果：日志在MultLanguageMaintenanceTool\\XmlSearchFolder文件夹中";
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, tipstr,null);
        });
        textH = resultText.gameObject.GetComponent<RectTransform>().rect.height;
    }

    // Start is called before the first frame update
    void Start()
    {
        tf1InputField.text = FilePathMgr.allPathData.Tables[0].Rows[1][1].ToString();
        tf2InputField.text = FilePathMgr.allPathData.Tables[0].Rows[1][2].ToString();
        tf3InputField.text = FilePathMgr.allPathData.Tables[0].Rows[1][3].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(childCount!=content.transform.childCount)
        {
            childCount = content.transform.childCount;
            if (childCount * (contentGlg.cellSize.x + contentGlg.spacing.x) > content.GetComponent<RectTransform>().rect.width)
            {
                content.GetComponent<RectTransform>().sizeDelta = new Vector2(childCount * (contentGlg.cellSize.x + contentGlg.spacing.x), contentGlg.cellSize.y);
            }
        }
        tipPanel.SetActive(tipDisplay);
        //resText.text = tiptext;
        if (!isCreatetoggle)
        { 
            if (chooseToggle1.isOn &&!tf1InputField.text.Trim().Equals(""))
            {
                xDoc_1 = XmlHandler.XmlReader2(tf1InputField.text.Trim());
                CreateToggle(xDoc_1);
                isCreatetoggle = true;
            }
            else if (chooseToggle2.isOn && !tf2InputField.text.Trim().Equals(""))
            {
                xDoc_2 = XmlHandler.XmlReader2(tf2InputField.text.Trim());
                CreateToggle(xDoc_2);
                isCreatetoggle = true;
            }
            else if (chooseToggle3.isOn && !tf3InputField.text.Trim().Equals(""))
            {
                xDoc_3 = XmlHandler.XmlReader2(tf3InputField.text.Trim());
                CreateToggle(xDoc_3);
                isCreatetoggle = true;
            }
        }
        file1 = tf1InputField.text;
        file2 = tf2InputField.text;
        file3 = tf3InputField.text;
        ison1 = chooseToggle1.isOn;
        ison2 = chooseToggle2.isOn;
        ison3 = chooseToggle3.isOn;

        if(resultText.gameObject.GetComponent<RectTransform>().rect.height>textH)
        {
            textH = resultText.gameObject.GetComponent<RectTransform>().rect.height;
            resTextContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, resultText.gameObject.GetComponent<RectTransform>().rect.height);
            //resultText.gameObject.GetComponent<RectTransform>().position = new Vector3(0, textH / 2, 0);
        }
        resultText.text = tiptext;

    }
    private void CreateToggle(XmlDocument xml)
    {
        for(int i=0;i<content.transform.childCount;i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        languageDic.Clear();
        artDic.Clear();
        XmlNode resNode = xml.SelectSingleNode("root");
        //test();
        int id = 0;
        foreach (XmlAttribute art in resNode.ChildNodes[0].Attributes)
        {
            if (!art.Name.Equals("key"))
            {
                if (art.Name.Equals("id")) break;
                GameObject _Toggle = Instantiate(languageToggle, content.transform);
                _Toggle.GetComponent<Toggle>().isOn = false;
                _Toggle.transform.Find("Label").GetComponent<Text>().text = art.Name;
                languageDic.Add(art.Name, false);
                artDic.Add(art.Name,id);
                _Toggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool ChangeValue) { OnToggleValueChange(art.Name, ChangeValue); });
            }
            id++;
        }
    }

    private void OnToggleValueChange(string artName,bool value)
    {
        if(languageDic.ContainsKey(artName))
        {
            languageDic[artName] = value;
        }
    }

    private void OnSearchBtnClick()
    {
        tiptext = "检索开始:\n";
        tipDisplay = true;
        if (ison1 && !file1.Trim().Equals(""))
        {
            SearchXml(file1.Trim(),xDoc_1);
        }
        else if (ison2 && !file2.Trim().Equals(""))
        {
            SearchXml(file2.Trim(), xDoc_2);
        }
        else if (ison3 && !file3.Trim().Equals(""))
        {
            SearchXml(file3.Trim(), xDoc_3);
        }
        EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, "检索结束，日志已保存到" + excelPath, FilePathMgr.xmlsearchfilepath);
        tipDisplay = false;
        Thread.Sleep(100);
        thread.Abort();
    }

    private void SearchXml(string filename,XmlDocument xml)
    {
        XmlNode resNode = xml.SelectSingleNode("root");
        Dictionary<string, Dictionary<string, string>> xmlData = XmlHandler.XmlReadByText(filename);
        string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
        tiptext += "文件：" + filename+"\n";
        excelPath = FilePathMgr.xmlsearchfilepath + "\\" + data + ".xlsx";
        Dictionary<string, List<int>> keyNodeIndex = new Dictionary<string, List<int>>();
        bool ischoose = false;
        foreach (string key in languageDic.Keys)
        {
            if (languageDic[key])
            {
                ischoose = true;
                break;
            }
        }
        if (!ischoose) { tiptext += "请先选择对应的语种！！！\n"; return; }
        if (keyIF.text.Trim().Equals(""))
        {
            tiptext += "全部关键字有" + resNode.ChildNodes.Count + "个\n";
            foreach (string key in languageDic.Keys)
            {
                if (languageDic[key])
                {
                    int count = 0;
                    int artid = artDic[key];
                    for (int i = 0; i < resNode.ChildNodes.Count; i++)
                    {
                        if (System.Security.SecurityElement.Escape(resNode.ChildNodes[i].Attributes[artid].Value).Equals(""))
                        {
                            count++;
                        }
                    }
                    tiptext += key + "空属性:" + count + "个\n";
                }
            }
            //ExcelHandler.CreateExcel(resNode, excelPath);
            //tiptext += "日志：" + excelPath + "\n";
        }
        else
        {
            string[] keys = keyIF.text.Split(',');
            for (int j = 0; j < keys.Length; j++)
            {
                if (keys[j].Trim().Equals("")) continue;
                if (keyNodeIndex.ContainsKey(keys[j])) continue;
                List<int> nodeid = new List<int>();
                for (int i = 0; i < resNode.ChildNodes.Count; i++)
                {
                    if (System.Security.SecurityElement.Escape(resNode.ChildNodes[i].Attributes[0].Value).IndexOf(keys[j]) >=0)
                    {
                        nodeid.Add(i);
                    }
                }
                keyNodeIndex.Add(keys[j], nodeid);
                tiptext += "包含" + keys[j] + "的关键字有" + nodeid.Count + "个\n";
                foreach (string key in languageDic.Keys)
                {
                    if (languageDic[key])
                    {
                        int count = 0;
                        int artid = artDic[key];
                        for (int i = 0; i < nodeid.Count; i++)
                        {
                            if (resNode.ChildNodes[nodeid[i]].Attributes[artid].Value.Equals(""))
                            {
                                count++;
                            }
                        }
                        tiptext +=key + "为空属性:"+count+"个\n";
                    }
                }
            }
            ExcelHandler.CreateExcel(keyNodeIndex, resNode, excelPath);
            tiptext += "日志：" + excelPath + "\n";
        }
    }

    private void SearchXml(string filename)
    {
        Dictionary<string, Dictionary<string, string>> xmlData = XmlHandler.XmlReadByText(filename);
        string excelPath;
        string data = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
        tiptext += "文件：" + filename + "\n";
        excelPath = FilePathMgr.xmlsearchfilepath + "\\" + data + ".xlsx";
        bool ischoose = false;
        foreach (string key in languageDic.Keys)
        {
            if (languageDic[key])
            {
                ischoose = true;
                break;
            }
        }
        if (!ischoose) { tiptext += "请先选择对应的语种！！！\n"; return; }
        if (keyIF.text.Trim().Equals(""))
        {
            tiptext += "全部关键字有" + xmlData.Count + "个\n";
            foreach (string key in languageDic.Keys)
            {
                if (languageDic[key])
                {
                    int count = 0;
                    foreach (string key2 in xmlData.Keys)
                    {
                        if (xmlData[key2][key].Equals(""))
                        {
                            count++;
                        }
                    }
                    tiptext += key + "为空属性:" + count + "个\n";
                }
            }
            //ExcelHandler.CreateExcel(resNode, excelPath);
            tiptext += "全部关键字过多，日志不输出\n";
        }
        else
        {
            string[] keys = keyIF.text.Split(',');
            Dictionary<string, Dictionary<string, string>> xmlTempData = new Dictionary<string, Dictionary<string, string>>();
            for (int j = 0; j < keys.Length; j++)
            {
                if (keys[j].Trim().Equals("")) continue;
                int count = 0;
                foreach (string key in xmlData.Keys)
                {
                    if (key.ToLower().IndexOf(keys[j].ToLower()) >= 0)
                    {
                        count++;
                        if (xmlTempData.ContainsKey(key)) continue;
                        xmlTempData.Add(key,xmlData[key]);
                    }
                }
                tiptext += "包含" + keys[j] + "的关键字有" + count + "个\n";
                foreach (string key in languageDic.Keys)
                {
                    if (languageDic[key])
                    {
                        int emptyCount = 0;
                        foreach (string key2 in xmlTempData.Keys)
                        {
                            if (xmlTempData[key2][key].Equals(""))
                            {
                                emptyCount++;
                            }
                        }
                        tiptext += key + "为空属性:" + emptyCount + "个\n";
                    }
                }
            }
            ExcelHandler.CreateExcel(xmlTempData, excelPath);
            tiptext += "日志：" + excelPath + "\n";
        }
    }

    private void OnBackButtonClick()
    {
        this.gameObject.SetActive(false);
        resultText.text = "暂无结果";
        isCreatetoggle = false;
        for(int i = 0;i<content.transform.childCount;i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        keyIF.text = "";
        languageDic.Clear();
        artDic.Clear();
        EventMgr.Broadcast(EventType.PanelBack);
    }
}
