using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalScore : Phase
{
    float time = 0;
    public override void EndPhase(string newPhaseName = null, bool finishControl = false)
    {
        base.EndPhase(newPhaseName, finishControl);
    }

    public override void Execute()
    {
        Debug.Log("Total Score Hi");
        time += Time.deltaTime;
        if (time>3)
        {
            EndPhase();
        }
    }

    public override void Initialize()
    {
        Debug.Log("Total Score Initialize Hi");
    }
}