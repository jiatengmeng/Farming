using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class FilePathMgr : MonoBehaviour
{
    public static DataSet allPathData = new DataSet(); 
    private static string path = Environment.CurrentDirectory+"\\ToolLogPath\\" + "SavePath.xlsx";
    public static string xmlupdatafilepath = Environment.CurrentDirectory + "\\ToolLogPath\\" + "XmlUpdataFolder";
    public static string synctextfilepath = Environment.CurrentDirectory + "\\ToolLogPath\\" + "SyncTextLogFolder";
    public static string xyncexcelfilepath = Environment.CurrentDirectory + "\\ToolLogPath\\" + "SyncExcelLogFolder";
    public static string xmlcheckfilepath = Environment.CurrentDirectory + "\\ToolLogPath\\" + "XmlCheckLogFolder";
    public static string excelexprotfilepath = Environment.CurrentDirectory + "\\ToolLogPath\\" + "ExcelExprotFolder";
    public static string xmlsearchfilepath = Environment.CurrentDirectory + "\\ToolLogPath\\" + "XmlSearchFolder";
    public static string syncVersionLogFolder = Environment.CurrentDirectory + "\\ToolLogPath\\" + "SyncVersionLogFolder";
    public static string compareLogFolder = Environment.CurrentDirectory + "\\ToolLogPath\\" + "CompareLogFolder";
    public static string errorLogFolder = Environment.CurrentDirectory + "\\ToolLogPath\\" + "ErrorLogFolder";
    private void Awake()
    {
        CreateFileFolder();
        ReadFilePath();
    }

    private void ReadFilePath()
    {
        Debug.Log(path);
        //保存方式为第一排是页面名+属性名：例如 xmlsyncpanel_targetfile_1_name

        if (File.Exists(path))
        {
            allPathData = ExcelHandler.ReadByNPOI(path);
        }
        else
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Panel_Name");
            dataTable.Columns.Add("targetfile1");
            dataTable.Columns.Add("targetfile2");
            dataTable.Columns.Add("targetfile3");
            DataRow row = dataTable.NewRow();
            row[0] = "title";
            dataTable.Rows.Add(row);
            allPathData.Tables.Add(dataTable);
            dataTable.Rows[0][0] = "页面";
            dataTable.Rows[0][1] = "目标文件1";
            dataTable.Rows[0][2] = "目标文件2";
            dataTable.Rows[0][3] = "目标文件3";

            dataTable.Rows.Add("XmlSearchPanel_targetfile");
            dataTable.Rows[1][0] = "检索页面";
            dataTable.Rows[1][1] = "";
            dataTable.Rows[1][2] = "";
            dataTable.Rows[1][3] = "";
            dataTable.Rows.Add("ExcelExportPanel_targetfile");
            dataTable.Rows[2][0] = "批量导出页面";
            dataTable.Rows[2][1] = "";
            dataTable.Rows[2][2] = "";
            dataTable.Rows[2][3] = "";
            dataTable.Rows.Add("VersionSyncPanel_targetfile");
            dataTable.Rows[3][0] = "版本同步页面";
            dataTable.Rows[3][1] = "";
            dataTable.Rows[3][2] = "";
            dataTable.Rows[3][3] = "";
            dataTable.Rows.Add("XmlInspectPanel");
            dataTable.Rows[4][0] = "检查页面";
            dataTable.Rows[4][1] = "";
            dataTable.Rows[4][2] = "";
            dataTable.Rows[4][3] = "";
            dataTable.Rows.Add("XmlModificationPanel");
            dataTable.Rows[5][0] = "i18n修改页面";
            dataTable.Rows[5][1] = "";
            dataTable.Rows[5][2] = "";
            dataTable.Rows[5][3] = "";
            dataTable.Rows.Add("XmlComparePanel");
            dataTable.Rows[6][0] = "i18n对比页面";
            dataTable.Rows[6][1] = "";
            dataTable.Rows[6][2] = "";
            dataTable.Rows[6][3] = ""; 
            dataTable.Rows.Add("XlsxToXmlanel");
            dataTable.Rows[7][0] = "表格转换工具";
            dataTable.Rows[7][1] = "";
            dataTable.Rows[7][2] = "";
            dataTable.Rows[7][3] = "";
            ExcelHandler.CreateExcel(allPathData, path);
        }
    }
    public static void savedata()
    {
        ExcelHandler.CreateExcel(allPathData, path);
    }

    private void CreateFileFolder()
    {
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "XmlUpdataFolder");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "SyncTextLogFolder");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "SyncExcelLogFolder");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "XmlCheckLogFolder");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "ExcelExprotFolder");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "XmlSearchFolder");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "SyncVersionLogFolder");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "CompareLogFolder");
        Directory.CreateDirectory(Environment.CurrentDirectory + "\\ToolLogPath\\" + "ErrorLogFolder");
        Debug.Log("文件创建完成");
    }
}
