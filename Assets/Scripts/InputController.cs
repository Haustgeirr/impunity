using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public LayerMask WindowMask;
    public Window CurrentlySelectedWindow;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.PauseGame();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            var window = hit.transform.GetComponentInParent<Window>();

            if (window != null)
            {
                CurrentlySelectedWindow = window;
                // window.HandleClick(hit);
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (CurrentlySelectedWindow != null)
                CurrentlySelectedWindow.HandleClick(hit);
        }

        // if (Input.GetButtonDown("Fire1"))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        //     if (hit.collider != null)
        //     {
        //         var window = hit.transform.GetComponentInParent<Window>();

        //         if (window != null)
        //         {
        //             CurrentlySelectedWindow = window;
        //             window.HandleClick(hit);
        //         }
        //     }
        // }

        if (Input.GetButtonUp("Fire1"))
        {
            if (CurrentlySelectedWindow != null)
            {
                CurrentlySelectedWindow.StopDragging();
                CurrentlySelectedWindow = null;
            }
        }
    }
}
