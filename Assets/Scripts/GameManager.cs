using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const float PIXEL_RATIO = 0.03125f;

    const float LAYER_OFFSET = -1f;

    public delegate void OnStartGameUI();
    public static OnStartGameUI onStartGameUI;

    public delegate void OnUpdateJoyUI(float currentJoy);
    public static OnUpdateJoyUI onUpdateJoyUI;

    public delegate void OnPauseUI();
    public static OnPauseUI onPauseUI;

    public delegate void OnGameOverUI(bool playerWins);
    public static OnGameOverUI onGameOverUI;

    public delegate void OnOutOfMemoryUI(bool show);
    public static OnOutOfMemoryUI onOutOfMemoryUI;

    public delegate void OnUpdateOutOfMemoryUI(float time);
    public static OnUpdateOutOfMemoryUI onUpdateOutOfMemoryUI;

    // Singleton pattern
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameManager>();

            return _instance;
        }
    }

    public static bool hasInstance { get { return instance != null; } }

    [Header("Game Settings")]
    public int MaxJoy;
    public float JoyDecayRate;
    public int WinningProjectScore;
    public AudioClip OutOfMemoryClip;
    public float OutOfMemoryDuration;

    private GameObject hubWindow;
    private AudioSource audioSource;
    private float outOfMemoryTime;

    [Header("Unity Settings")]
    public GameObject NewProjectPrefab;
    public int MaxOpenProjects;
    public float BaseProgressToAdd;
    public float CancellationCost;
    public float ProgressButtonChargeDuration;

    [Header("UI Settings")]
    public Transform FloatingTextCanvas;
    public GameObject JoyAddTextPrefab;
    public GameObject JoyRemoveTextPrefab;
    public float JoyTextDuration;

    [Header("Gameplay")]
    public bool Playing;
    public float CurrentJoy;
    public List<Window> OpenProjects;

    public void RestartGame()
    {
        Debug.Log("click");
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void StartGame()
    {
        if (onStartGameUI != null) onStartGameUI();
        hubWindow.SetActive(true);
        Playing = true;
    }

    public void PauseGame()
    {
        if (onPauseUI != null) onPauseUI();

        if (Playing)
        {
            Playing = false;
        }
        else
        {
            Playing = true;
        }
    }

    public void SetWindowOrder(Window front)
    {
        OpenProjects.Remove(front);
        OpenProjects.Sort((w1, w2) => w2.transform.position.z.CompareTo(w1.transform.position.z));

        // Set new layer offset

        var maxOffset = 0f;

        for (int i = 0; i < OpenProjects.Count; i++)
        {
            var t = OpenProjects[i].transform.position;
            maxOffset = i * LAYER_OFFSET;
            t = new Vector3(t.x, t.y, maxOffset);
            OpenProjects[i].transform.position = t;
        }

        front.transform.position = new Vector3(front.transform.position.x, front.transform.position.y, maxOffset + LAYER_OFFSET);
        OpenProjects.Add(front);
    }

    public void CancelProject(Window window)
    {
        RemoveJoy(GameManager.instance.CancellationCost);
        OpenProjects.Remove(window);

        var unityWindow = window as UnityWindow;
        unityWindow.CancelProject();
    }

    public void StartNewProject()
    {
        if (OpenProjects.Count - 1 < MaxOpenProjects)
        {
            var go = Instantiate(NewProjectPrefab, Vector3.zero, Quaternion.identity);
            var window = go.GetComponent<Window>();

            OpenProjects.Add(window);
            SetWindowOrder(window);
        }
        else
        {
            OutOfMemory();
        }
    }

    private void OutOfMemory()
    {
        Playing = false;
        audioSource.clip = OutOfMemoryClip;
        audioSource.Play();
        if (onOutOfMemoryUI != null) onOutOfMemoryUI(true);
        StartCoroutine(OutOfMemoryTimer());
    }

    private IEnumerator OutOfMemoryTimer()
    {
        outOfMemoryTime = 0;

        while (outOfMemoryTime < OutOfMemoryDuration)
        {
            outOfMemoryTime += Time.deltaTime;
            if (onUpdateOutOfMemoryUI != null) onUpdateOutOfMemoryUI(outOfMemoryTime / OutOfMemoryDuration);

            yield return null;
        }

        Playing = true;
        if (onOutOfMemoryUI != null) onOutOfMemoryUI(false);
    }

    public void AddJoy(int add)
    {
        var go = Instantiate(JoyAddTextPrefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(FloatingTextCanvas);

        var joyText = go.GetComponent<JoyText>();
        joyText.SetPosition();
        joyText.SetText("+" + add);

        if (CurrentJoy + add >= MaxJoy)
        {
            CurrentJoy = MaxJoy;
        }
        else
        {
            CurrentJoy += add;
        }
    }

    public void RemoveJoy(float remove)
    {
        var go = Instantiate(JoyRemoveTextPrefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(FloatingTextCanvas);

        var joyText = go.GetComponent<JoyText>();
        joyText.SetPosition();
        joyText.SetText("-" + remove);

        if (CurrentJoy - remove <= 0)
        {
            CurrentJoy = 0;
        }
        else
        {
            CurrentJoy -= remove;
        }
    }

    private void GameOver()
    {
        // Debug.Log("Game Over");
        if (onGameOverUI != null) onGameOverUI(false);
        Playing = false;
    }

    public void Win()
    {
        // Debug.Log("You Win");
        if (onGameOverUI != null) onGameOverUI(true);
        AddJoy(100);
        Playing = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();

        hubWindow = GameObject.Find("Hub Window");

        OpenProjects = new List<Window>();
        OpenProjects.Add(hubWindow.GetComponent<Window>());
        hubWindow.SetActive(false);

        CurrentJoy = MaxJoy;

        Playing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Playing)
            return;

        CurrentJoy -= JoyDecayRate * Time.deltaTime;

        if (onUpdateJoyUI != null) onUpdateJoyUI(CurrentJoy);

        if (CurrentJoy <= 0)
        {
            GameOver();
        }
    }
}
