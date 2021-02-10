using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcelData
{
    public ExcelData()
    {
        tableName = "";
        artDic = new Dictionary<string, string>();
        keyColDic = new Dictionary<int, string>();
        dataDic = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// 表格名称
    /// </summary>
    public string tableName;
    /// <summary>
    /// 全局属性列表
    /// </summary>
    public Dictionary<string, string> artDic;
    /// <summary>
    /// 对应key所在行的字典
    /// </summary>
    public Dictionary<int/*col*/, string/*key*/> keyColDic;
    /// <summary>
    /// 数据字典
    /// </summary>
    public Dictionary<string/**/, List<string>> dataDic;
}
