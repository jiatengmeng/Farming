using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelMgr : MonoBehaviour
{
    private Button xmlSearch;
    private Button xmlCompare;
    private Button excelsExport;
    private Button excelUpdate;
    private Button versionSync;
    private Button xmlInspect;
    private Button xmlModification;
    private Button XlsxToXml;
    private Button exitBtn;
    private void Awake()
    {
        xmlSearch = this.transform.Find("XmlSearch").GetComponent<Button>();
        xmlCompare = this.transform.Find("XmlCompare").GetComponent<Button>();
        excelsExport = this.transform.Find("ExcelsExport").GetComponent<Button>();
        excelUpdate = this.transform.Find("ExcelUpdate").GetComponent<Button>();
        versionSync = this.transform.Find("VersionSync").GetComponent<Button>();
        xmlInspect = this.transform.Find("XmlInspect").GetComponent<Button>();
        xmlModification = this.transform.Find("XmlModification").GetComponent<Button>();
        XlsxToXml = this.transform.Find("XlsxToXml").GetComponent<Button>();
        exitBtn = this.transform.Find("Exit").GetComponent<Button>();

        xmlSearch.onClick.AddListener(OnXmlSearchBtnClick);
        xmlCompare.onClick.AddListener(OnXmlCompareBtnClick);
        excelsExport.onClick.AddListener(OnExcelsExportBtnClick);
        excelUpdate.onClick.AddListener(OnExcelUpdateBtnClick);
        versionSync.onClick.AddListener(OnVersionSyncBtnClick);
        xmlInspect.onClick.AddListener(OnXmlInspectBtnClick);
        xmlModification.onClick.AddListener(OnXmlModificationBtnClick);
        XlsxToXml.onClick.AddListener(OnXlsxToXmlClick);
        exitBtn.onClick.AddListener(delegate () { Application.Quit(); });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnXmlSearchBtnClick()
    {
        EventMgr.Broadcast(EventType.XmlSearchPanelDisplay);
    }
    private void OnXmlCompareBtnClick()
    {
        EventMgr.Broadcast(EventType.XmlComparePanelDisplay);
    }
    private void OnExcelsExportBtnClick()
    {
        EventMgr.Broadcast(EventType.ExcelsExportPanelDisplay);
    }
    private void OnExcelUpdateBtnClick()
    {

        EventMgr.Broadcast(EventType.ExcelUpdatePanelDisplay);
    }
    private void OnVersionSyncBtnClick()
    {
        EventMgr.Broadcast(EventType.VersionSyncPanelDisplay);
    }
    private void OnXmlInspectBtnClick()
    {
        EventMgr.Broadcast(EventType.XmlInspectPanelDisplay);

    }
    private void OnXmlModificationBtnClick()
    {
        EventMgr.Broadcast(EventType.XmlModificationPanelDisplay);
    }
    private void OnXlsxToXmlClick()
    {
        EventMgr.Broadcast(EventType.XlsxToXmlPanelDisplay);
    }

}
