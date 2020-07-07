using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public partial
    class TouchManager : Singleton<TouchManager>
{

    public Camera Camera;
    private TouchScreenWithoutRaycast _tswor;
    private TouchScreenWithRaycast _tswr;
    private TouchScreenDoItYourself _tsdiy;
    public override IEnumerator Initialize()
    {
        if (Camera != null)
            Camera = Camera.main;

        yield return new WaitForEndOfFrame();
    }

    // InitTouchScreenWithoutRaycast---------------------------------------------------------------------------------------------
    public void InitTouchScreenWithoutRaycast(Action Down, Action Set, Action Up)
    {
        _tswor = new TouchScreenWithoutRaycast(Down, Set, Up);
    }
    public void KillTouchScreenWithoutRaycast()
    {
        _tswor = null;
    }
    //----------------------------------------------------------------------------------------------------------------------------

    // InitTouchScreenWithRaycast---------------------------------------------------------------------------------------------
    public void InitTouchScreenWithRaycast(
        Action<bool, RaycastHit> Down,
        Action<bool, RaycastHit> Set,
        Action<bool, RaycastHit> Up,
        LayerMask LayerMask)
    {
        _tswr = new TouchScreenWithRaycast(Down, Set, Up, LayerMask);
    }
    public void KillTouchScreenWithRaycast()
    {
        _tswr = null;
    }
    //----------------------------------------------------------------------------------------------------------------------------

    // TouchScreenDoItYourself---------------------------------------------------------------------------------------------
    public void InitTouchScreenDoItYourself(
        Action<bool, RaycastHit, bool, RaycastHit> ClickDown,
            Action<bool, RaycastHit, bool, RaycastHit> ClickSet,
            Action<bool, RaycastHit, bool, RaycastHit> ClickUp,
            LayerMask LayerMaskBase,
            LayerMask LayerMaskMain,
            Vector3? FirstPos = null,
            float PositiveX = float.MaxValue,
            float NegativeX = float.MinValue,
            float PositiveY = float.MaxValue,
            float NegativeY = float.MinValue,
            float PositiveZ = float.MaxValue,
            float NegativeZ = float.MinValue
            )
    {
        _tsdiy = new TouchScreenDoItYourself
            (ClickDown, ClickSet, ClickUp,
            LayerMaskBase, LayerMaskMain,
            PositiveX, NegativeX, PositiveY,
            NegativeY, PositiveZ, NegativeZ, FirstPos);
    }
    public void KillTouchScreenDoItYourself()
    {
        Destroy(_tsdiy.RayObj);
        _tsdiy = null;
    }

    //---------------------------------------------------------------------------------------------------------------------

    public void Update()
    {
        if (_tswor != null)
            _tswor.Action();
        if (_tswr != null)
            _tswr.Action();
        if (_tsdiy != null)
            _tsdiy.Action();
    }
}

public partial class TouchManager
{
    public class TouchScreenWithoutRaycast
    {
        public Action Down, Set, Up;
        public TouchScreenWithoutRaycast(Action Down, Action Set, Action Up)
        {
            this.Down = Down;
            this.Set = Set;
            this.Up = Up;
        }

        public void Action()
        {
            bool c =  !EventSystem.current.IsPointerOverGameObject();
            if (Input.GetMouseButtonDown(0) && c)
                Down?.Invoke();

            else if (Input.GetMouseButton(0) && c)
                Set?.Invoke();

            else if (Input.GetMouseButtonUp(0) && c)
                Up?.Invoke();
        }
    }
    public class TouchScreenWithRaycast
    {
        public Action<bool, RaycastHit> Down, Set, Up;
        public LayerMask LayerMask;
        public TouchScreenWithRaycast(
            Action<bool, RaycastHit> Down,
            Action<bool, RaycastHit> Set,
            Action<bool, RaycastHit> Up,
            LayerMask LayerMask)
        {
            this.Down = Down;
            this.Set = Set;
            this.Up = Up;
            this.LayerMask = LayerMask;
        }

        public void Action()
        {
            bool b = RayCast(out RaycastHit hit);
            bool c = !EventSystem.current.IsPointerOverGameObject();
            if (Input.GetMouseButtonDown(0) && c)
                Down?.Invoke(b, hit);

            else if (Input.GetMouseButton(0) && c)
                Set?.Invoke(b, hit);

            else if (Input.GetMouseButtonUp(0) && c)
                Up?.Invoke(b, hit);
        }
        public bool RayCast(out RaycastHit hit)
        {
            return Physics.Raycast(
                Instance.Camera.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask);
        }
    }
    public class TouchScreenDoItYourself
    {
        public Action<bool, RaycastHit, bool, RaycastHit> ClickDown, ClickSet, ClickUp;
        public LayerMask LayerMaskBase;
        public LayerMask LayerMaskMain;

        public float PositiveX, PositiveY, NegativeX, NegativeY, PositiveZ, NegativeZ;

        public Vector3? FirstPos = Vector3.zero;

        private bool _firstCallControl = true;

        private Vector3 _firstPos = Vector3.zero;

        private Vector3 _lastPos = Vector3.zero;

        public Transform RayObj;

        public TouchScreenDoItYourself(
            Action<bool, RaycastHit, bool, RaycastHit> ClickDown,
            Action<bool, RaycastHit, bool, RaycastHit> ClickSet,
            Action<bool, RaycastHit, bool, RaycastHit> ClickUp,
            LayerMask LayerMaskBase,
            LayerMask LayerMaskMain,
            float PositiveX,
            float NegativeX,
            float PositiveY,
            float NegativeY,
            float PositiveZ,
            float NegativeZ,
            Vector3? FirstPos)
        {
            this.ClickDown = ClickDown;
            this.ClickSet = ClickSet;
            this.ClickUp = ClickUp;

            this.LayerMaskBase = LayerMaskBase;
            this.LayerMaskMain = LayerMaskMain;

            this.PositiveX = PositiveX;
            this.NegativeX = NegativeX;
            this.PositiveY = PositiveY;
            this.NegativeY = NegativeY;
            this.PositiveZ = PositiveZ;
            this.NegativeZ = NegativeZ;

            if (FirstPos == null)
                FirstPos = Vector3.zero;

            this.FirstPos = FirstPos;

            _firstCallControl = true;
        }

        public void Action()
        {
            bool c = !EventSystem.current.IsPointerOverGameObject();
            if (Input.GetMouseButtonDown(0) && c)
            {
                (bool baseC, RaycastHit baseHit) = RayCastBase();

                if (baseC)
                {
                    if (_firstCallControl)
                    {
                        _firstPos = baseHit.point - (Vector3)FirstPos;
                        _firstCallControl = false;
                    }
                    else
                        _firstPos = baseHit.point - _lastPos;

                    (bool mainC, RaycastHit mainHit) = RayCastMain(ref baseHit);
                    ClickDown?.Invoke(baseC, baseHit, mainC, mainHit);
                }
            }
            else if (Input.GetMouseButton(0) && c)
            {
                (bool baseC, RaycastHit baseHit) = RayCastBase();

                if (baseC)
                {
                    (bool mainC, RaycastHit mainHit) = RayCastMain(ref baseHit);

                    if (mainC)
                        _lastPos = mainHit.point;

                    _firstPos = baseHit.point - _lastPos;

                    ClickSet?.Invoke(baseC, baseHit, mainC, mainHit);

                }
            }
            else if (Input.GetMouseButtonUp(0) && c)
            {
                (bool baseC, RaycastHit baseHit) = RayCastBase();

                if (baseC)
                {
                    (bool mainC, RaycastHit mainHit) = RayCastMain(ref baseHit);
                    ClickUp?.Invoke(baseC, baseHit, mainC, mainHit);
                }
            }
        }
        public (bool, RaycastHit) RayCastBase()
        {
            Ray ray = Instance.Camera.ScreenPointToRay(Input.mousePosition);

            bool r1 = Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMaskBase);

            hit.point = new Vector3
               (
                   Mathf.Clamp(hit.point.x, NegativeX, PositiveX),
                   Mathf.Clamp(hit.point.y, NegativeY, PositiveY),
                   Mathf.Clamp(hit.point.z, NegativeZ, PositiveZ)
               );

            return (r1, hit);
        }


        public (bool, RaycastHit) RayCastMain(ref RaycastHit hit)
        {
            Debug.DrawLine(Instance.Camera.transform.position, hit.point);

            Vector3 cp = Instance.Camera.transform.position;
            Vector3 ro = hit.point;
            Vector3 vec = (ro - _firstPos - cp);

            vec = new Vector3
                  (
                      Mathf.Clamp(vec.x, NegativeX, PositiveX),
                      Mathf.Clamp(vec.y, NegativeY, PositiveY),
                      Mathf.Clamp(vec.z, NegativeZ, PositiveZ)
                  );

            bool control = Physics.Raycast(cp, (vec).normalized, out RaycastHit nHit, 1000, LayerMaskMain);

            if (control)
            {
                Debug.DrawLine(Instance.Camera.transform.position, nHit.point, Color.red);
            }

            return (control, nHit);
        }
    }
}

public partial class TouchManager
{

}


