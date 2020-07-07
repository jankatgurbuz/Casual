using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Phase : MonoBehaviour
{
    public virtual void Initialize()
    {

    }
    public abstract void Execute();
    public virtual void EndPhase(string newPhaseName = null, bool finishControl = false)
    {
        ProcessManager.Instance.ChangePhase(newPhaseName, finishControl);
    }
}
