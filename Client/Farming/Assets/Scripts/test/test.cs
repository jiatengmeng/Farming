using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using Google.Protobuf;
using Test;
using System.IO;

[ProtoContract]
public class testdata
{
	[ProtoMember(1)]
	public List<double> double_lst;
	[ProtoMember(2)]
	public int double_lst_count;
	[ProtoMember(3)]
	public List<int> int_lst;
	[ProtoMember(4)]
	public int int_lst_count;
}
public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		testWriter1.Instance.convertXmlToBinary(@"E:\farming\Farming\Doc\Xml\PlantCfg.xml", @"E:\farming\Farming\Doc\Xml\PlantCfg.bin");
		//FileStream fs = new FileStream(@"E:\farming\Farming\Doc\Xml\PlantCfg.bin", FileMode.Open);
		/*byte[] data = File.ReadAllBytes(@"E:\farming\Farming\Doc\Xml\PlantCfg.bin");
		Config.PlantCfgSet plantCfgSet = ProtobufHelper.Instance.DeSerialize<Config.PlantCfgSet>(data);
		string lcc = ProtobufHelper.Instance.DeSerialize<string>(data);
		Debug.Log(lcc);
		Debug.Log(plantCfgSet.LCrcCode);
		foreach(var plantcfg in plantCfgSet.Plantcfgs)
        {
			Debug.Log(plantcfg.NId);
        }*/
		//testdata olddata = new testdata();
		//List<int> lst_int = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		//List<double> lst_double = new List<double>() { 0.1,0.2,0.3,0.4,0.5,0.6,0.7,0.8,0.9,1.0};
		//olddata.double_lst = lst_double;
		//olddata.double_lst_count = lst_double.Count;
		//olddata.int_lst = lst_int;
		//olddata.int_lst_count = lst_int.Count;
		//DataPackage data = new DataPackage();
		//data.RealCount = 10;
		//data.IntCount = 10;
		//IMessage message = new DataPackage();
		//DataPackage newdata = (DataPackage)message.Descriptor.Parser.ParseFrom(ProtobufHelper.Instance.Serialize(olddata));
		//for(int i=0;i<newdata.IntCount;i++)
		//      {
		//	Debug.Log(newdata.IntArray[i]);
		//      }
		//for (int i = 0; i < newdata.RealCount; i++)
		//{
		//	Debug.Log(newdata.RealArray[i]);
		//}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
