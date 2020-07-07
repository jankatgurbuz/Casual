using System;
using System.Collections;
using UnityEngine;
using JLibrary.JUI;
using UnityEngine.UI;
using System.Threading.Tasks;


public partial class GameManager : Singleton<GameManager>
{
    public Level[] Levels;
    public EnumManager.RandomLevel RandomLevel;
    private JImage _loadingBarForMainScene;
    public int WhichLevel
    {
        get
        {
            return SaveSystem.Instance.WhichLevel;
        }
        set
        {
            SaveSystem.Instance.WhichLevel = value;
        }
    }
    public int WhichProgress { get; private set; } = 0;

    private GameObject _currentProgress;

    public override void Awake()
    {
        base.Awake();
    }
    private IEnumerator Start()
    {
        LoadingBar(true);
        Coroutine c = StartCoroutine(Loading());
        yield return new WaitForSeconds(0.1f);

        //-----Initialize-------------------------
        yield return StartCoroutine(SaveSystem.Instance.Initialize());
        yield return StartCoroutine(Initialize());
        yield return StartCoroutine(ProcessManager.Instance.Initialize());
        yield return StartCoroutine(TouchManager.Instance.Initialize());
        //-----FinishInitialize-------------------------

        LoadingBar(false);
        StopCoroutine(c);
    }
    public override IEnumerator Initialize()
    {
        InstatiateProcess();
        yield return new WaitForEndOfFrame();
    }

    public void InstatiateProcess()
    {
        string proName = "ProcessManager";
        GameObject IsExist = GameObject.Find(proName);

        if (IsExist == null)
            InstatiateProcess(proName);
    }

    public void InstatiateProcess(string proName)
    {
        if (Levels.Length != 0)
        {
            Level level = Levels[WhichLevel];
            Debug.Log("WhichLevel " + WhichLevel + "  whichProgress" + WhichProgress);
            GameObject progress = level.Processes[WhichProgress];
            _currentProgress = Instantiate(progress, Vector3.zero, Quaternion.identity);
            _currentProgress.name = proName;
        }
        else
        {
            Debug.LogError("Levels Object Is Empty");
            Quit();
        }

    }
    public IEnumerator Loading()
    {
        Text _text = GameObject.Find("LoadingBarForMainSceneText").GetComponent<Text>();
        while (true)
        {
            _text.text = "Loading";
            yield return new WaitForSeconds(0.025f);
            _text.text = "Loading.";
            yield return new WaitForSeconds(0.025f);
            _text.text = "Loading..";
            yield return new WaitForSeconds(0.025f);
            _text.text = "Loading...";
            yield return new WaitForSeconds(0.025f);
        }
    }
    public void LoadingBar(bool control)
    {
        string name = "LoadingBarForMainScene";
        if (control)
        {
            _loadingBarForMainScene = UIManager.FindImage(name);
            if (_loadingBarForMainScene == null)
            {
                // Image LoadingBarForMainScene
                GameObject loadingBar = new GameObject(name);

                loadingBar.transform.SetParent(UIManager.FindEmpty("Canvas").transform);
                loadingBar.AddComponent<Image>().color = Color.black;
                loadingBar.AddComponent<JImage>();

                RectTransform rt = loadingBar.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;

                // Text LoadingBarForMainSceneText
                GameObject text = new GameObject(name + "Text");
                text.transform.SetParent(loadingBar.transform);

                Text t = text.AddComponent<Text>();
                t.font = Resources.Load("Editor/RifficFree-Bold") as Font;
                t.alignment = TextAnchor.MiddleCenter;
                t.fontSize = 50;

                text.AddComponent<JText>();
                rt = text.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;

                loadingBar.GetComponent<JImage>().WhichElementEnum = WhichElement.Image; 
                text.GetComponent<JText>().WhichElementEnum = WhichElement.Text;
                UIManager.Instance.Run();
               
                _loadingBarForMainScene = UIManager.FindImage(name);
            }
            _loadingBarForMainScene.gameObject.SetActive(true);
        }
        else
        {
            _loadingBarForMainScene.gameObject.SetActive(false);
        }
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public Level CurrentLevel()
    {
        return Levels[WhichLevel];
    }

    public void NextProgress()
    {
        if (_currentProgress != null)
            StartCoroutine(DestroyProcess());

        Level level = Levels[WhichLevel];
        // Finish Progress
        if (WhichProgress == level.Processes.Length - 1)
        {
            WhichProgress = 0;
            WhichLevel++;

            if (WhichLevel == Levels.Length)
                SaveSystem.Instance.TurnRandomLevel = 1;

            InstatiateProcess("ProcessManager");
            StartCoroutine(ProcessManager.Instance.Initialize());
        }
        else // Continue Progress
        {
            Debug.Log(WhichProgress + "  " + level.Processes.Length + "  devam");
            WhichProgress++;
            InstatiateProcess("ProcessManager");
            StartCoroutine(ProcessManager.Instance.Initialize());
        }

    }
    private IEnumerator DestroyProcess()
    {
        Destroy(_currentProgress);
        yield return null;
    }
}

public partial class GameManager
{
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
        }
    }
#endif
}
