using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// 内部要素を使いまわす軽量版スクロールリスト
/// 要素の表示サイズは固定サイズ
/// </summary>
public class ScrollList : MonoBehaviour
{
	[System.Serializable]
	public class OnUpdateElement : UnityEngine.Events.UnityEvent<ListElement> {}
	public OnUpdateElement onUpdateElement = new OnUpdateElement();

	public enum ScrollType
	{
		Horizontal,
		Vertical,
	}
	public ScrollType scrollType = ScrollType.Horizontal;

	public float scrollViewSize
	{
		get
		{
			return scrollType == ScrollType.Horizontal ? scrollTransform.sizeDelta.x : scrollTransform.sizeDelta.y;
		}
	}
	public float elementSize
	{
		get
		{
			return scrollType == ScrollType.Horizontal ? originElement.sizeDelta.x : originElement.sizeDelta.y;
		}
	}

	/// <summary>
	/// 要素数変更
	/// </summary>
	public void SetElementNum(int element_num)
	{
		if(elementNum == element_num) return;
		elementNum = element_num;
		UpdateViewSize();
	}
	public int GetElementNum()
	{
		return elementNum;
	}

	/// <summary>
	/// スクロール位置を設定
	/// </summary>
	/// <param name="normalized_pos">正規化されたスクロール値</param>
	public void SetNormalizedPosition(Vector2 normalized_pos)
	{
		scrollRect.normalizedPosition = normalized_pos;
	}

	// internal --------------------------------------------------------------------------------------------------------

	[SerializeField] RectTransform contentsXform;
	[SerializeField] RectTransform originElement;

	[SerializeField] int elementNum;
	[SerializeField] int columnNum = 1;

	class ManagedElement
	{
		public bool calculated = false;
		public ListElement element;
	}

	int currentIndex = 0;
	ScrollRect scrollRect;
	RectTransform scrollTransform;
	ManagedElement[] elements;

	void Awake()
	{
		scrollRect = GetComponent<ScrollRect>();
		if(scrollRect == null) scrollRect = gameObject.AddComponent<ScrollRect>();
		scrollRect.content = contentsXform;
		scrollRect.onValueChanged.AddListener(OnScroll);
		scrollRect.horizontal = scrollType == ScrollType.Horizontal;
		scrollRect.vertical = scrollType == ScrollType.Vertical;
		scrollTransform = scrollRect.transform as RectTransform;

		var element_num = (int)(scrollViewSize / elementSize) * (columnNum+1);
		// var element_num = (int)(scrollViewSize / elementSize)+2;
		elements = new ManagedElement[element_num];
		for(int i = 0; i < elements.Length; ++i)
		{
			var element_obj = Instantiate(originElement);
			var rect_trans = element_obj.transform as RectTransform;
			rect_trans.SetParent(contentsXform, false);
			elements[i] = new ManagedElement(){ element = element_obj.GetComponent<ListElement>(), calculated = false };
			elements[i].element.UpdateIndex(-1);
			if(i >= elementNum) continue;
			RelocateElement(i, elements[i]);
		}
		for(int i = 0; i < elements.Length; ++i)
		{
			if(elements[i].calculated) continue;
			elements[i].element.gameObject.SetActive(false);
		}
		originElement.gameObject.SetActive(false);

		UpdateViewSize();
	}

	void UpdateViewSize()
	{
		var size_delta = contentsXform.sizeDelta;
		var row_num = Mathf.CeilToInt((float)elementNum / columnNum);
		if(scrollType == ScrollType.Horizontal) size_delta.x = elementSize * row_num;
		else                                    size_delta.y = elementSize * row_num;
		contentsXform.sizeDelta = size_delta;

		UpdateElements();
	}

	void OnScroll(Vector2 scroll_ratio)
	{
		UpdateElements();
	}
	void UpdateElements()
	{
		var scroll_pos = 0.0f;
		if(scrollType == ScrollType.Horizontal)
		{
			scroll_pos = -contentsXform.anchoredPosition.x;
			if(contentsXform.pivot.x >= 1.0f) scroll_pos = Mathf.Abs(contentsXform.sizeDelta.x) - Mathf.Abs(scroll_pos);
        }
		else
		{
			scroll_pos = contentsXform.anchoredPosition.y;
			if(contentsXform.pivot.y <= 0.0f) scroll_pos = Mathf.Abs(contentsXform.sizeDelta.y) - Mathf.Abs(scroll_pos);
		}

		currentIndex = (int)(scroll_pos / elementSize);
		if(currentIndex < 0) currentIndex = 0;
		for(int i = 0; i < elements.Length; ++i)
		{
			elements[i].calculated = false;
		}
		for(int i = currentIndex; i < currentIndex + elements.Length; ++i)
		{
			if(i >= elementNum) continue;
			// 既にある要素はそのまま
			var element = Array.Find(elements, item => item.element.index == i);
			if(element != null)
			{
				element.calculated = true;
				// 消えてたやつは再配置する
				if(element.element.gameObject.activeSelf == false)
				{
					RelocateElement(i, element);
				}
				continue;
			}
			// 範囲外の要素を再利用
			element = Array.Find(
				elements,
				item => item.element.index < currentIndex || item.element.index > currentIndex + elements.Length-1
			);
			if(element == null) continue;
			RelocateElement(i, element);
		}
		// 未使用要素は非表示
		for(int i = 0; i < elements.Length; ++i)
		{
			if(elements[i].calculated) continue;
			elements[i].element.gameObject.SetActive(false);
		}
	}

	void RelocateElement(int new_index, ManagedElement element)
	{
		var rect_trans = element.element.xform;
		var pos = rect_trans.anchoredPosition;
		var column_num = new_index % columnNum;
		var row_num = new_index / columnNum;
		if(scrollType == ScrollType.Horizontal) pos.x = elementSize * new_index;
		else
		{
			pos.x = originElement.sizeDelta.x * column_num;
			pos.y = -elementSize * row_num;
		}
		rect_trans.anchoredPosition = pos;
		element.element.UpdateIndex(new_index);
		onUpdateElement.Invoke(element.element);
		element.calculated = true;
		element.element.gameObject.SetActive(true);
	}
}
