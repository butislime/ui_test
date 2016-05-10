using UnityEngine;
using System.Collections;

public class ListElement : MonoBehaviour
{
	[HideInInspector] public int index;

	public RectTransform xform
	{
		get
		{
			if(rectXform == null)
			{
				rectXform = transform as RectTransform;
			}
			return rectXform;
		}
	}
	public Vector2 size
	{
		get
		{
			return xform.sizeDelta;
		}
	}
	public void UpdateIndex(int _index)
	{
		index = _index;
	}

	RectTransform rectXform;
}
