using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PanelMgr : MonoBehaviour
{
    //对应界面
    private GameObject mainPanel;
    private GameObject xmlSearchPanel;
    private GameObject xmlSyncPanel;
    private GameObject excelsExportPanel;
    //private GameObject excelUpdatePanel;
    private GameObject versionSyncPanel;
    private GameObject XmlInspectPanel;
    private GameObject XmlModificationPanel;
    private GameObject TipPanel;
    private GameObject xmlComparePanel;
    private GameObject XlsxToXmlPanel;

    //界面管理栈
    private Stack<GameObject> panelStk = new Stack<GameObject>();
    private bool tipPanelDisplay = false;
    string tip = "";
    string openFilePath = "";
    //string testpath = @"D:\Users\Administrator\Desktop\i18n.xml";
    private void Awake()
    {
        //获取实例
        mainPanel = this.transform.Find("MainPanel").gameObject;
        xmlSearchPanel = this.transform.Find("XmlSearchPanel").gameObject;
        xmlSyncPanel = this.transform.Find("XmlSyncPanel").gameObject;
        excelsExportPanel = this.transform.Find("ExcelsExportPanel").gameObject;
       // excelUpdatePanel = this.transform.Find("ExcelUpdatePanel").gameObject;
        versionSyncPanel = this.transform.Find("VersionSyncPanel").gameObject;
        XmlInspectPanel = this.transform.Find("XmlInspectPanel").gameObject;
        XmlModificationPanel = this.transform.Find("XmlModificationPanel").gameObject;
        xmlComparePanel = this.transform.Find("XmlComparePanel").gameObject;
        TipPanel = this.transform.Find("TipPanel").gameObject;
        XlsxToXmlPanel = this.transform.Find("XlsxToXmlPanel").gameObject;
        //设置显示界面
        SetPanelActive();
        mainPanel.SetActive(true);
        panelStk.Push(mainPanel);
        //添加事件监听
        EventMgr.AddListener(EventType.XmlSearchPanelDisplay, XmlSearchPanelDisplay);
        EventMgr.AddListener(EventType.XmlSyncPanelDisplay, XmlSyncPanelDisplay);
        EventMgr.AddListener(EventType.ExcelsExportPanelDisplay, ExcelsExportPanelDisplay);
        EventMgr.AddListener(EventType.ExcelUpdatePanelDisplay, ExcelUpdatePanelDisplay);
        EventMgr.AddListener(EventType.VersionSyncPanelDisplay, VersionSyncPanelDisplay);
        EventMgr.AddListener(EventType.XmlInspectPanelDisplay, XmlInspectPanelDisplay);
        EventMgr.AddListener(EventType.XmlModificationPanelDisplay, XmlModificationPanelDisplay);
        EventMgr.AddListener(EventType.XmlComparePanelDisplay, XmlComparePanelDisplay);
        EventMgr.AddListener(EventType.XlsxToXmlPanelDisplay, XlsxToXmlPanelDisplay);
        EventMgr.AddListener<string,string>(EventType.TipPanelDisplay, TipPanelDisplay);
        EventMgr.AddListener(EventType.PanelBack, HidePanel);
    }

   

    Thread thread;
    // Start is called before the first frame update
    void Start()
    {
        //string str = "&lt;font color=&apos;ui.grade_yellow&apos;&gt;{0}&lt;/font&gt; invites you to join &lt;font color=&apos;ui.grade_yellow&apos;&gt;{1}&lt;/font&gt;, do you join?";
        //Debug.Log(XmlHandler.XmlString(str, true));
        //Debug.Log(str);
    }
    // Update is called once per frame
    void Update()
    {
        if (tipPanelDisplay)
        {
            tipPanelDisplay = false;
            TipPanel.transform.Find("TipBackground").Find("TipScrollView").GetComponent<ScrollRect>().content.transform.Find("TipText").GetComponent<Text>().text = tip;
            Button open = TipPanel.transform.Find("TipBackground").Find("OpenFile").GetComponent<Button>();
            if (openFilePath!=null)
            {
                open.gameObject.SetActive(true);
                open.onClick.AddListener(() =>
                {
                    OpenFileByWin32.OpenFolder(openFilePath);
                });
            }
            else
            {
                open.gameObject.SetActive(false);
            }
            TipPanel.SetActive(true); 
        }
    }

    private void OnDestroy()
    {
        //结束移除对应事件
        EventMgr.RemoveListener(EventType.XmlSearchPanelDisplay, XmlSearchPanelDisplay);
        EventMgr.RemoveListener(EventType.XmlSyncPanelDisplay, XmlSyncPanelDisplay);
        EventMgr.RemoveListener(EventType.ExcelsExportPanelDisplay, ExcelsExportPanelDisplay);
        EventMgr.RemoveListener(EventType.ExcelUpdatePanelDisplay, ExcelUpdatePanelDisplay);
        EventMgr.RemoveListener(EventType.VersionSyncPanelDisplay, VersionSyncPanelDisplay);
        EventMgr.RemoveListener(EventType.XmlInspectPanelDisplay, XmlInspectPanelDisplay);
        EventMgr.RemoveListener(EventType.XlsxToXmlPanelDisplay, XlsxToXmlPanelDisplay);
        EventMgr.RemoveListener<string,string>(EventType.TipPanelDisplay, TipPanelDisplay);
        EventMgr.RemoveListener(EventType.PanelBack, HidePanel);
    }

    private void SetPanelActive()
    {
        xmlSearchPanel.SetActive(false);
        xmlSyncPanel.SetActive(false);
        excelsExportPanel.SetActive(false);
        //excelUpdatePanel.SetActive(false);
        versionSyncPanel.SetActive(false);
        XmlInspectPanel.SetActive(false);
        TipPanel.SetActive(false);
        XmlModificationPanel.SetActive(false);
        xmlComparePanel.SetActive(false);
        XlsxToXmlPanel.SetActive(false);
    }
    //对应界面显示时同时将其加入栈中
    private void XmlSearchPanelDisplay()
    {
        SetPanelActive();
        xmlSearchPanel.SetActive(true);
        panelStk.Push(xmlSearchPanel);
    }

    private void XmlSyncPanelDisplay()
    {
        SetPanelActive();
        xmlSyncPanel.SetActive(true);
        panelStk.Push(xmlSyncPanel);
    }

    private void ExcelsExportPanelDisplay()
    {
        SetPanelActive();
        excelsExportPanel.SetActive(true);
        panelStk.Push(excelsExportPanel);
    }


    private void ExcelUpdatePanelDisplay()
    {
        SetPanelActive();
        //excelUpdatePanel.SetActive(true);
        //panelStk.Push(excelUpdatePanel);
    }

    private void VersionSyncPanelDisplay()
    {
        SetPanelActive();
        versionSyncPanel.SetActive(true);
        panelStk.Push(versionSyncPanel);
    }
    private void XmlInspectPanelDisplay()
    {
        SetPanelActive();
        XmlInspectPanel.SetActive(true);
        panelStk.Push(XmlInspectPanel);
    }

    private void XmlModificationPanelDisplay()
    {
        SetPanelActive();
        XmlModificationPanel.SetActive(true);
        panelStk.Push(XmlModificationPanel);
    }

    private void TipPanelDisplay(string tipstr,string filePath = null)
    {
        tipPanelDisplay = true;
        tip = tipstr;
        openFilePath = filePath;
    }
    private void XmlComparePanelDisplay()
    {
        SetPanelActive();
        xmlComparePanel.SetActive(true);
        panelStk.Push(xmlComparePanel);
    }
    private void XlsxToXmlPanelDisplay()
    {
        SetPanelActive();
        XlsxToXmlPanel.SetActive(true);
        panelStk.Push(XlsxToXmlPanel);
    }

    //返回按钮用于将之前显示的界面隐藏
    private void HidePanel()
    {
        if (panelStk.Count != 0)
        {
            GameObject hideObj = panelStk.Pop();
            hideObj.SetActive(false);
            GameObject displayObj = panelStk.Peek();
            displayObj.SetActive(true);
        }
    }
}
