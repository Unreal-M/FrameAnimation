﻿// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-09
// Time: 14:56
// Author: Karsion

using UnityEngine;
using UnrealM;

public class ActionSequenceSystemExample : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        //Start a once timer
        this.Delayer(1, () => Debug.Log(1));
        this.Sequence().Interval(1).Action(() => Debug.Log(1));//Same

        //Allso use transform as a ID to start a sequence
        transform.Delayer(1, () => Debug.Log(1));

        //Start a loop timer
        this.Looper(0.5f, 3, false, () => Debug.Log(-1));
        this.Sequence().Loop(3).Interval(0.5f).Action(() => Debug.Log(-1));//Same

        //Start a long sequence
        this.Sequence()
            .Interval(2)
            .Action(() => Debug.Log("Test1"))
            .Interval(3)
            .Action(() => Debug.Log("Test2"))
            .Interval(1)
            .Action(() => Debug.Log("Test3 end"))
            ;

        //Check Q key per 0.2 seconds
        this.Sequence()
            .Loop()
            .Interval(0.2f)
            .Condition(() => Input.GetKeyDown(KeyCode.Q))
            .Action(n => Debug.Log("Q键 按下次数" + n));
    }

    // Update is called once per frame
    private void Update()
    {
        //Start a loop in Update, using transform as ID
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Looper(1, -1, true, count => Debug.Log("A" +count));
        }

        //Start a loop in Update
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.Looper(1, -1, true, count => Debug.Log("S" +count));
        }

        //Stop all sequences start by this ID
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.StopSequence();
        }

        //Stop all sequences start by transform
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.StopSequence();
        }

        //If this Component is destroyed, the associated Sequence will automatically stop and recycle to the pool.
    }
}