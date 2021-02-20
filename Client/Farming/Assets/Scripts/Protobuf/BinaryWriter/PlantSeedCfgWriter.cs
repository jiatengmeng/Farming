using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class PlantSeedCfgWriter : SingleTon<PlantSeedCfgWriter>
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
        Config.PlantSeedCfgSet records = new Config.PlantSeedCfgSet();

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
            Config.PlantSeedCfgSet.PlantSeedCfg record = new Config.PlantSeedCfgSet.PlantSeedCfg();
            record.NId = node.Attributes["n_id"].ToString().ToInt();
            record.SName = node.Attributes["s_name"].ToString();
            record.NRarity = node.Attributes["n_rarity"].ToString().ToInt();
            record.SImage = node.Attributes["s_image"].ToString();
            record.SDes = node.Attributes["s_des"].ToString();
            record.NPlantid = node.Attributes["n_plantid"].ToString().ToInt();

            records.Plantseedcfgs.Add(record);
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
