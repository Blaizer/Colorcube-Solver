using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class Triangles : MonoBehaviour, IDragHandler, IPointerDownHandler, IScrollHandler
{
    [SerializeField]
    public Color[] primaryColors;

    [SerializeField]
    public Color[] secondayColors;

    Triangle[] triangles;

    int selectedColor;

    public static Triangles Instance { get; set; }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        ClickTriangle(eventData);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        ClickTriangle(eventData);
    }

    void IScrollHandler.OnScroll(PointerEventData eventData)
    {
        selectedColor = (selectedColor + (int)Mathf.Sign(eventData.scrollDelta.x) + 3) % 3;
    }

    void ClickTriangle(PointerEventData eventData)
    {
        var target = eventData.pointerEnter;
        if (target == null)
            return;
        var triangle = target.GetComponent<Triangle>();
        if (triangle == null)
            return;

        triangle[selectedColor] = true;
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        triangles = GetComponentsInChildren<Triangle>();
    }
}
