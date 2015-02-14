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
	
	void Update () {
		TargetSprite = TargetSprite ?? this.GetComponent<SpriteRenderer>();
		TargetSprite.sprite = SourceSprite.sprite;
		TargetSprite.material = SourceSprite.material;
		
		SourceSprite.GetPropertyBlock(block);
		TargetSprite.SetPropertyBlock(block);
	}
}
