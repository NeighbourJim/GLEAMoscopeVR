using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBGSpriteControl : MonoBehaviour
{
    [SerializeField] GameObject BGPanel;
    private CanvasGroup CanvasGroup;

    private void Start()
    {
        CanvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        float CanvasGroupAlpha = CanvasGroup.alpha;

        if(CanvasGroupAlpha > 0)
        {
            BGPanel.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            BGPanel.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
