using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weld : Phase
{
    public LayerMask layerMaskBase, layerMaskMain;
    private GameObject obj;
    public override void Initialize()
    {
        TouchManager.Instance.InitTouchScreenDoItYourself(Down, Set, Up, layerMaskBase, layerMaskMain);
        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.GetComponent<BoxCollider>().enabled = false;
    }

    private void Up(bool c, RaycastHit hitbase, bool c2, RaycastHit hitmain)
    {

    }

    private void Set(bool c, RaycastHit hitbase, bool c2, RaycastHit hitmain)
    {
        if (c2 && c)
        {
            obj.transform.position = hitmain.point;
        }

    }

    private void Down(bool c, RaycastHit hitbase, bool c2, RaycastHit hitmain)
    {

    }

    public override void EndPhase(string newPhaseName = null, bool finishControl = false)
    {
        base.EndPhase(newPhaseName, finishControl);
    }

    public override void Execute()
    {


    }


}