using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapScrollHor : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private RectTransform contentPanel, samplelistItem;

    [SerializeField] private HorizontalLayoutGroup HLG;

    private bool isSnapped = false;
    private float snapSpeed;

    [SerializeField] private float snapForce;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        int currentItem =
            Mathf.RoundToInt((0 - contentPanel.localPosition.x / (samplelistItem.rect.width + HLG.spacing)));
        Debug.Log(currentItem);
        Debug.Log(scrollRect.velocity.magnitude +"mag" + HLG.spacing);

        if (scrollRect.velocity.magnitude < 200 && !isSnapped)
        {
            Debug.Log("snapping");
            scrollRect.velocity = Vector2.zero;

            snapSpeed += snapForce * Time.deltaTime;


            contentPanel.localPosition = new Vector3( Mathf.MoveTowards(contentPanel.localPosition.x, 0 - (currentItem * (samplelistItem.rect.width   +HLG.spacing)), snapSpeed),
                contentPanel.localPosition.y,
                contentPanel.localPosition.z);

            if (contentPanel.localPosition.x == 0 - (currentItem * (samplelistItem.rect.width + HLG.spacing)))

                isSnapped = true;
            
        }

        if (scrollRect.velocity.magnitude > 200)

        {
            isSnapped = false;

            snapSpeed = 0;
        }
    }
}