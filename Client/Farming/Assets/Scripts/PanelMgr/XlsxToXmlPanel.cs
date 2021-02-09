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
    //文件浏览按钮
    private Button fileBrowseBtn1;
    private Button fileBrowseBtn2;

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

        fileBrowseBtn1 = rfInputField.transform.Find("FileBrowseButton").GetComponent<Button>();
        fileBrowseBtn2 = tf1InputField.transform.Find("FileBrowseButton").GetComponent<Button>();

        resText = this.transform.Find("ResultTextBackground").Find("ResultText").GetComponent<Text>();

        tipPanel = this.transform.Find("TipPanel").gameObject;

        fileBrowseBtn1.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.Browse(new StringBuilder(rfInputField.text).ToString());
            if (!filename.Equals(""))
            {
                rfInputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[6][1] = rfInputField.text;
                FilePathMgr.savedata();
            }
        });
        fileBrowseBtn2.onClick.AddListener(delegate () {
            string filename = OpenFileByWin32.Browse(new StringBuilder(tf1InputField.text).ToString());
            Debug.Log(filename);
            if (!filename.Equals(""))
            {
                tf1InputField.text = filename;
                FilePathMgr.allPathData.Tables[0].Rows[6][2] = tf1InputField.text;
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
        rfInputField.text = FilePathMgr.allPathData.Tables[0].Rows[6][1].ToString();
        tf1InputField.text = FilePathMgr.allPathData.Tables[0].Rows[6][2].ToString();
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

    private void OntransBtnClick()
    {
        if (rfInputField.text.Trim().Equals(""))
        {
            EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, "源文件夹为名空！！", null);
            return;
        }
        if (tf1InputField.text.Trim().Equals(""))
        {
            EventMgr.Broadcast<string, string>(EventType.TipPanelDisplay, "目标文件夹名为空！！", null);
            return;
        }
        tipDisplay = true;
        if(File.Exists(rfInputField.text))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(rfInputField.text);
            FileInfo[] files = directoryInfo.GetFiles("*.xlsx");
            for(int i = 0;i<files.Length;i++)
            {
                XlsxToXml(files[i]);
            }
        }
        
        tipDisplay = false;
        Thread.Sleep(100);
        thread.Abort();
    }

    private void XlsxToXml(FileInfo file)
    {
        DataSet exceldata = ExcelHandler.ReadByNPOI(file.FullName);
        int rowCount = exceldata.Tables[0].Rows.Count;
        for(int i = 0;i<rowCount;i++)
        {
            string rowHead = exceldata.Tables[0].Rows[i][0].ToString();
            if (rowHead[0] == '#') continue;
        }
    }

}
