using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// change from 2D to 3D and vice versa
public class Change2D : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject cam2D;
    [SerializeField] private GameObject cam3D;
    [SerializeField] private GameObject buttonAt2D;
    [SerializeField] private GameObject buttonAt3D;
    private bool is2D = true;

    // alternates between 2D and 3D on click
    public void OnPointerDown(PointerEventData pointerEventData){
        is2D = !is2D;

        cam2D.SetActive(is2D);
        cam3D.SetActive(!is2D);
        buttonAt2D.SetActive(is2D);
        buttonAt3D.SetActive(!is2D);
    }
}
