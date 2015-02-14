using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SetShaderProperties : MonoBehaviour {

	public Color Screen;
	public float Contrast;
	public float Brightness;
	
	public SpriteRenderer TargetRenderer;
	
	private MaterialPropertyBlock block;
	
	public void Awake()
	{
		block = new MaterialPropertyBlock();
	}
	
	void OnWillRenderObject () {
		TargetRenderer.GetPropertyBlock(block);
		block.AddColor("_Screen", Screen);
		block.AddFloat("_Contrast", Contrast);
		block.AddFloat("_Brightness", Brightness);
		TargetRenderer.SetPropertyBlock(block);
	}
}
