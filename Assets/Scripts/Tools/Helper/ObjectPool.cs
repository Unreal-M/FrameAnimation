﻿// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-10
// Time: 23:35
// Author: Karsion

using System;
using System.Collections.Generic;
using UnityEngine;

internal class ObjectPool<T> where T : class
{
    private readonly Stack<T> stack;

    internal ObjectPool()
    {
        stack = new Stack<T>();
    }

    internal ObjectPool(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException("count");
        }

        stack = new Stack<T>(count);
    }

    public int countAll { get; private set; }

    public int countActive { get { return countAll - countInactive; } }

    public int countInactive { get { return stack.Count; } }

    public T Get()
    {
        T t;
        if (stack.Count == 0)
        {
            t = default(T) ?? Activator.CreateInstance<T>();
            countAll++;
        }
        else
        {
            t = stack.Pop();
        }

        return t;
    }

    public void Release(T element)
    {
        if (stack.Count > 0 && ReferenceEquals(stack.Peek(), element))
        {
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            return;
        }

        stack.Push(element);
    }
}