using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CSelectableAreaGroup : MonoBehaviour
{
    public List<CSelectableArea_New> m_ChildAreas = new List<CSelectableArea_New>();
    [Header("======================================")]
    public CSelectableAreaGroup m_Near_Left = null;
    public CSelectableAreaGroup m_Near_Right = null;
    public CSelectableAreaGroup m_Near_Up = null;
    public CSelectableAreaGroup m_Near_Down = null;

    public bool m_CloseGroup = false;



    public bool m_IsSoftFocusing = false;
    public CSelectableArea_New m_SoftFocus = null;

    public CSelectableArea_New GetOther(CSelectableArea_New _area, CSelectableArea_New.EUI_Move _dir) 
    {
        if (m_ChildAreas.Count == 0) return null;

        if (m_IsSoftFocusing == true && m_SoftFocus != null)
            return m_SoftFocus;

        foreach (var it in m_ChildAreas) 
        {
            if (it.gameObject.activeInHierarchy == true && it.m_CanvasGroup.interactable == true) 
            { return it; }
        }

		switch (_dir)
		{
			case CSelectableArea_New.EUI_Move.LEFT:
                return _area.m_Near_Left;
			case CSelectableArea_New.EUI_Move.RIGHT:
                return _area.m_Near_Right;
            case CSelectableArea_New.EUI_Move.UP:
                return _area.m_Near_Up;
            case CSelectableArea_New.EUI_Move.DOWN:
                return _area.m_Near_Down;
        }

        return null;
	}


    public void CloseGroup() 
    {
        m_CloseGroup = true;

        foreach (var it in m_ChildAreas)
        { if (it.gameObject.activeSelf == true) it.Focus_Close(); }
    }

    public void OpenGroup() 
    {
        m_CloseGroup = false;

        if (CGameManager.Instance.m_SelectableHandler.m_IsFocusing == true) return;

        foreach (var it in m_ChildAreas)
        { if (it.gameObject.activeSelf == true) it.Focus_Open(); }
    }


    //활성화 되어 있는 놈이 있다면, 걔를 반환하게하자.
    //활성화 된게 없다면, 참조한 놈의 방향을 따라가자


    public void SoftFocusIn(CSelectableArea_New _area) 
    {
        foreach (var it in m_ChildAreas)
        {
            if (it == _area)
            { it.Focus_Open(); }
            else it.Focus_Close();
        }
    }

    public void SoftFocusOut()
    {
        if(CGameManager.Instance.m_SelectableHandler.m_IsFocusing)

        foreach (var it in m_ChildAreas)
        { it.Focus_Open(); }
    }


    public void FocusIn_OtherArea()
    {
        //m_CanvasGroup.interactable = false;
        //m_CanvasGroup.blocksRaycasts = false;
        foreach (var it in m_ChildAreas)
        { it.Focus_Open(); }
    }

    public void FocusOut() 
    {
        if (m_IsSoftFocusing == true)
            SoftFocusIn(m_SoftFocus);

        foreach (var it in m_ChildAreas)
        { it.Focus_Open(); }
        //m_CanvasGroup.interactable = true;
        //m_CanvasGroup.blocksRaycasts = true;
    }
}
