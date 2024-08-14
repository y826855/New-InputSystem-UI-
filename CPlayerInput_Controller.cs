using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CPlayerInput_Controller : MonoBehaviour
{
    [SerializeField] PlayerInput m_PlayerInput = null;
    [SerializeField] Selectable currSelectable = null;


    [SerializeField] bool m_CursorMode = true;

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


    EventSystem eventSystem;

    private void Start()
    {
        eventSystem = EventSystem.current;

        currSelectable = GetFirstSelectable();
        eventSystem.SetSelectedGameObject(currSelectable.gameObject);

        //CGameManager.Instance.m_PlayerInput = this;
    }

    //public void OnInput4Direction(InputValue _value)
    public void OnInput4Direction(InputAction.CallbackContext _context)
    {
        Debug.Log(_context.ReadValue<Vector2>());

        switch (_context.phase) 
        {
            case InputActionPhase.Started: break;
            case InputActionPhase.Performed:

                StartCoroutine(CoAfterSelectable());
                //Debug.Log(eventSystem.currentSelectedGameObject?.GetComponent<Selectable>());
                break;
        }
    }

    IEnumerator CoAfterSelectable() 
    {
        yield return null;
        currSelectable = eventSystem.currentSelectedGameObject?.GetComponent<Selectable>();
        //currSelectable = eventSystem.currentSelectedGameObject?.GetComponent<Selectable>();
        //m_Cursor.transform.position = currSelectable.transform.position;
    }

    public void ChangeSelectable(Selectable _selectable) 
    {
        Debug.Log(_selectable.name);

        if (_selectable == null) return;
        eventSystem.SetSelectedGameObject(_selectable.gameObject);
        currSelectable = _selectable;
    }

    public Selectable GetCurrSelectable() 
    { return currSelectable; }

    //press gamepad R Trigger, keyboard enter
    public void OnSubmit(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Started: break;
            case InputActionPhase.Performed:
                RayCastOnCursor();
                break;

            case InputActionPhase.Canceled:
            case InputActionPhase.Disabled:
                ThrowDice();
                m_CursorLockArea = null;
                break;
        }

        if (currSelectable != null)
        {

        }
    }

    //�����ɽ�Ʈ
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
                    dragObj = hit.collider.transform;
                    if (coDrag == null) coDrag = StartCoroutine(CoDrag(dragObj));
                    m_CursorLockArea = m_DiceRollArea;
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

    //Ŀ�� ������ �Է�
    //public void OnCursorMove(InputValue _value)
    public void OnCursorMove(InputAction.CallbackContext _context)
    {
        cursorMove = _context.ReadValue<Vector2>();
        if (coCursorMove == null &&
            cursorMove != Vector2.zero) coCursorMove = StartCoroutine(CoCursorMove());
    }

    Vector2 cursorMove = Vector2.zero;
    Coroutine coCursorMove = null;

    IEnumerator CoCursorMove() 
    {
        Vector2 boundaryMin = Vector2.zero; // ������ �κ�
        Vector2 boundaryMax = m_DiceRollArea.rect.size - m_Cursor.rect.size; // ������ �κ�

        while (cursorMove != Vector2.zero)
        {
            if (m_CursorLockArea == null) 
            { m_Cursor.anchoredPosition += (cursorMove * Time.deltaTime * m_CursorMoveSpeed); }
            
            else 
            {//Ŀ�� ���� ���
                Rect uiBounds = m_CursorLockArea.rect;
                float cx = m_CursorLockArea.position.x;
                float cy = m_CursorLockArea.position.z;

                Vector2 movePos = m_Cursor.anchoredPosition + (cursorMove * Time.deltaTime * m_CursorMoveSpeed);

                movePos.x = Mathf.Clamp(movePos.x, uiBounds.xMin + cx, uiBounds.xMax + cx);
                movePos.y = Mathf.Clamp(movePos.y, uiBounds.yMin + cy, uiBounds.yMax + cy);

                //m_Cursor.anchoredPosition = movePos;
                m_Cursor.anchoredPosition = movePos;
            }

            yield return null;
        }

        coCursorMove = null;
    }

    IEnumerator CoCursorMoveHold() 
    {
        yield return null;
    }


    public void OnMouseMove(InputAction.CallbackContext _context) 
    //���콺 �Է� �������� �۵�
    {
        var pos = _context.ReadValue<Vector2>();
        pos.y -= 1080;
        m_Cursor.anchoredPosition = pos;
        
    }

    public void OnDebug_DiceRoll()
    {
        m_DiceHolder.GrabCup();
        m_DiceHolder.ThrowDice();
    }

    private void ExecuteUIAction()
    {
        // ���� ���õ� UI ��ҿ��� ��ȣ�ۿ��� �����ϴ� �ڵ� �ۼ�
        Selectable currentSelectable = eventSystem.currentSelectedGameObject?.GetComponent<Selectable>();
        if (currentSelectable)
        {
            currentSelectable.OnSelect(null);
            //currentSelectable.OnSubmit(null);
        }
    }

    //TODO : ���콺�� ������ or ������ ��ǲ�ѳ� ���� �����ؾ���
    private Selectable GetFirstSelectable()
    {
        // ù ��° ���� ������ ��� ��ȯ
        return eventSystem.firstSelectedGameObject.GetComponent<Selectable>();
    }
}