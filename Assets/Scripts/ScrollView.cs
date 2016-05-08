using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScrollView : MonoBehaviour
{
	[SerializeField] RectTransform view;
	[SerializeField] RectTransform originContent;

	[SerializeField] int contentsNum;

	int currentIndex = 0;
	ScrollRect scrollRect;
	Content[] contents;

	void Awake()
	{
		scrollRect = GetComponent<ScrollRect>();
		scrollRect.onValueChanged.AddListener(OnScroll);
	}

	void Start()
	{
		contents = new Content[4];
		for(int i = 0; i < 4; ++i)
		{
			var content = Instantiate(originContent);
			var rect_trans = content.transform as RectTransform;
			rect_trans.SetParent(view, false);
			contents[i] = content.GetComponent<Content>();
			var pos = rect_trans.anchoredPosition;
			pos.y = -rect_trans.sizeDelta.y * i;
			rect_trans.anchoredPosition = pos;
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

	void OnScroll(Vector2 pos)
	{
		var content = Array.Find(contents, item => item.index == currentIndex);
		if(view.anchoredPosition.y > (content.size.y * (content.index+1)))
		{
			var index = content.index + contentsNum;
			// var rect_trans = content.transform as RectTransform;
			// var pos = rect_trans.anchoredPosition;
			content.UpdateContent(index);
			currentIndex++;
			return;
		}
	}
}
