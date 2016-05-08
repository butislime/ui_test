using UnityEngine;
using System.Collections;

public class Content : MonoBehaviour
{
	[SerializeField] UnityEngine.UI.Text text;
	public void UpdateContent(int index)
	{
		text.text = index.ToString();
	}
}
