using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHandler : SingleTon<FileHandler>
{
    public bool XmlToBinary()
    {
        Debug.Log(GlobeInit.Instance.ObjPath);
        if (!PlantCfgWriter.Instance.convertXmlToBinary(GlobeInit.Instance.ObjPath + "/GameData/Resource/Xml/PlantCfg.xml", GlobeInit.Instance.ObjPath + "/GameData/Resource/Xml/PlantCfg.bin", true)) return false;
        if (!PlantPropertyCfgWriter.Instance.convertXmlToBinary(GlobeInit.Instance.ObjPath + "/GameData/Resource/Xml/PlantPropertyCfg.xml", GlobeInit.Instance.ObjPath + "/GameData/Resource/Xml/PlantPropertyCfg.bin", true)) return false;
        if (!PlantSeedCfgWriter.Instance.convertXmlToBinary(GlobeInit.Instance.ObjPath + "/GameData/Resource/Xml/PlantSeedCfg.xml", GlobeInit.Instance.ObjPath + "/GameData/Resource/Xml/PlantSeedCfg.bin", true)) return false;
        return true;
    }
    public T BinaryFileSerialize<T>(string path)
    {
        path = GlobeInit.Instance.ObjPath + "/GameData/Resource/Xml/" + path;
        byte[] data = File.ReadAllBytes(path);
        T result = ProtobufHelper.Instance.DeSerialize<T>(data);
        return result;
    }
}
