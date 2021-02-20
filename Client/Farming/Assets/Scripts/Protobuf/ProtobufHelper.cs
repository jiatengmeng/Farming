using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using ProtoBuf;
using Google.Protobuf;

public class ProtobufHelper : SingleTon<ProtobufHelper>
{
	/// <summary>
	/// 将消息序列化为二进制的方法
	/// </summary>
	/// < param name="model">要序列化的对象< /param>
	public byte[] Serialize<T>(T model)
	{
		try
		{
			//涉及格式转换，需要用到流，将二进制序列化到流中
			using (MemoryStream ms = new MemoryStream())
			{
				//使用ProtoBuf工具的序列化方法
				Serializer.Serialize<T>(ms, model);
				//定义二级制数组，保存序列化后的结果
				byte[] result = new byte[ms.Length];
				//将流的位置设为0，起始点
				ms.Position = 0;
				//将流中的内容读取到二进制数组中
				ms.Read(result, 0, result.Length);
				return result;
			}
		}
		catch (Exception ex)
		{
			Debug.Log("序列化失败: " + ex.ToString());
			return null;
		}
	}

	public byte[] Serialize(ProtoBuf.IExtensible data)
	{
		try
		{
			//涉及格式转换，需要用到流，将二进制序列化到流中
			using (MemoryStream ms = new MemoryStream())
			{
				//使用ProtoBuf工具的序列化方法
				Serializer.Serialize(ms, data);
				//定义二级制数组，保存序列化后的结果
				byte[] result = new byte[ms.Length];
				//将流的位置设为0，起始点
				ms.Position = 0;
				//将流中的内容读取到二进制数组中
				ms.Read(result, 0, result.Length);
				return result;
			}
		}
		catch (Exception ex)
		{
			Debug.Log("序列化失败: " + ex.ToString());
			return null;
		}
	}

	/// 将收到的消息反序列化成对象
	/// < returns>The serialize.< /returns>
	/// < param name="msg">收到的消息.</param>
	public T DeSerialize<T>(byte[] msg)
	{
		try
		{
			using (MemoryStream ms = new MemoryStream(msg))
			{
				//使用工具反序列化对象
				T result = Serializer.Deserialize<T>(ms);
				return result;
			}
		}
		catch (Exception ex)
		{
			Debug.Log("反序列化失败: " + ex.ToString());
			return default(T);
		}
	}
	/// <summary>
	/// 序列化Proto对象--弃用
	/// </summary>
	/// <param name="proto">需要序列化的proto</param>
	/// <returns>序列化后的二进制数组</returns>
	public byte[] SerializeProto(IMessage proto)
	{
		byte[] data = null;
		using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
		{
			data = proto.ToByteArray();
			//Serializer.Serialize(ms, proto);
			//if (ms.Length > 0)
			//{
			//	data = new byte[(int)ms.Length];
			//	ms.Seek(0, System.IO.SeekOrigin.Begin);
			//	ms.Read(data, 0, data.Length);
			//}
			//ms.Close();
		}
		return data;
	}

	/// <summary>
	/// 反序列化为对应的proto
	/// </summary>
	/// <typeparam name="T">对应的proto的类型</typeparam>
	/// <param name="data">二进制数据</param>
	/// <returns>对应的proto值</returns>
	public T DeSerializeProto<T>(byte[] data)
	{
		try
		{
			using (MemoryStream ms = new MemoryStream(data))
			{
				//使用工具反序列化对象
				return Serializer.Deserialize<T>(ms);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("反序列化失败: " + ex.ToString());
			return default(T);
		}
	}
}
