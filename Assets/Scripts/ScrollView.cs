using UnityEngine;
using System.Collections;

public class ScrollView : MonoBehaviour
{
	[SerializeField] RectTransform view;
	[SerializeField] RectTransform originContent;

	[SerializeField] int contentsNum;

	Content[] contents;

	void Start()
	{
		contents = new Content[4];
		for(int i = 0; i < 4; ++i)
		{
			var content = Instantiate(originContent);
			var rect_trans = content.transform as RectTransform;
			rect_trans.SetParent(view, false);
			contents[i] = content.GetComponent<Content>();
			var pos = rect_trans.localPosition;
			pos.y = -rect_trans.sizeDelta.y * i;
			content.transform.localPosition = pos;
		}
		originContent.gameObject.SetActive(false);

		{
			var rect_trans = view.transform as RectTransform;
			var size_delta = rect_trans.sizeDelta;
			size_delta.y = 100*contentsNum;
			rect_trans.sizeDelta = size_delta;
		}

		for(int i = 0; i < contents.Length; ++i)
		{
			contents[i].UpdateContent(i);
		}
	}
}
