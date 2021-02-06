using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubWindow : Window
{
    [Header("Buttons")]
    public BoxCollider2D NewProjectButton;

    [Header("Sound")]
    public AudioClip NewProjectClick;

    private AudioSource audioSource;

    public override void HandleHover(RaycastHit2D hit)
    {
        if (!GameManager.instance.Playing)
            return;

        if (hit.collider == NewProjectButton)
        {
            // hover effect
        }
    }

    public override void HandleClick(RaycastHit2D hit)
    {
        if (!GameManager.instance.Playing)
            return;

        if (hit.collider == NewProjectButton)
        {
            NewProject();
        }
        else
        {
            StartDragging(hit);
        }
    }

    private void NewProject()
    {
        // Debug.Log("New Project!");
        audioSource.clip = NewProjectClick;
        audioSource.pitch = Random.Range(1.0f, 1.1f);
        audioSource.Play();
        GameManager.instance.StartNewProject();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.Playing)
            return;

        Drag();
    }
}
