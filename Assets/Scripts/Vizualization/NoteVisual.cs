using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NoteVisual : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image icon;
    private Vector3[] waypoints;
    private Vector3 startPosition;

    public void ChangeStartPosition(Transform root)
    {
        int randomInt = Random.Range(0, 2) * 2 - 1;
        startPosition = transform.parent.InverseTransformPoint(root.position);
        waypoints = new Vector3[]
        {
            new(startPosition.x - 10 * randomInt, startPosition.y + 50),
            new(startPosition.x + 10 * randomInt, startPosition.y + 100),
            new(startPosition.x - 10 * randomInt, startPosition.y + 150),
            new(startPosition.x + 10 * randomInt, startPosition.y + 200)
        };
    }
    
    public void Play()
    {
        Sequence notesSeq = DOTween.Sequence();
        notesSeq.Append(rect.DOScale(1, 0.5f).SetEase(Ease.OutBack))
            .Insert(0.1f, rect.DOLocalPath(waypoints,1,PathType.CatmullRom))
            .Insert(0.5f,icon.DOFade(0, 0.5f));
        
        notesSeq.OnComplete(() =>
        {
            rect.localScale = Vector3.zero;
            rect.localPosition = startPosition;
            icon.color = Color.white;
        });
    }
}
