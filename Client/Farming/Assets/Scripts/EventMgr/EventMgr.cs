using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMgr
{
    private static Dictionary<EventType, Delegate> m_EventTab = new Dictionary<EventType, Delegate>();

    //添加事件监听的参数判断
    private static void OnListenerAdding(EventType eventType, Delegate callBack)
    {
        if (!m_EventTab.ContainsKey(eventType))
        {
            m_EventTab.Add(eventType, null);
        }
        Delegate d = m_EventTab[eventType];
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("尝试为事件{0}添加不同的委托类型，当前事件所对应的委托类型为{1}，要添加的委托类型为{2}", eventType, d.GetType(), callBack.GetType()));
        }
    }
    //移除事件监听的参数判断
    private static void OnListenerRemoving(EventType eventType, Delegate callBack)
    {
        if (m_EventTab.ContainsKey(eventType))
        {
            Delegate d = m_EventTab[eventType];
            if (d == null)
            {
                throw new Exception(string.Format("移出监听错误：事件{0}没有对应的委托", eventType));
            }
            else if (d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移出监听错误：尝试为事件{0}移除不同类型的委托，当前委托类型为{1}，要移除的委托类型为{2}", eventType, d.GetType(), callBack.GetType()));
            }
        }
        else
        {
            throw new Exception(string.Format("移出监听错误：事件{0}不存在", eventType));
        }
    }
    private static void OnListenerRemoved(EventType eventType)
    {
        if (m_EventTab[eventType] == null)
        {
            m_EventTab.Remove(eventType);
        }
    }
    //添加无参数的监听事件
    public static void AddListener(EventType eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTab[eventType] = (CallBack)m_EventTab[eventType] + callBack;
    }
    //添加一个参数参数的监听事件
    public static void AddListener<T>(EventType eventType, CallBack<T> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T>)m_EventTab[eventType] + callBack;
    }
    //添加二个参数参数的监听事件
    public static void AddListener<T, X>(EventType eventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T, X>)m_EventTab[eventType] + callBack;
    }
    //添加三个参数参数的监听事件
    public static void AddListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T, X, Y>)m_EventTab[eventType] + callBack;
    }
    //添加四个参数参数的监听事件
    public static void AddListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T, X, Y, Z>)m_EventTab[eventType] + callBack;
    }
    //添加五个参数参数的监听事件
    public static void AddListener<T, X, Y, Z, W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTab[eventType] + callBack;
    }

    //移除无参数的监听事件
    public static void RemoveListener(EventType eventType, CallBack callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTab[eventType] = (CallBack)m_EventTab[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //移除一个参数的监听事件
    public static void RemoveListener<T>(EventType eventType, CallBack<T> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T>)m_EventTab[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //移除二个参数的监听事件
    public static void RemoveListener<T, X>(EventType eventType, CallBack<T, X> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T, X>)m_EventTab[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //移除三个参数的监听事件
    public static void RemoveListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T, X, Y>)m_EventTab[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //移除四个参数的监听事件
    public static void RemoveListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T, X, Y, Z>)m_EventTab[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //移除五个参数的监听事件
    public static void RemoveListener<T, X, Y, Z, W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTab[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTab[eventType] - callBack;
        OnListenerRemoved(eventType);
    }


    //无参广播
    public static void Broadcast(EventType eventType)
    {
        Delegate d;
        if (m_EventTab.TryGetValue(eventType, out d))
        {
            CallBack callBack = d as CallBack;
            if (callBack != null)
            {
                callBack();
            }
            else
            {
                throw new Exception(string.Format("广播错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //一个参数广播
    public static void Broadcast<T>(EventType eventType, T arg)
    {
        Delegate d;
        if (m_EventTab.TryGetValue(eventType, out d))
        {
            CallBack<T> callBack = d as CallBack<T>;
            if (callBack != null)
            {
                callBack(arg);
            }
            else
            {
                throw new Exception(string.Format("广播错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //二个参数广播
    public static void Broadcast<T, X>(EventType eventType, T arg1, X arg2)
    {
        Delegate d;
        if (m_EventTab.TryGetValue(eventType, out d))
        {
            CallBack<T, X> callBack = d as CallBack<T, X>;
            if (callBack != null)
            {
                callBack(arg1, arg2);
            }
            else
            {
                throw new Exception(string.Format("广播错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //三个参数广播
    public static void Broadcast<T, X, Y>(EventType eventType, T arg1, X arg2, Y arg3)
    {
        Delegate d;
        if (m_EventTab.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            else
            {
                throw new Exception(string.Format("广播错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //四个参数广播
    public static void Broadcast<T, X, Y, Z>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate d;
        if (m_EventTab.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception(string.Format("广播错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //五个参数广播
    public static void Broadcast<T, X, Y, Z, W>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
    {
        Delegate d;
        if (m_EventTab.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z, W> callBack = d as CallBack<T, X, Y, Z, W>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                throw new Exception(string.Format("广播错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
}
