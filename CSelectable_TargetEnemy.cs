using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public class CSelectable_TargetEnemy : CSelectableArea
public class CSelectable_TargetEnemy : CSelectableArea_New
{
    public int m_SelectCount = 0;

    public TMPro.TextMeshProUGUI m_TMP_Count = null;

    public CPlayerChar m_Player = null;
    public CUI_SkillCard m_UI_SkillCard = null;
    //public System.Action m_CB_UseSkill = null;

    public bool m_IsCanSelect_ForSkill = false;

    //적 생성
    public void SpawnEnemy(CMonster _monster) 
    {
        m_ChildSelectables.Add(_monster.m_UI_Target.m_Selectable);

    }

    //적 제거
    public void RemoveEnemy(CMonster _monster) 
    {
        m_ChildSelectables.Add(_monster.m_UI_Target.m_Selectable);
    }

    //선택 모드 진입
    public void OnSelectMode(int _count) 
    {
        CGameManager.Instance.m_SelectableHandler.m_SelectionMode_Field
            = CSelectableArea_Handler.ESelectionMode_Field.FOCUS_SKILL;

        m_IsCanSelect_ForSkill = true;

        m_SelectCount = _count;
        m_TMP_Count.text = m_SelectCount.ToString();
        m_TMP_Count.gameObject.SetActive(true);
        Open();
    }

    //적 선택
    public void SelectedEnemy(CHitable _target) 
    {
        if (m_IsCanSelect_ForSkill == false) return;

        m_SelectCount--;
        m_TMP_Count.text = m_SelectCount.ToString();

        _target.m_Field_Info.AddTarget();
        m_Player.m_SkillTargets.Add(_target);
        Debug.Log("ON TARGET!");

        if (m_SelectCount <= 0) 
        { SelectAllTarget(); }
    }

    //모든 타겟 선택
    public void SelectAllTarget() 
    {
        //if (m_CB_UseSkill != null) m_CB_UseSkill();
        //Close();

        m_UI_SkillCard.UseSkill();
        m_TMP_Count.gameObject.SetActive(false);

        m_Parent.CloseGroup();
        CGameManager.Instance.m_SelectableHandler.Pop_FocusArea();

        m_IsCanSelect_ForSkill = false;
    }

    //공격 종료
    public void AttackDone() 
    {
        //Close();
        //CGameManager.Instance.SelectableArea_FocusOut(this);
        //m_Parent.SoftFocusIn(this);
        Debug.Log("DONE CHECK" + m_Player.m_IsEndAttack);

        m_Parent.OpenGroup();
        if (m_Player.m_IsEndAttack == false)
        {
            m_Player.m_IsEndAttack = true;
            return;
        }

        Debug.Log("DONE");

        ClearTargetTextIcon();
        Destroy(m_Player.m_Hitter.gameObject);
    }

    //창 포커싱 시작
    public override void Open()
    {
        //m_CanvasGroup.interactable = true;
        //m_CanvasGroup.blocksRaycasts = true;
        m_IsCanSelect_ForSkill = true;
    }

    //창 포커싱 종료
    public override void Close()
    {
        //m_CanvasGroup.interactable = false;
        //m_CanvasGroup.blocksRaycasts = false;
        m_IsCanSelect_ForSkill = false;
        m_SelectCount = 100;
        m_TMP_Count.gameObject.SetActive(false);

        //CGameManager.Instance.m_SelectableHandler.m_SelectionMode_Field
        //    = CSelectableArea_Handler.ESelectionMode_Field.NONE;
    }

    //모든 타겟팅 표시 제거
    public void ClearTargetTextIcon() 
    {
        foreach (var it in m_Player.m_SkillTargets)
            it.m_Field_Info.ClearTarget();
        m_Player.m_SkillTargets.Clear();
        m_Player.m_IsEndAttack = false;
    }

    //포커싱 종료
    public override void Focus_Close()
    {
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;
    }

    //포커스가 풀려도 활성화 안되게 함
    public override void Focus_Open()
    {
        m_CanvasGroup.interactable = true;
        m_CanvasGroup.blocksRaycasts = true;
    }

    //포커싱 탈출
    public override void Escape()
    {
        if (m_IsCanSelect_ForSkill == false) return;

        m_UI_SkillCard.OnInputEscape();
        //reset targets
        foreach (var it in m_Player.m_SkillTargets) 
        { it.m_Field_Info.RemoveTarget(); }

        m_Player.m_SkillTargets.Clear();
    }
}
