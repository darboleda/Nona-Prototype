using UnityEngine;
using System.Collections;

public class SharedSprite : MonoBehaviour
{
	public SpriteRenderer TargetSprite;
	public SpriteRenderer SourceSprite;
	
	private MaterialPropertyBlock block;
	
	void Awake()
	{
		block = new MaterialPropertyBlock();
	}

    void OnWillRenderObject()
    {
        TargetSprite = TargetSprite ?? this.GetComponent<SpriteRenderer>();
        TargetSprite.sprite = SourceSprite.sprite;
        TargetSprite.material = SourceSprite.material;
        TargetSprite.sortingLayerID = SourceSprite.sortingLayerID;
        TargetSprite.sortingOrder = SourceSprite.sortingOrder;

        SourceSprite.GetPropertyBlock(block);
        TargetSprite.SetPropertyBlock(block);
    }
}
