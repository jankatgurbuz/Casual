using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sil : Phase
{
    public override void EndPhase(string newPhaseName = null, bool finishControl = false)
    {
        base.EndPhase(newPhaseName, finishControl);
      
    }

    public override void Execute()
    {
       
        EndPhase("Rasp");
    }

    public override void Initialize()
    {
       
    }
}