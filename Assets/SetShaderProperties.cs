using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SetShaderProperties : MonoBehaviour {

	public Color Screen;
	
	[Range(-100, 100)]
	public float Contrast;
	
	[Range(-2, 2)]
	public float Brightness;
	
	public SpriteRenderer TargetRenderer;
	
	private MaterialPropertyBlock block;
	
	void OnWillRenderObject () {
		block = block ?? new MaterialPropertyBlock();
		TargetRenderer.GetPropertyBlock(block);
		block.AddColor("_Screen", Screen);
		block.AddFloat("_Contrast", Contrast);
		block.AddFloat("_Brightness", Brightness);
		TargetRenderer.SetPropertyBlock(block);
	}
}
