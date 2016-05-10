using UnityEngine;
using System.Collections;

public class TestElement : ListElement
{
	[SerializeField] UnityEngine.UI.Text text;

	public void UpdateElement()
	{
		text.text = index.ToString();
	}
}
