using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class XmlModifcationPanel : MonoBehaviour
{
    public GameObject keyButton;
    private Button BtnHowToUse;
    public GameObject ValueUI;

    private Button openXmlFile;
    private Button saveXmlFile;
    private Button search;
    private Button backButton;

    private Text tipText;
    private InputField searchInputField;

    private ScrollRect keyScrollRect;
    private ScrollRect valueScrollRect;
    private GameObject keyContent;
    private GameObject valueContent;
    private GridLayoutGroup keyGridLayoutGroup;
    private GridLayoutGroup valueGridLayoutGroup;

    private float screenWidth = 0.0f;
    private string filepath = "";
    private string tip = "";
    Dictionary<string, Dictionary<string, string>> xmldata = new Dictionary<string, Dictionary<string, string>>();
    Dictionary<string, GameObject> ValueUIDic = new Dictionary<string, GameObject>();

    private Thread thread;
    // Start is called before the first frame update
    void Awake()
    {
        //ui
        BtnHowToUse = this.transform.Find("BtnHowToUse").GetComponent<Button>();
        openXmlFile = this.transform.Find("OpenXmlFile").GetComponent<Button>();
        saveXmlFile = this.transform.Find("SaveXmlFile").GetComponent<Button>();
        backButton = this.transform.Find("BackButton").GetComponent<Button>();
        searchInputField = this.transform.Find("SearchInputField").GetComponent<InputField>();
        search = searchInputField.transform.Find("Search").GetComponent<Button>();
        tipText = searchInputField.transform.Find("TipText").GetComponent<Text>();

        keyScrollRect = this.transform.Find("KeyScrollView").GetComponent<ScrollRect>();
        valueScrollRect = this.transform.Find("ValueScrollView").GetComponent<ScrollRect>();
        keyContent = keyScrollRect.content.gameObject;
        valueContent = valueScrollRect.content.gameObject;
        keyGridLayoutGroup = keyContent.GetComponent<GridLayoutGroup>();
        valueGridLayoutGroup = valueContent.GetComponent<GridLayoutGroup>();

        //uievent
        openXmlFile.onClick.AddListener(delegate ()
        {
            string filter = "(*.xml)|\0*.xml\0\0";
            string defExt = "xml";
            string Path = OpenFileByWin32.OpenFile(filter, defExt, filepath);
            if (!Path.Equals("")) 
            {
                filepath = Path;
                FilePathMgr.allPathData.Tables[0].Rows[5][1] = filepath;
                FilePathMgr.savedata();
                tipText.text = "当前路径为" + filepath;
                thread = new Thread(new ThreadStart(delegate ()
                {
                    VersionSyncPanel.ProcessCommand("cmd.exe", "svn update " + "\"" + filepath + "\"");
                    xmldata = XmlHandler.XmlReadByText(new StringBuilder(filepath).ToString());
                    tip = filepath + "读取完毕,可以开始查找";
                    Thread.Sleep(100);
                    thread.Abort();
                }));
                thread.Start();
            }
            else tipText.text = "<color=red>无效路径</color>";
        });
        BtnHowToUse.onClick.AddListener(delegate ()
        {
            string tipstr = "1.openfile：打开对应的xml文件\n" +
            "2.save：保存修改结果\n" +
            "3.查找：在openfile之后对打开的xml进行查找，可以无视大小写，可以输入中文\n" +
            "4.左半边框：左半边显示的为查找结果，点击对应的条目将会显示其属性与右半边框中\n" +
            "5.右半边框：右半边显示对应key的属性值，点击对应属性值进行修改，在结束修改时会对修改进行保存\n" +
            "6.日志：日志存储于MultLanguageMaintenanceTool\\XmlUpdataFolder。";
            EventMgr.Broadcast<string,string>(EventType.TipPanelDisplay, tipstr,null);
        });
        search.onClick.AddListener(OnSearchButtonClick);
        saveXmlFile.onClick.AddListener(OnSaveButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);


        tipText.text = "<color=red>请先选择文件!!</color>";
    }

    private void Start()
    {
        filepath = FilePathMgr.allPathData.Tables[0].Rows[5][1].ToString();
        if(!filepath.Equals(""))
        {
            tipText.text = "当前路径为" + filepath;
            thread = new Thread(new ThreadStart(delegate ()
            {
                //VersionSyncPanel.ProcessCommand("cmd.exe", "svn update " + "\"" + filepath + "\"");
                xmldata = XmlHandler.XmlReadByText(filepath);
                tip = filepath + "读取完毕,可以开始查找";
                Thread.Sleep(100);
                thread.Abort();
            }));
            thread.Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!tip.Equals(""))
        {
            tipText.text = tip;
            tip = "";
        }
        if(Screen.width>screenWidth)
        {
            screenWidth = Screen.width;
            keyScrollRect.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(screenWidth / 2, Screen.height - 200);
            valueScrollRect.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(screenWidth / 2, Screen.height - 200);
            keyGridLayoutGroup.cellSize = new Vector2(screenWidth / 2 - keyScrollRect.verticalScrollbar.GetComponent<RectTransform>().sizeDelta.x, 30);
            valueGridLayoutGroup.cellSize = new Vector2(screenWidth / 2 - valueScrollRect.verticalScrollbar.GetComponent<RectTransform>().sizeDelta.x, 30);
        }
    }

    private void OnSearchButtonClick()
    {
        if (filepath.Equals("")) { tipText.text = "<color=red>请先选择对应的文件!!</color>";return; }
        if (searchInputField.text.Trim().Equals("")) { tipText.text = "<color=red>请先输入对应的查找文字!!</color>"; return; }
        string searchStr = searchInputField.text.Trim();
        DestroyALLChild(keyContent);
        DestroyALLChild(valueContent);
        bool isSearchedKey = false;
        foreach (string key in xmldata.Keys)
        {
            if (key.ToLower().IndexOf(searchStr.ToLower()) >= 0)
            {
                isSearchedKey = true;
                GameObject keybtn = Instantiate(keyButton, keyContent.transform) as GameObject;
                keybtn.transform.Find("Text").GetComponent<Text>().text = key;
                keybtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    DestroyALLChild(valueContent);
                    ValueUIDic.Clear();
                    foreach (string artkey in xmldata[key].Keys)
                    {
                        GameObject valueui = Instantiate(ValueUI, valueContent.transform) as GameObject;
                        Text ArtName = valueui.transform.Find("ArtName").GetComponent<Text>();
                        ArtName.text = artkey;
                        InputField ArtValue = valueui.transform.Find("ArtValue").GetComponent<InputField>();
                        ArtValue.GetComponent<RectTransform>().sizeDelta = new Vector2(valueGridLayoutGroup.cellSize.x - ArtName.GetComponent<RectTransform>().sizeDelta.x, 0);
                        ArtValue.text = xmldata[key][artkey];
                        ArtValue.onEndEdit.AddListener(delegate (string str)
                        {
                            //if(artkey == "trunk")
                            //{
                            //    if(Regex.IsMatch(str, @"[\u4e00-\u9fa5]"))
                            //    {
                            //        xmldata[key]["zh_tw"] = ChineseStringUtility.ToTraditional(str);
                            //        ValueUIDic["zh_tw"].transform.Find("ArtValue").GetComponent<InputField>().text = ChineseStringUtility.ToTraditional(str);
                            //    }
                            //}
                            if (!xmldata[key][artkey].Equals(str))
                            {
                                xmldata[key][artkey] = str;
                                XmlHandler.TextSerialization(xmldata, filepath);
                                tipText.text = filepath + "修改已保存";
                            }
                        });
                        ValueUIDic.Add(artkey, valueui);
                    }
                });
            }
            else if(xmldata[key].ContainsKey("trunk")&& xmldata[key]["trunk"].ToLower().IndexOf(searchStr.ToLower())>=0)
            {
                isSearchedKey = true;
                GameObject keybtn = Instantiate(keyButton, keyContent.transform) as GameObject;
                keybtn.transform.Find("Text").GetComponent<Text>().text = key;
                keybtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    DestroyALLChild(valueContent);
                    ValueUIDic.Clear();
                    foreach (string artkey in xmldata[key].Keys)
                    {
                        GameObject valueui = Instantiate(ValueUI, valueContent.transform) as GameObject;
                        Text ArtName = valueui.transform.Find("ArtName").GetComponent<Text>();
                        ArtName.text = artkey;
                        InputField ArtValue = valueui.transform.Find("ArtValue").GetComponent<InputField>();
                        ArtValue.GetComponent<RectTransform>().sizeDelta = new Vector2(valueGridLayoutGroup.cellSize.x - ArtName.GetComponent<RectTransform>().sizeDelta.x, 0);
                        ArtValue.text = xmldata[key][artkey];
                        ArtValue.onEndEdit.AddListener(delegate (string str)
                        {
                            //if(artkey == "trunk")
                            //{
                            //    if(Regex.IsMatch(str, @"[\u4e00-\u9fa5]"))
                            //    {
                            //        xmldata[key]["zh_tw"] = ChineseStringUtility.ToTraditional(str);
                            //        ValueUIDic["zh_tw"].transform.Find("ArtValue").GetComponent<InputField>().text = ChineseStringUtility.ToTraditional(str);
                            //    }
                            //}
                            if (!xmldata[key][artkey].Equals(str))
                            {
                                xmldata[key][artkey] = str;
                                XmlHandler.TextSerialization(xmldata, filepath);
                                tipText.text = filepath + "修改已保存";
                            }
                        });
                        ValueUIDic.Add(artkey, valueui);
                    }
                });
            }
        }
        if (!isSearchedKey) tipText.text = "key:"+searchStr+"查找失败";
    }

    private void OnSaveButtonClick()
    {
        XmlHandler.TextSerialization(xmldata, filepath);
        tipText.text = filepath + "保存成功";
    }

    private void OnBackButtonClick()
    {
        filepath = "";
        tipText.text = "<color=red>请先选择文件!!</color>";
        ValueUIDic.Clear();
        xmldata.Clear();
        DestroyALLChild(keyContent);
        DestroyALLChild(valueContent);
        EventMgr.Broadcast(EventType.PanelBack);
    }

    private void DestroyALLChild(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Destroy(obj.transform.GetChild(i).gameObject);
        }
    }
}
