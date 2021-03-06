﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SingleTon<T>: IDisposable where T:new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance==null)
            {
                instance = new T();
            }
            return instance;
        }
    }

    public virtual void Dispose()
    {

    }
}
