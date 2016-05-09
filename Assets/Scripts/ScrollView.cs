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
		contents = new Content[6];
		for(int i = 0; i < 6; ++i)
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
			size_delta.y = originContent.sizeDelta.y*contentsNum;
			rect_trans.sizeDelta = size_delta;
		}

		for(int i = 0; i < contents.Length; ++i)
		{
			contents[i].UpdateContent(i);
		}
	}

	void OnScroll(Vector2 scroll_ratio)
	{
		currentIndex = (int)(view.anchoredPosition.y / originContent.sizeDelta.y);
		for(int i = 0; i < contents.Length; ++i)
		{
			contents[i].calculated = false;
		}
		for(int i = currentIndex; i < currentIndex + contents.Length; ++i)
		{
			if(i >= contentsNum) continue;
			var content = Array.Find(contents, item => item.index == i);
			if(content != null)
			{
				content.calculated = true;
				continue;
			}
			content = Array.Find(contents, item => item.index < currentIndex || item.index > currentIndex + contents.Length-1);
			if(content == null) continue;
			var rect_trans = content.transform as RectTransform;
			var pos = rect_trans.anchoredPosition;
			pos.y = -content.size.y * i;
			rect_trans.anchoredPosition = pos;
			content.UpdateContent(i);
			content.calculated = true;
		}
	}
}
