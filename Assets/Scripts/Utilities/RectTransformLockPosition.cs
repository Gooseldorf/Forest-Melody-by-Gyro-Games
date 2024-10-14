using UnityEngine;

[ExecuteInEditMode()]
public class RectTransformLockPosition : MonoBehaviour
{
    [SerializeField] private RectTransform refRectTransform;
    
    private Vector3 startPosition;
    private RectTransform rectTransform;

    protected void Awake()
    {
		rectTransform = GetComponent<RectTransform>();
        startPosition = new Vector3(refRectTransform.rect.width / 2.0f, refRectTransform.rect.height / 1.6f, 0);
    }
    
    private void LateUpdate()
    {
		if (startPosition != null) 
		{
			rectTransform.position = startPosition;
		}
    }
}