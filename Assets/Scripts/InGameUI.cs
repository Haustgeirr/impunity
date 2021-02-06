using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public GameObject MaskPanel;
    public GameObject JoyBarPanel;
    public GameObject TaskBar;
    public GameObject TitlePanel;
    public GameObject InstructionsPanel;
    public GameObject PausePanel;
    public GameObject GameOverPanel;
    public TextMeshProUGUI GameOverScoreText;
    public TextMeshProUGUI VictoryText;
    public GameObject OutOfMemoryPanel;
    public Slider OutOfMemoryTimer;

    private bool isPaused = false;
    private bool showingInstructions = false;

    public void ShowInstructions()
    {
        if (!showingInstructions)
        {
            InstructionsPanel.SetActive(true);
            showingInstructions = true;
        }
        else
        {
            InstructionsPanel.SetActive(false);
            showingInstructions = false;
        }
    }

    private void ShowGameUI()
    {
        MaskPanel.SetActive(false);
        TitlePanel.SetActive(false);
        InstructionsPanel.SetActive(false);
        TaskBar.SetActive(true);
        JoyBarPanel.SetActive(true);
    }

    private void ShowOutOfMemoryPanel(bool show)
    {
        if (show)
        {
            MaskPanel.SetActive(true);
            OutOfMemoryPanel.SetActive(true);
            OutOfMemoryTimer.value = 0;
        }
        else
        {
            MaskPanel.SetActive(false);
            OutOfMemoryPanel.SetActive(false);
        }
    }

    private void UpdateOutOfMemory(float time)
    {
        OutOfMemoryTimer.value = time;
    }

    private void ShowGameOverPanel(bool playerWins)
    {
        MaskPanel.SetActive(true);

        if (playerWins)
        {
            // WinPanel.SetActive(true);
            var joyScore = (int)((GameManager.instance.CurrentJoy / GameManager.instance.MaxJoy) * 100);
            GameOverScoreText.text = "You had " + joyScore + "% joy remaining!";
            VictoryText.text = "You Win!";
        }
        else
        {
            var maxScore = 0;

            foreach (var window in GameManager.instance.OpenProjects)
            {
                var unityWindow = window.GetComponent<UnityWindow>();

                if (unityWindow != null)
                {
                    if (unityWindow.ProjectScore > maxScore)
                        maxScore = (int)unityWindow.ProjectScore;
                }
            }

            GameOverScoreText.text = "HIGH SCORE " + maxScore + "%";
            VictoryText.text = "You lose!";
        }

        GameOverPanel.SetActive(true);
    }

    private void ShowPauseMenu()
    {
        if (!isPaused)
        {
            MaskPanel.SetActive(true);
            PausePanel.SetActive(true);
            isPaused = true;
        }
        else
        {
            MaskPanel.SetActive(false);
            PausePanel.SetActive(false);
            isPaused = false;
        }
    }

    private void OnEnable()
    {
        GameManager.onStartGameUI += ShowGameUI;
        GameManager.onPauseUI += ShowPauseMenu;
        GameManager.onGameOverUI += ShowGameOverPanel;
        GameManager.onOutOfMemoryUI += ShowOutOfMemoryPanel;
        GameManager.onUpdateOutOfMemoryUI += UpdateOutOfMemory;
    }

    private void OnDisable()
    {
        GameManager.onStartGameUI -= ShowGameUI;
        GameManager.onPauseUI -= ShowPauseMenu;
        GameManager.onGameOverUI -= ShowGameOverPanel;
        GameManager.onOutOfMemoryUI -= ShowOutOfMemoryPanel;
        GameManager.onUpdateOutOfMemoryUI -= UpdateOutOfMemory;
    }

    // Start is called before the first frame update
    void Start()
    {
        MaskPanel.SetActive(true);
        TitlePanel.SetActive(true);
        TaskBar.SetActive(false);
        JoyBarPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
