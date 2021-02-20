using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class testWriter1:SingleTon<testWriter1>
{
    public bool convertXmlToBinary(string xmlFile,string binaryFile, bool verbose = false)
    {
        if(verbose)
        {
            Debug.Log("packing"+binaryFile+"...");
        }

        XmlDocument doc = XmlHandler.XmlReader2(xmlFile);
        if(doc==null)
        {
            return false;
        }
        XmlNode ExData = doc.SelectSingleNode("ExData");
        XmlNode table = ExData.SelectSingleNode("Table");
        Config.PlantCfgSet plantCfgSet = new Config.PlantCfgSet();

        Dictionary<string, XmlNode> attributeDic = new Dictionary<string, XmlNode>();
        foreach(XmlNode node in table.SelectNodes("Attribute"))
        {
            attributeDic.Add(node.Attributes["Name"].Value, node);
        }

        {
            if(attributeDic.ContainsKey("l_crc_code"))
            {
                plantCfgSet.LCrcCode = long.Parse(attributeDic["l_crc_code"].Attributes["Value"].Value);
            }
        }

        foreach (XmlNode node in table.SelectNodes("Record"))
        {
            Config.PlantCfgSet.PlantCfg record = new Config.PlantCfgSet.PlantCfg();
            record.NId = node.Attributes["n_id"].ToString().ToInt();
            record.SName = node.Attributes["s_name"].ToString();
            record.NRarity = node.Attributes["n_rarity"].ToString().ToInt();
            record.SModel = node.Attributes["s_model"].ToString();
            record.NTimes = node.Attributes["n_times"].ToString().ToInt();
            record.NProperty = node.Attributes["n_property"].ToString().ToInt();
            record.SDes = node.Attributes["s_des"].ToString();
            plantCfgSet.Plantcfgs.Add(record);
        }
        byte[] data = ProtobufHelper.Instance.Serialize(plantCfgSet);
        if(data==null)
        {
            return false;
        }

        FileStream file = File.Create(binaryFile);
        file.Write(data, 0, data.Length);
        return true;
    }
}
