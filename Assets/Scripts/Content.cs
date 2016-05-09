using UnityEngine;
using System.Collections;

public class Content : MonoBehaviour
{
	[SerializeField] UnityEngine.UI.Text text;
	public int index;
	public bool calculated = false;

	public Vector2 size
	{
		get
		{
			var rect_trans = transform as RectTransform;
			return rect_trans.sizeDelta;
		}
	}

	public void UpdateContent(int _index)
	{
		index = _index;
		text.text = index.ToString();
	}
}
