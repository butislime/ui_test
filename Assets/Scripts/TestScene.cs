using UnityEngine;

public class TestScene : MonoBehaviour
{
	[SerializeField] ScrollList scrollList;

	void Awake()
	{
		scrollList.onUpdateElement.AddListener(OnUpdateElement);
	}
	void OnUpdateElement(ListElement element)
	{
		(element as TestElement).UpdateElement();
	}

	public void AddListElement()
	{
		scrollList.SetElementNum(scrollList.GetElementNum()+1);
	}
}
