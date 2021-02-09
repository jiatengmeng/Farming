using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excel;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Xml;
using System;
using NPOI.HSSF.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;

public class ExcelHandler
{
    
    public static DataSet ReadByNPOI(string filePath)
    {
        DataSet dt = new DataSet();
        FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read); 
        ISheet sheet=null;
        try
        {
            HSSFWorkbook wk = new HSSFWorkbook(fs);
            sheet = wk.GetSheetAt(0);
            Debug.Log("Excel2007+");
        }
        catch(Exception e)
        {
            Debug.Log(e);
            fs.Dispose();
            fs = File.Open(filePath, FileMode.Open, FileAccess.Read);
            XSSFWorkbook wk = new XSSFWorkbook(fs); 
            sheet = wk.GetSheetAt(0);
            Debug.Log("Excel2003-2007");
        }
        if(sheet==null)
        {
            Debug.Log("读取失败草了");
            return null;
        }

        DataTable dataTable = new DataTable();
        IRow row = sheet.GetRow(0);
        int col = row.Cells.Count;
        for (int i = 0; i < col; i++)
        {
            dataTable.Columns.Add();
        }
        DataRow dataRow = dataTable.NewRow();
        dataRow[0] = "title";
        dataTable.Rows.Add(dataRow);
        dt.Tables.Add(dataTable);
        int rowindex = 0;
        while (row != null)
        {
            if(rowindex!=0)
            {
                dataTable.Rows.Add(rowindex);
            }
            for (int i = 0; i < col; i++)
            {
                if (row.GetCell(i) != null)
                {
                    dataTable.Rows[rowindex][i] = row.GetCell(i).ToString();
                }
                else
                {
                    dataTable.Rows[rowindex][i] = "";
                }
            }
            rowindex++;
            row = sheet.GetRow(rowindex);
        }

        return dt;
    }

    public static DataSet ReadExcelSheet(string filePath)
    {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

        DataSet result = excelReader.AsDataSet();
        //Tables[0] 下标0表示excel文件中第一张表的数据
        //int columnNum = result.Tables[0].Columns.Count;
        //int rowNum = result.Tables[0].Rows.Count;
        //for(int i = 0;i<rowNum;i++)
        //{
        //    for(int j=0;j<columnNum;j++)
        //    {
        //        string eValue = result.Tables[0].Rows[i][j].ToString();
        //        Debug.Log(eValue);
        //    }
        //}
        if(result!=null)
        {
            Debug.Log("读取成功");
        }
        else
        {
            Debug.Log("完全无法读取Excel？？？");
        }
        excelReader.Close();
        stream.Close();
        stream.Dispose();
        return result;
        //FileStream fs = File.OpenRead(filePath);
        //HSSFWorkbook wk = new HSSFWorkbook(fs);
        //return wk;
    }

    public static void CreateExcel(DataSet dataSet, string filePath)
    {
        Debug.Log(filePath);
        HSSFWorkbook wk = new HSSFWorkbook();
        IFont redFont = wk.CreateFont();
        redFont.FontHeightInPoints = 12; // 字体高度  
        redFont.FontName = "宋体"; // 字体
        redFont.Color = HSSFColor.Red.Index;
        for (int n = 0; n < dataSet.Tables.Count; n++)
        {
            Debug.Log("Tables"+n);
            ISheet sheet = wk.CreateSheet("sheet"+n);
            sheet.FitToPage = true;
            int columnNum = dataSet.Tables[n].Columns.Count;
            int rowNum = dataSet.Tables[n].Rows.Count;
            Debug.Log(rowNum);
            Debug.Log(columnNum);
            IRow row;
            for (int i = 0; i < rowNum; i++)
            {
                row = sheet.CreateRow(i);
                for (int j = 0; j < columnNum; j++)
                {
                    sheet.AutoSizeColumn(j);
                    string cellValue = dataSet.Tables[n].Rows[i][j].ToString();
                    ICell cell = row.CreateCell(j);
                    if (cellValue.Length >= 3 && cellValue.Substring(0, 3) == "RED")
                    {
                        ICellStyle cellStyle = wk.CreateCellStyle();
                        cellStyle.SetFont(redFont);
                        //XSSFRichTextString ts = new XSSFRichTextString(cellValue.Substring(3,cellValue.Length-3));
                        //ts.ApplyFont(redFont);
                        //cell.CellStyle = cellStyle;
                        cell.SetCellValue(cellValue.Substring(3, cellValue.Length - 3));
                        cell.CellStyle = cellStyle;
                    }
                    else cell.SetCellValue(cellValue);
                }
            }
        }
        if (File.Exists(filePath)) File.Delete(filePath);
        FileStream fs = File.Create(filePath);
        wk.Write(fs);
        wk.Close();
        fs.Close();
        fs.Dispose();
        Debug.Log("创建表格成功");
    }
    public static void CreateExcel(Dictionary<string ,Dictionary<string,string>> xmlData, string filePath)
    {
        Debug.Log(filePath);
        HSSFWorkbook wk = new HSSFWorkbook();
        IFont redFont = wk.CreateFont();
        redFont.FontHeightInPoints = 12; // 字体高度  
        redFont.FontName = "宋体"; // 字体
        redFont.Color = HSSFColor.Red.Index;
        ISheet sheet = wk.CreateSheet("sheet");
        sheet.FitToPage = true;
        IRow row = sheet.CreateRow(0);
        int colId = 0;
        int rowId = 1;
        foreach (var key in xmlData.Keys)
        {
            foreach (var art in xmlData[key].Keys)
            {
                ICell cell = row.CreateCell(colId);
                cell.SetCellValue(art);
                colId++;
            }
            break;
        }

        foreach (var key in xmlData.Keys)
        {
            Debug.Log(rowId);
            row = sheet.CreateRow(rowId);
            int j = 0;
            foreach (var art in xmlData[key].Keys)
            {
                sheet.AutoSizeColumn(j);
                ICell cell = row.CreateCell(j);
                if (!xmlData[key][art].Equals("")) cell.SetCellValue(xmlData[key][art]);
                j++;
            }
            rowId++;
        }
        if (File.Exists(filePath)) File.Delete(filePath);
        FileStream fs = File.Create(filePath);
        wk.Write(fs);
        wk.Close();
        fs.Close();
        fs.Dispose();
        Debug.Log("创建表格成功");
    }
    public static void CreateExcel(XmlNode xdoc,string createPath)
    {
        HSSFWorkbook wk = new HSSFWorkbook();
        wk.UnwriteProtectWorkbook();
        ISheet sheet = wk.CreateSheet("result");
        XmlNode root = xdoc;
        int ColNum = root.ChildNodes[0].Attributes.Count;
        int RowNum = root.ChildNodes.Count;
        IRow row = sheet.CreateRow(0);
        for(int i=0;i<ColNum;i++)
        {
            ICell cell = row.CreateCell(i);
            cell.SetCellValue(root.ChildNodes[0].Attributes[i].Name);
        }
        for(int i=0;i<RowNum;i++)
        {
            row = sheet.CreateRow(i+1);
            for (int j =0;j<ColNum;j++)
            {
                sheet.AutoSizeColumn(j);
                ICell cell = row.CreateCell(j);
                if(j==0)
                {
                    cell.SetCellValue(root.ChildNodes[i].Attributes[j].Value);
                }
                else if (!root.ChildNodes[i].Attributes[j].Value.Equals(""))
                {
                    cell.SetCellValue("√");
                }
            }
        }
        if (File.Exists(createPath)) File.Delete(createPath);
        FileStream fs = File.Create(createPath);
        wk.Write(fs);
        wk.Close();
        fs.Close();
        fs.Dispose();
        Debug.Log("创建表格成功");
    }
    public static void CreateExcel(Dictionary<string, List<int>> keyNodeIndex, XmlNode xdoc, string createPath)
    {
        HSSFWorkbook wk = new HSSFWorkbook();
        foreach (string key in keyNodeIndex.Keys)
        {
            ISheet sheet = wk.CreateSheet(key);
            XmlNode root = xdoc;
            List<int> nodeID = keyNodeIndex[key];
            int ColNum = root.ChildNodes[0].Attributes.Count;
            int RowNum = Math.Min(500,nodeID.Count);
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("行号");
            for (int i = 0; i < ColNum; i++)
            {
                ICell cell = row.CreateCell(i+1);
                cell.SetCellValue(root.ChildNodes[0].Attributes[i].Name);
            }
            for (int i = 0; i < RowNum; i++)
            {
                row = sheet.CreateRow(i + 1);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(nodeID[i] + 3);
                for (int j = 0; j < ColNum; j++)
                {
                    sheet.AutoSizeColumn(j + 1);
                    cell = row.CreateCell(j + 1);
                    if (!root.ChildNodes[nodeID[i]].Attributes[j].Value.Equals(""))
                    {
                        cell.SetCellValue(System.Security.SecurityElement.Escape(root.ChildNodes[nodeID[i]].Attributes[j].Value).Replace("\n", "&#x0A;"));
                    }
                }
            }
        }
        if (File.Exists(createPath)) File.Delete(createPath);
        FileStream fs = File.Create(createPath);
        wk.Write(fs);
        wk.Close();
        fs.Close();
        fs.Dispose();
        Debug.Log("创建表格成功");
    }

    public static void CreateExcel(DataSet dataSet, string filePath, Dictionary<string, int> langueDic)
    {
        HSSFWorkbook wk = new HSSFWorkbook();
        ISheet sheet = wk.CreateSheet("result");
        int columnNum = dataSet.Tables[0].Columns.Count;
        int rowNum = dataSet.Tables[0].Rows.Count;
        IRow row = sheet.CreateRow(0);
        ICell cell;
        for (int i=0;i<columnNum;i++)
        {
            cell = row.CreateCell(i);
            cell.SetCellValue(dataSet.Tables[0].Rows[0][i].ToString());
        }
        //_dataSet.Tables[0].Rows[0][columnNum] = "处理结果";
        //_dataSet.Tables[0].Rows[0][columnNum + 1] = "key";
        //_dataSet.Tables[0].Rows[0][columnNum + 2] = "trunk";
        cell = row.CreateCell(columnNum);
        cell.SetCellValue("处理结果");
        cell = row.CreateCell(columnNum+1);
        cell.SetCellValue("key");
        cell = row.CreateCell(columnNum+2);
        cell.SetCellValue("trunk");
        for (int i = 0; i < langueDic.Count; i++)
        {
            cell = row.CreateCell(columnNum + 3+i);
            cell.SetCellValue(dataSet.Tables[0].Rows[0][1 + i].ToString() + "翻译");
        }
        for (int i = 1; i < rowNum; i++)
        {
            row = sheet.CreateRow(i);
            for (int j = 0; j < columnNum; j++)
            {
                cell = row.CreateCell(j);
                cell.SetCellValue(dataSet.Tables[0].Rows[i][j].ToString());
            }
        }
        if (File.Exists(filePath)) File.Delete(filePath);
        FileStream fs = File.Create(filePath);
        wk.Write(fs);
        wk.Close();
        fs.Close();
        fs.Dispose();
        Debug.Log("创建表格成功");
        //dataSet = ReadExcelSheet(filePath);
    }
}
