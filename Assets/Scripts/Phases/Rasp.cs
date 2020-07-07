using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rasp : Phase
{
    float time = 0;
    public override void EndPhase(string newPhaseName = null, bool finishControl = false)
    {
        base.EndPhase(newPhaseName, finishControl);
    }

    public override void Execute()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            EndPhase(null,true);
        }

    }

    public override void Initialize()
    {
        time = 0;
        Debug.Log("Rasp Selem "+ GameManager.Instance.WhichLevel);
    }
}