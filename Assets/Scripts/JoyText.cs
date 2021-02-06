using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoyText : MonoBehaviour
{
    private RectTransform rect;
    private RectTransform canvasRect;
    private TextMeshProUGUI TMProText;

    private float lifetime;

    public void SetText(string text)
    {
        TMProText.text = text;
    }

    public void SetPosition()
    {
        Vector2 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        Vector2 objectScreenPos = new Vector2(
            ((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        objectScreenPos = new Vector2((int)objectScreenPos.x, (int)objectScreenPos.y);

        rect.anchoredPosition = objectScreenPos;
        rect.localScale = Vector3.one;
    }

    private void Awake()
    {
        canvasRect = GameObject.Find("Floating Text").GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();
        TMProText = GetComponent<TextMeshProUGUI>();
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

        // if (lifetime >= GameManager.instance.JoyTextDuration)
        // {
        //     Destroy(this.gameObject);
        // }
        // else
        // {
        rect.anchoredPosition += Vector2.up * 0.5f;

        if (rect.anchoredPosition.y >= 150)
            Destroy(this.gameObject);

        lifetime += Time.deltaTime;
        // }
    }
}
