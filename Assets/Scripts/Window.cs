using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    // public BoxCollider2D Background;

    private bool isDragging;
    private Vector2 pointerOffset;

    public virtual void HandleHover(RaycastHit2D hit)
    {
        if (!GameManager.instance.Playing)
            return;
    }

    public virtual void HandleClick(RaycastHit2D hit)
    {
        if (!GameManager.instance.Playing)
            return;

        StartDragging(hit);
    }

    // round to whole pixel
    public void StopDragging()
    {
        // Debug.Log("Stop Dragging!");
        isDragging = false;
    }

    protected void StartDragging(RaycastHit2D hit)
    {
        // Debug.Log("Start Dragging!");
        SetWindowOrder();
        pointerOffset = (Vector2)transform.position - hit.point;
        isDragging = true;
    }

    protected void SetWindowOrder()
    {
        // Debug.Log("Set Order");
        GameManager.instance.SetWindowOrder(this);
    }

    protected virtual void Drag()
    {
        if (isDragging)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var windowCentre = (Vector2)worldPos + pointerOffset;
            var x = (int)(windowCentre.x / GameManager.PIXEL_RATIO) * GameManager.PIXEL_RATIO;
            var y = (int)(windowCentre.y / GameManager.PIXEL_RATIO) * GameManager.PIXEL_RATIO;

            transform.position = new Vector3(x, y, transform.position.z);
        }
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
    }
}
