using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "EffectVisualManager", menuName = "ScriptableObjects/OneTime/EffectVisualManager")]
public class EffectVisualManager : ScriptableObjSingleton<EffectVisualManager>
{
    [SerializeField] private List<BranchVisual> commonBranchVisuals;
    [SerializeField] private List<BranchVisual> specialBranchVisuals;
    [SerializeField] private List<ItemPositionController> treeItemPrefabs;
    [SerializeField] private TapEffectVisual tapEffectPrefab;
    [SerializeField] private int tapEffectPoolSize;

    public SpriteAtlas birdsAtlas;
    public SpriteAtlas decorAtlas;
    
    public ObjectPool<TapEffectVisual> TapEffectPool { get; private set; }

    public void Init()
    {
        InitTapEffectPool();
    }
    
    public ItemPositionController GetTreeItem(string id)
    {
        return treeItemPrefabs.Find(x => x.gameObject.name == id);
    }

    public BranchVisual GetBranchVisual(int branchPosition, List<ContainerType> containerTypes) 
    {
        if (!containerTypes.Contains(ContainerType.Special))
        {
            int visualIndex = branchPosition % commonBranchVisuals.Count;
            return commonBranchVisuals[visualIndex];
        }

        if (containerTypes[0] == ContainerType.Special) return specialBranchVisuals[0];
        return specialBranchVisuals[1];
    }    

    private void InitTapEffectPool()
    {
        TapEffectPool = new ObjectPool<TapEffectVisual>(CreateTapEffect,GetTapEffect,ReleaseTapEffect,
            tapEffect => Destroy(tapEffect.gameObject),true, tapEffectPoolSize);
        
    }

    public void PlayTapEffect(Transform root, Transform parent)
    {
        TapEffectVisual tapEffect = TapEffectPool.Get();
        tapEffect.transform.position = root.position;
        tapEffect.transform.SetParent(parent);
        tapEffect.Play();
    }
    
    private TapEffectVisual CreateTapEffect()
    {
        TapEffectVisual tapEffect = Instantiate(tapEffectPrefab);
        tapEffect.gameObject.SetActive(false);
        return tapEffect;
    }

    private void GetTapEffect(TapEffectVisual tapEffect)
    {
        tapEffect.gameObject.SetActive(true);
    }

    private void ReleaseTapEffect(TapEffectVisual notesText)
    {
        notesText.gameObject.SetActive(false);
    }
}
