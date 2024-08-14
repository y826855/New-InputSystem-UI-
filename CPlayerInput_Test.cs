using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class CPlayerInput_Test : MonoBehaviour
{

    [Header("============================")]
    public RectTransform m_Canvas = null;
    public CDiceHolder m_DiceHolder = null;

    public RectTransform m_Cursor = null;
    public float m_CursorMoveSpeed = 1f;

    public RectTransform m_DiceRollArea = null;

    public RectTransform m_CursorLockArea = null;
    [Header("============================")]
    [SerializeField] Camera m_DiceCamera = null;
    [SerializeField] LayerMask m_Pickable;

    Vector3 m_StartMousePos = Vector3.zero;
    Vector3 m_DragtMousePos = Vector3.zero;
    Transform dragObj = null;


    //public GameObject m_CurrFocus_SelectableArea = null;
    public CSelectableArea_New m_CurrSelectableArea = null;
    public CSelectableArea_New m_PrevSelectableArea = null;

    [SerializeField] Selectable currSelectable = null;
    
    public Selectable m_CurrSelectable
    {
        get  { return currSelectable; }
        set 
        {
            if (value == null || value.gameObject.activeInHierarchy == false) return;
            currSelectable = value;
            ShowTargetCursor();
        }
    }

    [Header("===============SHORT_CUT=================")]
    [SerializeField] Selectable m_Reroll = null;

    public RectTransform m_TargetCursor = null;

    public bool m_IsFocusing = false;
    public bool m_IsHoldCup = false;

    EventSystem eventSystem;

    private void Start()
    {
        eventSystem = EventSystem.current;

        m_CurrSelectable = m_CurrSelectableArea.GetSelectable();
        eventSystem.SetSelectedGameObject(m_CurrSelectable.gameObject);
        ShowTargetCursor();

        CGameManager.Instance.m_PlayerInput = this;
        CGameManager.Instance.m_SelectableHandler.m_PlayerInput = this;
    }

    public void ShowTargetCursor() 
    {
        m_TargetCursor.parent = currSelectable.transform;

        m_TargetCursor.anchorMin = Vector2.zero;
        m_TargetCursor.anchorMax = Vector2.one;
        m_TargetCursor.offsetMin = Vector2.zero;
        m_TargetCursor.offsetMax = Vector2.zero;
        m_TargetCursor.localScale = Vector2.one;

        m_TargetCursor.localRotation = Quaternion.identity;
        var pos = m_TargetCursor.localPosition; pos.z = 0;
        m_TargetCursor.localPosition = pos;
        //m_TargetCursor.localPosition = Vector3.zero;

        
    }

    public void OnInput4Direction(InputAction.CallbackContext _context)
    {
        if (m_IsHoldCup == true) return;

        switch (_context.phase)
        {
            case InputActionPhase.Started: break;
            case InputActionPhase.Performed:
                if (m_CurrSelectable.tag == "Unselectable")
                {
                    Debug.Log("unselectable");
                    m_CurrSelectable = m_CurrSelectableArea.GetSelectable();
                }
                MoveInSelectableArea(_context.ReadValue<Vector2>());
                Debug.Log("INPUT!");
                break;
        }
    }

    //CSelectableArea.EUI_Move uiMove = CSelectableArea.EUI_Move.NONE;
    CSelectableArea_New.EUI_Move uiMove = CSelectableArea_New.EUI_Move.NONE;
    
    //방향 입력으로 UI 커서 움직이기
    public void MoveInSelectableArea(Vector2 _dir)
    {
        Transform p = m_CurrSelectable.transform.parent;

        Selectable next = null;
        if (_dir.y > 0) { next = m_CurrSelectable.FindSelectableOnUp();
            uiMove = CSelectableArea_New.EUI_Move.UP; }
        else if (_dir.y < 0) { next = m_CurrSelectable.FindSelectableOnDown();
            uiMove = CSelectableArea_New.EUI_Move.DOWN; }
        else if (_dir.x > 0) { next = m_CurrSelectable.FindSelectableOnRight();
            uiMove = CSelectableArea_New.EUI_Move.RIGHT; }
        else if (_dir.x < 0) { next = m_CurrSelectable.FindSelectableOnLeft();
            uiMove = CSelectableArea_New.EUI_Move.LEFT; }


        //내부 구간에서 움직일 때
        if (next != null && next.transform.IsChildOf(m_CurrSelectableArea.transform) == true)
        {
            m_CurrSelectable = next;
            m_CurrSelectableArea.m_LastInput = next;
            eventSystem.SetSelectedGameObject(m_CurrSelectable.gameObject);
        }

        //포커싱 창 변경
        else if(m_IsFocusing == false)
        {
            
            //var nextGroup = m_CurrSelectableArea.GetNextGroup(uiMove);
            var nextGroup = m_CurrSelectableArea.Found_Area_InDirection(uiMove);
            Debug.Log(nextGroup);

            if (nextGroup != null)
            {
                m_CurrSelectableArea = nextGroup;
                m_CurrSelectable = m_CurrSelectableArea.GetSelectable(next);

                if (m_CurrSelectable != null) eventSystem.SetSelectedGameObject(m_CurrSelectable.gameObject);
                //else m_CurrSelectable = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
            }
        }

        //스크롤 뷰 내부를 움직일때 스크롤 움직이게 함
        if (m_CurrSelectableArea.m_ScrollView != null)
        {
            var scroll = m_CurrSelectableArea.m_ScrollView;
            var itemRect = m_CurrSelectable.GetComponent<RectTransform>();
            float height = scroll.content.rect.height - itemRect.rect.height;
            scroll.verticalScrollbar.value = 1f + itemRect.anchoredPosition.y / height;

            Debug.Log("anchoredY " + m_CurrSelectable.GetComponent<RectTransform>().anchoredPosition.y);
            Debug.Log("height " + height);
            Debug.Log("scrollVal " + scroll.verticalScrollbar.value);
        }
    }

    //tab, Pad Y 누르면
    public void OnViewDetail(InputAction.CallbackContext _context) 
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                CGameManager.Instance.m_DetailManager.ShowDetailClass(m_CurrSelectable);
                break;
        }
    }

    //F, Pad X 누르면
    public void OnInteraction(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                var btn = m_CurrSelectable.GetComponent<ISelectEvent>();
                if (btn != null) 
                {
                    Debug.Log("INTER ACTION");
                    btn.OnInteraction(); 
                }
                Debug.Log("F");
                break;
        }
    }

    //확인 버튼
    public void OnSubmit(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                m_CurrSelectable.OnSelect(null);
                break;
        }

    }

    //포커싱 중인 오브젝트에서 나가기
    public void OnEscape(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Performed)
        {
            Debug.Log("escape");
            var selectable = m_CurrSelectable.GetComponent<ISelectEvent>();
            if (selectable != null) selectable.OnInputEscape();

            CGameManager.Instance.m_SelectableHandler.InputEscape();

            //if (m_PrevSelectableArea != null && m_IsFocusing == true)
            //{
            //    m_CurrSelectableArea.Close();
            //    m_CurrSelectableArea = m_PrevSelectableArea;
            //    m_CurrSelectable = m_CurrSelectableArea.GetSelectable();
            //    m_PrevSelectableArea = null;
            //    CGameManager.Instance.SelectableArea_FocusOut();
            //}
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////CURSOR////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////
    public void OnHoldCup(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                if (m_CurrSelectable.tag == "Cup") 
                {
                    Debug.Log("HOLD CUP");
                    m_CursorLockArea = m_DiceRollArea;
                    m_Cursor.gameObject.SetActive(true);
                    m_Cursor.position = m_DiceCamera.WorldToScreenPoint(m_CurrSelectable.transform.position);
                    RayCastOnCursor();

                    m_CurrSelectable = m_Reroll;
                    FocusToSelectable();

                    m_DiceHolder.m_DiceManager.
                        m_DiceChoiceArea.m_SelectableArea.m_CanvasGroup.interactable = false;
                    //m_IsHoldCup = true;
                }
                break;


            case InputActionPhase.Canceled:
                if (m_IsHoldCup == true)
                {
                    Debug.Log("STOP HOLD");
                    m_IsHoldCup = false;
                    m_Cursor.gameObject.SetActive(false);
                    ThrowDice();
                }
                break;
        }
    }

    //커서 움직임 입력
    public void OnCursorMove(InputAction.CallbackContext _context)
    {
        if (m_IsHoldCup == true)
        {
            cursorMove = _context.ReadValue<Vector2>();
            if (coCursorMove == null &&
                cursorMove != Vector2.zero) coCursorMove = StartCoroutine(CoCursorMove());
        }
    }

    Vector2 cursorMove = Vector2.zero;
    Coroutine coCursorMove = null;

    IEnumerator CoCursorMove()
    {
        Vector2 boundaryMin = Vector2.zero; // 수정된 부분
        Vector2 boundaryMax = m_DiceRollArea.rect.size - m_Cursor.rect.size; // 수정된 부분
        

        while (cursorMove != Vector2.zero)
        {
            if (m_CursorLockArea == null)
            { m_Cursor.anchoredPosition += (cursorMove * Time.deltaTime * m_CursorMoveSpeed); }

            else
            {//커서 범위 잠금
                Rect uiBounds = m_CursorLockArea.rect;
                float cx = m_CursorLockArea.position.x;
                float cy = m_CursorLockArea.position.z;

                Vector2 movePos = m_Cursor.anchoredPosition + (cursorMove * Time.deltaTime * m_CursorMoveSpeed);

                movePos.x = Mathf.Clamp(movePos.x, uiBounds.xMin + cx, uiBounds.xMax + cx);
                movePos.y = Mathf.Clamp(movePos.y, uiBounds.yMin + cy, uiBounds.yMax + cy);

                //m_Cursor.anchoredPosition = movePos;
                m_Cursor.anchoredPosition = movePos;

                //Debug.Log(cursorMove);
            }

            yield return null;
        }

        coCursorMove = null;
    }

    public void OnMouseMove(InputAction.CallbackContext _context) 
    {
        if (m_Cursor.gameObject.activeSelf == false) m_Cursor.gameObject.SetActive(true);

        var pos = _context.ReadValue<Vector2>();
        
        //TODO : 해상도 바뀌면 못쓰는 코드
        pos.y -= 1080;

        m_Cursor.anchoredPosition = pos;
    }

    public void OnMouseLClick(InputAction.CallbackContext _context) 
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                //Debug.Log("CLICK");
                RayCastOnCursor();
                break;
            case InputActionPhase.Canceled:
                //Debug.Log("CLICK DONE");
                if (m_IsHoldCup == true)
                {
                    ThrowDice();
                    m_IsHoldCup = false;
                }
                break;
        }

        FindParentArea();
    }


    public void RayCastOnCursor()
    {
        //if (m_CursorMode == true)
        {
            m_StartMousePos = m_DiceCamera.ScreenToWorldPoint(m_Cursor.position);
            m_DragtMousePos = m_StartMousePos;
            var ray = m_DiceCamera.ScreenPointToRay(m_Cursor.position);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, m_Pickable, QueryTriggerInteraction.Collide) == true)
            {
                if (hit.collider.tag == "Cup")
                {
                    Debug.Log("CUP!");

                    dragObj = hit.collider.transform;
                    if (coDrag == null) coDrag = StartCoroutine(CoDrag(dragObj));
                    m_CursorLockArea = m_DiceRollArea;
                    m_IsHoldCup = true;
                    GrabCup();
                }
            }

        }
    }

    Coroutine coDrag = null;
    IEnumerator CoDrag(Transform _dragObj)
    {
        Vector3 currPos = Vector3.zero;

        while (true)
        {
            currPos = m_DiceCamera.ScreenToWorldPoint(m_Cursor.position);
            _dragObj.position += currPos - m_DragtMousePos;
            m_DragtMousePos = currPos;
            yield return null;
        }
    }

    //선택된 selectable의 부모 클래스 찾기
    public void FindParentArea()
    {
        var currObj = eventSystem.currentSelectedGameObject;
        if (currObj == null || currObj.tag == "Unselectable") return;
        var selectable = currObj.GetComponent<Selectable>();
        //var area = selectable.GetComponentInParent<CSelectableArea>();
        var area = selectable.GetComponentInParent<CSelectableArea_New>();
        if (area == null || area.m_CanvasGroup.interactable == false) return;
        m_CurrSelectable = selectable;
        m_CurrSelectableArea = area;
    }

    public void ChangeSelectableArea(CSelectableArea_New _area) 
    {
        m_CurrSelectableArea = _area;
        if (_area.m_ChildSelectables.Contains(m_CurrSelectable) == false) 
        { m_CurrSelectable = _area.GetSelectable(); }
    }

    public void FocusToSelectable() 
    //{ m_CurrSelectableArea = m_CurrSelectable.GetComponentInParent<CSelectableArea>(); }
    { m_CurrSelectableArea = m_CurrSelectable.GetComponentInParent<CSelectableArea_New>(); }

    //public void FocusOnWindow(CSelectableArea _area) 
    public void FocusOnWindow(CSelectableArea_New _area) 
    {
        if (m_CurrSelectableArea == _area) return;
        
        _area.m_CanvasGroup.blocksRaycasts = true;
        _area.m_CanvasGroup.interactable = true;

        m_PrevSelectableArea = m_CurrSelectableArea;
        m_CurrSelectableArea = _area;
        
        m_CurrSelectable = m_CurrSelectableArea.GetSelectable();
        m_IsFocusing = true;

        //CGameManager.Instance.SelectableArea_FocusIn(_area);
    }

    //셀렉터블 변경. 주사위 눌렀을때 오른쪽 놈으로 바뀌는거 구현
    public void ChangeSelectable(Selectable _selectable) 
    {
        if (_selectable == null) return;
        eventSystem.SetSelectedGameObject(_selectable.gameObject);
        m_CurrSelectable = _selectable;
    }

    /// <summary>
    /// /////////////////////////////
    /// </summary>
    public void GrabCup()
    {
        m_DiceHolder.GrabCup();
    }

    public void ThrowDice()
    {
        if (coDrag != null) StopCoroutine(coDrag);
        coDrag = null;
        m_DiceHolder.ThrowDice();
    }
    /// //////////////////////////////// /////////////////////////////


    public void OnDebug_Input(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                CGameManager.Instance.m_TurnManager.m_PlayerChar.TestATK();
                break;
        }
    }
}
