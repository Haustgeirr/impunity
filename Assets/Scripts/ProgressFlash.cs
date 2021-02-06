using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressFlash : MonoBehaviour
{
    const float FLASH_LENGTH = 0.2f;

    private SpriteRenderer sprite;
    private float flashTimer;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (flashTimer < FLASH_LENGTH)
        {
            sprite.color = Color.Lerp(new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 0.0f), flashTimer / FLASH_LENGTH);
            this.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, flashTimer / FLASH_LENGTH);
            flashTimer += Time.deltaTime;

        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
