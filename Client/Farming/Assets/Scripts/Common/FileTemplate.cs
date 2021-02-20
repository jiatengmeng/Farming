using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileTemplate : SingleTon<FileTemplate>
{
    public string protoTemplate = "syntax = \"proto2\";\n\n" +
        "package config;\n\n" +
        "option optimize_for = LITE_RUNTIME;\n\n" +
        "message ${TABLE_NAME}\n" +
        "{\n" +
        "\tmessage ${RECORD_NAME}\n" +
        "\t{\n" +
        "${RECORD_LIST}\n" +
        "\t}\n" +
        "\trepeated ${RECORD_NAME} ${RECORDS_VAR_NAME} = 1;\n" +
        "${ATTRIBUTE_LIST}\n" +
        "}";

    public string binaryFileWriter =
        @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class ${RECORD_NAME}Writer : SingleTon<${RECORD_NAME}Writer>
{
    public bool convertXmlToBinary(string xmlFile, string binaryFile, bool verbose = false)
    {
        if (verbose)
        {
            Debug.Log(""packing"" + binaryFile + ""..."");
        }

        XmlDocument doc = XmlHandler.XmlReader2(xmlFile);
        if (doc == null)
        {
            return false;
        }
        XmlNode ExData = doc.SelectSingleNode(""ExData"");
        XmlNode table = ExData.SelectSingleNode(""Table"");
        Config.${RECORD_NAME}Set records = new Config.${RECORD_NAME}Set();

        Dictionary<string, XmlNode> attributeDic = new Dictionary<string, XmlNode>();
        foreach (XmlNode node in table.SelectNodes(""Attribute""))
        {
            attributeDic.Add(node.Attributes[""Name""].Value, node);
        }

        {
            if(attributeDic.ContainsKey(""l_crc_code""))
            {
                records.LCrcCode = long.Parse(attributeDic[""l_crc_code""].Attributes[""Value""].Value);
             }
        }
${ATTRIBUTE_SETTER}

        foreach (XmlNode node in table.SelectNodes(""Record""))
        {
            Config.${RECORD_NAME}Set.${RECORD_NAME} record = new Config.${RECORD_NAME}Set.${RECORD_NAME}();
${RECORD_PROPERTY_SETTER}
            records.${TABLE_NAME_FIRST_UPPER}s.Add(record);
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
";
}
