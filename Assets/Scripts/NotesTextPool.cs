using UnityEngine;
using UnityEngine.Pool;

public class NotesTextPool: MonoBehaviour
{
    [SerializeField] private NotesText notesTextPrefab;
    [SerializeField] private int poolSize;

    public ObjectPool<NotesText> NotesTextObjPool { get; private set; }

    public static NotesTextPool Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        NotesTextObjPool = new ObjectPool<NotesText>(CreateNotesText,GetNotesText,ReleaseNotesText,
            notesText => Destroy(notesText.gameObject),true, poolSize);
    }

    private NotesText CreateNotesText()
    {
        NotesText notesText = Instantiate(notesTextPrefab, transform);
        notesText.gameObject.SetActive(false);
        return notesText;
    }

    private void GetNotesText(NotesText notesText)
    {
        notesText.gameObject.SetActive(true);
    }

    private void ReleaseNotesText(NotesText notesText)
    {
        notesText.gameObject.SetActive(false);
    }
}

