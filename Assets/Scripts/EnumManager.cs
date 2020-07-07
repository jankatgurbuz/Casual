using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumManager 
{
    public enum Process
    {
        Empty, Initialize, Execute, EndPhase, FinishProcess
    }
    public enum RandomLevel 
    {
        AlwaysRandom, 
        RegularRandom,
        BackToTop
    }
}
