using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    Transform objToFollow;
    RectTransform canvasRect;

    public void Initialize(Transform toFollow, RectTransform canvas)
    {
        objToFollow = toFollow;
        canvasRect = canvas;
    }

    void Update()
    {
        if (objToFollow == null) { 
            Destroy(gameObject); 
            return;
        }
        // Follow position
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(objToFollow.position);
        Vector2 compScreenPos = new Vector2(
            (viewportPos.x * canvasRect.sizeDelta.x)-(canvasRect.sizeDelta.x * 0.5f),
            (viewportPos.y * canvasRect.sizeDelta.y)-(canvasRect.sizeDelta.y * 0.4f));
        GetComponent<RectTransform>().anchoredPosition = compScreenPos;

    }
}
