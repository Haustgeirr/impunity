using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityWindow : Window
{
    private static readonly Vector2[] buttonPositions = {
        new Vector2(0f, 0f),
        new Vector2(0f, -2f),
        new Vector2(3.5f, 0.75f),
        new Vector2(-3.5f, 0.75f),
        new Vector2(-3.5f, -1.75f),
         };

    [Header("Buttons")]
    public BoxCollider2D CloseButton;
    public GameObject ProgressFlashPrefab;
    public Transform ProgressButton;
    public BoxCollider2D ProgressButtonCollider;
    public Slider ProgressChargeSlider;
    public Slider ProjectScoreSlider;

    private GameObject canvas;
    private RectTransform progressChargeRect;

    [Header("Sound")]
    public AudioClip ProgressClick;
    public AudioClip CancelClick;
    public AudioClip ProgressReady;

    private AudioSource audioSource;

    [Header("Gameplay")]
    public float ProjectScore = 0;

    private bool progressRecharging;
    private float currentProgressButtonCharge;
    private float currentProgressToAdd;
    private float closeTimer;

    private float progressMoveDuration = 0.5f;
    private float progressMoveTimer;

    public override void HandleHover(RaycastHit2D hit)
    {
        if (!GameManager.instance.Playing)
            return;

        if (hit.collider == CloseButton)
        {
            // hover effect
        }
        else if (hit.collider == ProgressButtonCollider)
        {
            // hover effect
        }
    }

    public override void HandleClick(RaycastHit2D hit)
    {
        if (!GameManager.instance.Playing)
            return;

        if (hit.collider == CloseButton)
        {
            CloseWindow();
        }
        else if (hit.collider == ProgressButtonCollider)
        {
            if (!progressRecharging)
                ProgressProject();
        }
        else
        {
            StartDragging(hit);
        }
    }

    public void CancelProject()
    {
        canvas.SetActive(false);
        CloseButton.gameObject.SetActive(false);
        ProgressButton.gameObject.SetActive(false);

        StartCoroutine(ShrinkWindow());
    }

    private IEnumerator ShrinkWindow()
    {
        while (closeTimer < 0.2f)
        {
            this.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, closeTimer / (0.2f));
            closeTimer += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    private void CloseWindow()
    {
        SetWindowOrder();
        audioSource.clip = CancelClick;
        audioSource.Play();
        GameManager.instance.CancelProject(this);
    }

    // Create a ramp for score, so early on it gives like +10, then towards end only +1
    private void ProgressProject()
    {
        SetWindowOrder();
        audioSource.clip = ProgressClick;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();

        progressRecharging = true;

        var joyToAdd = GameManager.instance.BaseProgressToAdd;

        if (ProjectScore > 0)
        {
            joyToAdd = Mathf.Clamp((1 - (ProjectScore / GameManager.instance.WinningProjectScore)) * GameManager.instance.BaseProgressToAdd, 1f, GameManager.instance.BaseProgressToAdd);
        }

        ProjectScore += (int)joyToAdd;
        ProjectScoreSlider.value = (ProjectScore / GameManager.instance.WinningProjectScore) * 12;
        GameManager.instance.AddJoy((int)joyToAdd);
        SetProgressButtonsPosition();
    }

    private void SetProgressButtonsPosition()
    {
        var pos = buttonPositions[Random.Range(0, buttonPositions.Length)];

        StartCoroutine(MoveProgressButtonToNewPosition(
            ProgressButton.localPosition,
            new Vector3(pos.x, pos.y, -0.1f)));

        // ProgressButton.localPosition = new Vector3(pos.x, pos.y, ProgressButton.position.z);
        // progressChargeRect.anchoredPosition = pos;
    }

    private IEnumerator MoveProgressButtonToNewPosition(Vector3 startPos, Vector3 targetPos)
    {
        while (progressMoveTimer < progressMoveDuration)
        {
            progressMoveTimer += Time.deltaTime;
            var t = progressMoveTimer / progressMoveDuration;
            ProgressButton.localPosition = Vector3.Lerp(startPos, targetPos, t);
            progressChargeRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        progressMoveTimer = 0;
        ProgressButton.localPosition = targetPos;
        progressChargeRect.anchoredPosition = targetPos;
    }

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>().gameObject;
        audioSource = GetComponent<AudioSource>();
        progressChargeRect = ProgressChargeSlider.GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.Playing)
            return;

        Drag();

        if (ProjectScore >= GameManager.instance.WinningProjectScore)
            GameManager.instance.Win();

        if (progressRecharging)
        {
            ProgressChargeSlider.value = (currentProgressButtonCharge / GameManager.instance.ProgressButtonChargeDuration) * 64;

            if (currentProgressButtonCharge >= GameManager.instance.ProgressButtonChargeDuration)
            {
                currentProgressButtonCharge = 0;
                var flash = Instantiate(ProgressFlashPrefab, ProgressButton.position, Quaternion.identity, this.transform);
                flash.transform.localPosition += Vector3.forward * -0.1f;
                audioSource.clip = ProgressReady;
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.Play();

                progressRecharging = false;
            }
            else
            {
                currentProgressButtonCharge += Time.deltaTime;
            }
        }
    }
}
