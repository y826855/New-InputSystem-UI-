using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class CSelectableArea_Handler : MonoBehaviour
{
    public CSelectableAreaGroup m_LeftAreas = null;
    public CSelectableAreaGroup m_MidAreas = null;
    public CSelectableAreaGroup m_RightAreas = null;

    public List<CSelectableArea_New> m_Group_CurrScene = new List<CSelectableArea_New>();

    public bool m_IsFocusing = false;

    public List<CSelectableArea_New> m_PrevArea = null;
    public CPlayerInput_Test m_PlayerInput = null;

    public enum ESelectionMode 
    {
        NONE,
        FOCUSING,
        FOCUSING_SOFT,
    }

    public enum ESelectionMode_Field { 
        NONE = 0, 
        FOCUS_SKILL,
        ATTACKING,
        //���� �Է� Ÿ�� �����ϰ�, ���� Ÿ�� �����Ͽ� �Է� ���� �ణ �޶���
    }
    public ESelectionMode_Field m_SelectionMode_Field = ESelectionMode_Field.NONE;

    public void Awake()
    {
        CGameManager.Instance.m_SelectableHandler = this;

        m_Group_CurrScene.Clear();
        m_Group_CurrScene.AddRange(m_MidAreas.m_ChildAreas);
        m_Group_CurrScene.AddRange(m_RightAreas.m_ChildAreas);
        m_Group_CurrScene.AddRange(m_LeftAreas.m_ChildAreas);
    }

    //��â ���̾�
    public void Push_FocusArea(CSelectableArea_New _area) 
    {
        if (m_PlayerInput.m_CurrSelectableArea == null) return;

        m_PrevArea.Add(m_PlayerInput.m_CurrSelectableArea);
        Debug.Log(_area);
        m_PlayerInput.ChangeSelectableArea(_area);
        SelectableArea_FocusIn(_area);
    }

    //���̾� �� â �ݱ�
    public void Pop_FocusArea(CSelectableArea_New _area = null) 
    {
        Debug.Log("POP");

        SelectableArea_FocusOut();

        if (m_PrevArea.Count == 1) 
        {
            m_PlayerInput.ChangeSelectableArea(m_PrevArea[0]);
            m_PrevArea.Clear();
            return;
        }

        int idx = m_PrevArea.Count - 1;
        m_PlayerInput.ChangeSelectableArea(m_PrevArea[idx]);
        m_PrevArea.RemoveAt(idx);
        Debug.Log(idx);
    }

    //��� ���̾�� â �ݱ�
    public void PopAll_FocusArea() 
    {
        if (m_PrevArea.Count == 0) return;

        m_PlayerInput.ChangeSelectableArea(m_PrevArea[0]);
        m_PrevArea.Clear();
    }

    //���� Area�� ��Ŀ�� ��Ű��
    void SelectableArea_FocusIn(CSelectableArea_New _area)
    {
        m_IsFocusing = true;

        foreach (var it in m_Group_CurrScene)
        {
            if (it == null) continue;
            if (it.gameObject.activeInHierarchy == true
                && it != _area)
            { it.Focus_Close(); }
        }
    }

    //Area�� ��Ŀ�� ����
    void SelectableArea_FocusOut(CSelectableArea_New _area = null)
    {
        m_IsFocusing = false;

        foreach (var it in m_Group_CurrScene)
        {
            if (it.gameObject.activeInHierarchy == true)
            { it.Focus_Open(); }
        }

        if (_area != null) _area.Focus_Close();
    }
    ////////////////////////////////


    //�ݱ� �Է�
    public void InputEscape() 
    {
        var currArea = m_PlayerInput.m_CurrSelectableArea;

        //if (currArea.m_Btn_Escape != null) 
        if (currArea.m_IsCanEscape == true && m_IsFocusing == true) 
        {
            currArea.Escape();
            currArea.Close();
            SelectableArea_FocusOut();

            if (m_IsFocusing == true) 
            {
                
            }
        }
            
    }
}
