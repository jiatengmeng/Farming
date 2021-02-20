using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class PlantPropertyCfgWriter : SingleTon<PlantPropertyCfgWriter>
{
    public bool convertXmlToBinary(string xmlFile, string binaryFile, bool verbose = false)
    {
        if (verbose)
        {
            Debug.Log("packing" + binaryFile + "...");
        }

        XmlDocument doc = XmlHandler.XmlReader2(xmlFile);
        if (doc == null)
        {
            return false;
        }
        XmlNode ExData = doc.SelectSingleNode("ExData");
        XmlNode table = ExData.SelectSingleNode("Table");
        Config.PlantPropertyCfgSet records = new Config.PlantPropertyCfgSet();

        Dictionary<string, XmlNode> attributeDic = new Dictionary<string, XmlNode>();
        foreach (XmlNode node in table.SelectNodes("Attribute"))
        {
            attributeDic.Add(node.Attributes["Name"].Value, node);
        }

        {
            if (attributeDic.ContainsKey("l_crc_code"))
            {
                records.LCrcCode = long.Parse(attributeDic["l_crc_code"].Attributes["Value"].Value);
            }
        }


        foreach (XmlNode node in table.SelectNodes("Record"))
        {
            Config.PlantPropertyCfgSet.PlantPropertyCfg record = new Config.PlantPropertyCfgSet.PlantPropertyCfg();
            record.NId = node.Attributes["n_id"].ToString().ToInt();
            record.SName = node.Attributes["s_name"].ToString();

            records.Plantpropertycfgs.Add(record);
        }
        byte[] data = ProtobufHelper.Instance.Serialize(records);
        if (data == null)
        {
            return false;
        }

        FileStream file = File.Create(binaryFile);
        file.Write(data, 0, data.Length);
        return true;
    }
}
