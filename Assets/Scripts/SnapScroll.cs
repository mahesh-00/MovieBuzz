using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel, samplelistItem;
    [SerializeField] private VerticalLayoutGroup HLG;
    
    private bool isDragging;
    public int currentItem;
    int previousItem = 0;
    [SerializeField] private float scrollVelocity;
    [SerializeField] private float snapForce;

    void Start()
    {
        var y = contentPanel.localPosition.y;
    }

    void Update()
    {
        currentItem = Mathf.RoundToInt((contentPanel.localPosition.y / (samplelistItem.rect.height + HLG.spacing)));

        // Only perform snapping when not dragging
        if (!isDragging)
        {
                contentPanel.localPosition = new Vector3(
                    contentPanel.localPosition.x,
                    Mathf.MoveTowards(contentPanel.localPosition.y, (currentItem * (samplelistItem.rect.height + HLG.spacing)), snapForce),
                    contentPanel.localPosition.z
                );
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        //Debug.Log("isDragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        //Debug.Log("not Dragging");
    }
}
