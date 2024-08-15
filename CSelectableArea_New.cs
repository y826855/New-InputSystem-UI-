using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CSelectableArea_New : MonoBehaviour
{
	public CanvasGroup m_CanvasGroup = null;
	public CSelectableAreaGroup m_Parent = null;

	[Header("======================================")]
	public CSelectableArea_New m_Near_Left = null;
	public CSelectableArea_New m_Near_Right = null;
	public CSelectableArea_New m_Near_Up = null;
	public CSelectableArea_New m_Near_Down = null;
	[Header("======================================")]
	public Selectable m_LastInput = null;
	public List<Selectable> m_ChildSelectables = new List<Selectable>();
	public bool m_IsCanEscape = false;
	public Button m_Btn_Escape = null;
	//public bool m_IsEscapeable = false;

	/// 
	[Header("======================================")]
	public GameObject m_ParentArea = null;
	public ScrollRect m_ScrollView = null;
	public Image m_Disable = null;
	[Header("======================================")]
	//������ �Է��� ã�� �Ұ��� 
	[SerializeField] bool m_IsGoto_NearSelectable = false;

    private void Awake()
    {
		if (m_Btn_Escape != null) m_IsCanEscape = true;
	}

    //�̵� ���� 
    public enum EUI_Move { NONE, LEFT, RIGHT, UP, DOWN }
	public CSelectableArea_New Found_Area_InDirection(EUI_Move _dir) 
	{//�׺���̼��� ���� �ش���� ��ư ã��
		if(m_Parent != null) m_Parent.GetOther(this, _dir);

		switch (_dir)
		{
			case CSelectableArea_New.EUI_Move.LEFT:
				return CheckDir(m_Near_Left, _dir);
			case CSelectableArea_New.EUI_Move.RIGHT: 
				return CheckDir(m_Near_Right, _dir);
			case CSelectableArea_New.EUI_Move.UP:
				return CheckDir(m_Near_Up, _dir);
			case CSelectableArea_New.EUI_Move.DOWN:
				return CheckDir(m_Near_Down, _dir);
		}

		return null;
	}

	//�Է¹��� ������ ��ư ã��
	public CSelectableArea_New CheckDir(CSelectableArea_New _area, EUI_Move _dir)
	{
		if (_area == null) return null;
		else if (_area.gameObject.activeInHierarchy == false
			|| _area.m_CanvasGroup.interactable == false)
		{
			if (_area.m_Parent == this.m_Parent)
				return _area.Found_Area_InDirection(_dir);
			else
				return _area.m_Parent.GetOther(_area, _dir);
		}
		else return _area;
	}


	//ȭ�� �ű涧 ���������� �Էµ� ���̳� �迭 ù��° selectable ����
	public Selectable GetSelectable()
	{
		if (m_LastInput != null && m_LastInput.gameObject.activeInHierarchy == true)
			return m_LastInput;

		foreach (var it in m_ChildSelectables)
			if (it.gameObject.activeInHierarchy == true) return it;

		return null;
	}

	//ȭ�� �ű���� �����ϴ� ���� �ִٸ�
	public Selectable GetSelectable(Selectable _focus)
	{
		//������ �Է� �޾ƿ�
		if (m_IsGoto_NearSelectable == false) return GetSelectable();
		
		//�ڵ����� ����� ���� �ִٸ� ��ȯ, ���ٸ� ������ �Է��̳� �������� ó��
		if (m_ChildSelectables.Contains(_focus) == true)
			return _focus;
		else
			return GetSelectable();
	}

	//��Ŀ�� ����
	public virtual void Focus_Close()
	{
		m_CanvasGroup.interactable = false;
		m_CanvasGroup.blocksRaycasts = false;
	}
	//��Ŀ�� ����
	public virtual void Focus_Open()
	{
		if (m_Parent != null && m_Parent.m_CloseGroup == true) 
			return;

		m_CanvasGroup.interactable = true;
		m_CanvasGroup.blocksRaycasts = true;
	}
	//â �ݱ�
	public virtual void Close()
	{
		if (m_ParentArea != null)
		{ m_ParentArea.SetActive(false); }
		this.gameObject.SetActive(false);
	}
	//â ����
	public virtual void Open()
	{
		if (m_ParentArea != null)
		{ m_ParentArea.SetActive(true); }
		this.gameObject.SetActive(true);
	}


	//�ݱ� �Է�
	public virtual void Escape() 
	{
		if(m_Btn_Escape != null) m_Btn_Escape.onClick.Invoke();
	}
}
