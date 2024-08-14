using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CSelectableArea : MonoBehaviour
{
	public CanvasGroup m_CanvasGroup = null;

	[Header("======================================")]
	public CSelectableArea m_Near_Left = null;
	public CSelectableArea m_Near_Right = null;
	public CSelectableArea m_Near_Up = null;
	public CSelectableArea m_Near_Down = null;
	[Header("======================================")]
	public Selectable m_LastInput = null;
	public List<Selectable> m_ChildSelectables = new List<Selectable>();

	public GameObject m_ParentArea = null;
	public ScrollRect m_ScrollView = null;
	public Image m_Disable = null;


    private void Start()
    {

	}
    private void OnEnable()
	{
		if (m_CanvasGroup == null) m_CanvasGroup = this.GetComponent<CanvasGroup>();
		m_CanvasGroup.interactable = true;
	}

	private void OnDisable()
	{
		m_CanvasGroup.interactable = false;
	}



	public enum EUI_Move { NONE, LEFT, RIGHT, UP, DOWN }

	public CSelectableArea GetNextGroup(EUI_Move _dir) 
	{
		switch (_dir) 
		{
			case EUI_Move.LEFT:
				if (m_Near_Left == null) return null;
				else if (m_Near_Left.gameObject.activeSelf == true
					&& m_Near_Left.m_CanvasGroup.interactable == true) return m_Near_Left;
				else if (m_Near_Left.m_Near_Left != null) { return m_Near_Left.GetNextGroup(_dir); }
				else return null;
			case EUI_Move.RIGHT:
				if (m_Near_Right == null) return null;
				else  if (m_Near_Right.gameObject.activeSelf == true
					&& m_Near_Right.m_CanvasGroup.interactable == true) return m_Near_Right;
				else if (m_Near_Right.m_Near_Right != null) { return m_Near_Right.GetNextGroup(_dir); }
				else return null;
			case EUI_Move.UP:
				if (m_Near_Up == null) return null;
				else if (m_Near_Up.gameObject.activeSelf == true
					&& m_Near_Up.m_CanvasGroup.interactable == true) return m_Near_Up;
				else if (m_Near_Up.m_Near_Up != null) { return m_Near_Up.GetNextGroup(_dir); }
				else return null;
			case EUI_Move.DOWN:
				if (m_Near_Down == null) return null;
				else if (m_Near_Down.gameObject.activeSelf == true
					&& m_Near_Down.m_CanvasGroup.interactable == true) return m_Near_Down;
				else if (m_Near_Down.m_Near_Down != null) { return m_Near_Down.GetNextGroup(_dir); }
				else return null;
		}

		return null;
	}

	//화면 옮길때 마지막으로 입력된 놈이나 배열 첫번째 selectable 들고옴
	public Selectable GetSelectable() 
	{
		if (m_LastInput != null && m_LastInput.gameObject.activeInHierarchy == true)
			return m_LastInput;

		foreach (var it in m_ChildSelectables)
			if (it.gameObject.activeInHierarchy == true) return it;

		return null;
	}

	//화면 옮기는지 참조하는 놈이 있다면
	public Selectable GetSelectable(Selectable _focus) 
	{
		if (m_ChildSelectables.Contains(_focus) == true)
			return _focus;
		else 
			return GetSelectable();
	}
	public virtual void FocusIn_OtherArea()
	{
		m_CanvasGroup.interactable = false;
		m_CanvasGroup.blocksRaycasts = false;
	}
	public virtual void FocusOut()
	{
		m_CanvasGroup.interactable = true;
		m_CanvasGroup.blocksRaycasts = true;
	}

	public virtual void Close() 
	{
		if (m_ParentArea != null)
		{ m_ParentArea.SetActive(false); }
		this.gameObject.SetActive(false);
	}

	public virtual void Open() 
	{
		if (m_ParentArea != null)
		{ m_ParentArea.SetActive(true); }
		this.gameObject.SetActive(true);
	}
}
