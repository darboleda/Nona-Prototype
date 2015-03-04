using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SharedSprite : MonoBehaviour
{
	public SpriteRenderer TargetSprite;
	public SpriteRenderer SourceSprite;
	
	private MaterialPropertyBlock block;
	
    void OnWillRenderObject()
    {
		block = block ?? new MaterialPropertyBlock();
        TargetSprite = TargetSprite ?? this.GetComponent<SpriteRenderer>();
        TargetSprite.sprite = SourceSprite.sprite;
        TargetSprite.sharedMaterial = SourceSprite.sharedMaterial;
        TargetSprite.sortingLayerID = SourceSprite.sortingLayerID;
        TargetSprite.sortingOrder = SourceSprite.sortingOrder;

        SourceSprite.GetPropertyBlock(block);
        TargetSprite.SetPropertyBlock(block);
    }
}
