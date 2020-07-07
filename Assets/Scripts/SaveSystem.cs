using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : Singleton<SaveSystem>
{
    private int _randomLevelControl = int.MaxValue;
    private int _temp = 0;
    private List<int> _regularRandom;
    private int _regularRanCounter = 0;
    public int WhichLevel
    {
        get
        {
            if (TurnRandomLevel == 0)
            {
                return PlayerPrefs.GetInt("WhichLevel", 0);
            }
            else
            {
                switch (GameManager.Instance.RandomLevel)
                {
                    case EnumManager.RandomLevel.AlwaysRandom:

                        if (_randomLevelControl == int.MaxValue)
                        {
                            _randomLevelControl = RandomLevelNo();
                            _temp = _randomLevelControl;
                            PlayerPrefs.SetInt("WhichLevel", _randomLevelControl);
                            return _randomLevelControl;
                        }

                        _randomLevelControl = PlayerPrefs.GetInt("WhichLevel", 0);
                        if (_randomLevelControl != _temp)
                        {
                            _randomLevelControl = RandomLevelNo();
                            _temp = _randomLevelControl;
                        }

                        PlayerPrefs.SetInt("WhichLevel", _randomLevelControl);
                        return _randomLevelControl;

                    case EnumManager.RandomLevel.RegularRandom:

                        if (_randomLevelControl == int.MaxValue)
                        {
                            _randomLevelControl = _regularRandom[_regularRanCounter++];
                            _temp = _randomLevelControl;
                            PlayerPrefs.SetInt("WhichLevel", _randomLevelControl);
                            return _randomLevelControl;
                        }

                        _randomLevelControl = PlayerPrefs.GetInt("WhichLevel", 0);

                        if (_randomLevelControl != _temp)
                        {
                            _randomLevelControl = _regularRandom[_regularRanCounter++];
                            _temp = _randomLevelControl;
                        }

                        if (_regularRanCounter == _regularRandom.Count)
                        {
                            PrepareRandomLevel();
                            _regularRanCounter = 0;
                        }
                        PlayerPrefs.SetInt("WhichLevel", _randomLevelControl);
                        return _randomLevelControl;
                    case EnumManager.RandomLevel.BackToTop:
                        int currentLevel = PlayerPrefs.GetInt("WhichLevel", 0);
                        if (currentLevel == GameManager.Instance.Levels.Length)
                        {
                            PlayerPrefs.SetInt("WhichLevel", 0);
                            currentLevel = 0;
                        }
                        return currentLevel;
                    default:
                        return 0;
                }
            }
        }
        set
        {
            PlayerPrefs.SetInt("WhichLevel", value);
        }
    }
    public int TurnRandomLevel
    {
        get
        {
            return PlayerPrefs.GetInt("TurnRandomLevel");
        }
        set
        {
            PlayerPrefs.SetInt("TurnRandomLevel", value);
        }
    }
    public override IEnumerator Initialize()
    {
        PrepareRandomLevel();
        yield return new WaitForEndOfFrame();
    }
    private int RandomLevelNo()
    {
        return Random.Range(0, GameManager.Instance.Levels.Length);
    }
    private void PrepareRandomLevel()
    {
        switch (GameManager.Instance.RandomLevel)
        {
            case EnumManager.RandomLevel.AlwaysRandom:
                break;
            case EnumManager.RandomLevel.RegularRandom:

                int arrayLength = GameManager.Instance.Levels.Length;
                _regularRandom = new List<int>();

                for (int i = 0; i < arrayLength; i++)
                    _regularRandom.Add(i);

                for (int i = 0; i < arrayLength; i++)
                {
                    int r = Random.Range(0, arrayLength);
                    int temp = _regularRandom[r];
                    _regularRandom[r] = _regularRandom[i];
                    _regularRandom[i] = temp;
                }
                break;
        }
    }
}
