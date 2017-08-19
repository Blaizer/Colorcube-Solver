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

    public static Triangles Instance { get; private set; }

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

        if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Right)
        {
            bool on = eventData.button == PointerEventData.InputButton.Left;
            triangle[selectedColor] = on;
            PlayerPrefs.SetInt(triangle.GetKey(selectedColor), System.Convert.ToInt32(on));
        }

        //Debug.Log(string.Join(", ", (from pair in triangles.Select((value, index) => new { value, index }) where pair.value[0] select pair.index.ToString()).ToArray()));
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        triangles = GetComponentsInChildren<Triangle>();
        
        foreach (var triangle in triangles)
        {
            for (int c = 0; c < 3; c++)
            {
                triangle[c] = System.Convert.ToBoolean(PlayerPrefs.GetInt(triangle.GetKey(c)));
            }
        }
    }

    public bool[,] TrianglesColors
    {
        get
        {
            var result = new bool[24, 3];
            for (int i = 0; i < 24; i++)
            {
                for (int c = 0; c < 3; c++)
                {
                    result[i, c] = triangles[i][c];
                }
            }
            return result;
        }
    }
}
