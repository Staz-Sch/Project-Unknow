using UnityEngine;

public class Highlight_Controller : MonoBehaviour
{
    [SerializeField] private string outlineLayerName = "Outline";

    private int outlineLayer;
    private GameObject lastHiglighted;


    private void Awake()
    {
        outlineLayer = LayerMask.NameToLayer(outlineLayerName);
    }

    private void Update()
    {
       HighlightRaycastCheck();
    }

    private void HighlightRaycastCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

        var distance = 3f;

        if (Physics.Raycast(ray,out RaycastHit hit, distance))
        {
            if (hit.collider.TryGetComponent(out Highlight_Tag target))
            {
                GameObject targetObject = target.gameObject;
                if (lastHiglighted != target)
                {
                    targetObject.layer = outlineLayer;
                    lastHiglighted = targetObject;
                }
                return;
            }
        }
        //Clear Hilight

        ClearHighlight();


    }


    private void ClearHighlight()
    {
        if(lastHiglighted != null)
        {
            if (lastHiglighted.TryGetComponent(out Highlight_Tag target)) 
            { 
                lastHiglighted.layer = target.originalLayer;
            }
            lastHiglighted = null;
        }
    }
}
