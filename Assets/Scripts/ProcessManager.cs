using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProcessManager : Singleton<ProcessManager>, IAddPhase
{
    public EnumManager.Process ProcessControl { get; set; }

    private Dictionary<string, Phase> _phase = new Dictionary<string, Phase>();
    private Phase _currentPhase = null;
    private string _currentKey = null;

    public override IEnumerator Initialize()
    {
        AddPhase();
        ChangePhase();
        yield return new WaitForEndOfFrame();
    }
    public void AddPhase()
    {
        foreach (Transform child in transform)
            _phase.Add(child.name, child.GetComponent<Phase>());

    }
    private void Update()
    {
        if (_currentPhase == null)
            return;

        switch (ProcessControl)
        {
            case EnumManager.Process.Initialize:
                _currentPhase.Initialize();
                ProcessControl = EnumManager.Process.Execute;
                break;
            case EnumManager.Process.Execute:
                _currentPhase.Execute();
                break;
            case EnumManager.Process.EndPhase:
                break;
        }
    }
    public void ChangePhase(string phaseName = null, bool finishControl = false)
    {
        if (_phase.Count == 0)
        {
            ProcessControl = EnumManager.Process.Empty;
            Debug.LogError("no phase added");
            return;
        }
        if (finishControl)
        {
            FinishProcess();
            return;
        }

        if (phaseName == null)
        {
            if (_currentPhase == null)
            {
                _currentPhase = _phase.First().Value;
                _currentKey = _phase.First().Key;
            }
            else
            {
                (_currentPhase, _currentKey) = PhaseMoveNext();

                if (_currentPhase == null)
                    FinishProcess();
            }
        }
        else
        {
            _currentPhase = _phase[phaseName];
            _currentKey = phaseName;
        }


        ProcessControl = EnumManager.Process.Initialize;
    }
    private (Phase, string) PhaseMoveNext()
    {
        ProcessControl = EnumManager.Process.Initialize;

        var enumerator = _phase.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Key == _currentKey)
            {
                enumerator.MoveNext();
                return (enumerator.Current.Value, enumerator.Current.Key);
            }
        }
        return (null, null);
    }
    public void FinishProcess()
    {
        ProcessControl = EnumManager.Process.FinishProcess;
        GameManager.Instance.NextProgress();
    }

    public T GetPhase<T>(string phaseName) where T : class
    {
        return _phase[phaseName] as T;
    }


}
