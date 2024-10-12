using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapScroll : MonoBehaviour ,IDragHandler
{
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private RectTransform contentPanel, samplelistItem;

    [SerializeField] private VerticalLayoutGroup HLG;

    private bool isSnapped = false;
    private float snapSpeed;

    private bool isDragging;
    public int currentItem;
    int previousItem = 0 ;
    [SerializeField] private float scrollVelocity;
    [SerializeField] private float snapForce;

    // Start is called before the first frame update
    void Start()
    {
        var y = contentPanel.localPosition.y;
        //contentPanel.localPosition = new Vector3(contentPanel.localPosition.x, contentPanel.localPosition.y + (samplelistItem.rect.height + HLG.spacing),contentPanel.localPosition.z);

    }

    void Update()
    {
        
         currentItem =
            Mathf.RoundToInt((contentPanel.localPosition.y / (samplelistItem.rect.height + HLG.spacing)));
        
        //Debug.Log(scrollRect.velocity.magnitude +"mag");
        
        if (scrollRect.velocity.magnitude < scrollVelocity && !isSnapped )
        {
            // Debug.Log("magnitude"+ scrollRect.velocity.magnitude+ "contentPos"+ contentPanel.localPosition.y);
            // Debug.Log("predicted pos "+  (currentItem * (samplelistItem.rect.height + HLG.spacing)));
            scrollRect.velocity = Vector2.zero;
        
            snapSpeed += snapForce * Time.deltaTime;
        
        
            contentPanel.localPosition = new Vector3( contentPanel.localPosition.x,
            Mathf.MoveTowards(contentPanel.localPosition.y, (currentItem * (samplelistItem.rect.height + HLG.spacing)), snapSpeed),
                contentPanel.localPosition.z);
        
            if (contentPanel.localPosition.y == (currentItem * (samplelistItem.rect.height + HLG.spacing)))
        
                isSnapped = true;
            
        }
        
        if (scrollRect.velocity.magnitude > scrollVelocity)
        
        {
            isSnapped = false;
        
            snapSpeed = 0;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            isDragging = true;
        }
        else
        {
            isDragging = false;
        }
        Debug.Log("isDragging");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.IsScrolling())
        {
            isDragging = true;
        }
        else
        {
            isDragging = false;
        }
    }
}