using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public RectTransform joystickBG;
    [SerializeField] private RectTransform joystickHandle;

    private Vector2 inputVector;
    private Canvas canvas;

    public bool canMove = true;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        joystickBG.gameObject.SetActive(false);
    }

    //조이스틱 생성
    public void OnClickDown(PointerEventData eventData)
    {
        gameObject.SetActive(true);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out pos);
        joystickBG.anchoredPosition = pos;
    }


    //조이스틱 위치 정보 생성
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickBG, Input.mousePosition, null, out pos);
            pos.x /= joystickBG.sizeDelta.x;
            pos.y /= joystickBG.sizeDelta.y;
            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            joystickHandle.anchoredPosition =
                new Vector2(inputVector.x * (joystickBG.sizeDelta.x / 2),
                            inputVector.y * (joystickBG.sizeDelta.y / 2));
        }
        if (Input.GetMouseButtonUp(0))
        {
            inputVector = Vector2.zero;
            joystickHandle.anchoredPosition = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    public float Horizontal() => canMove ? inputVector.x : 0;
    public float Vertical() => canMove ? inputVector.y : 0;
    public Vector2 Direction() => canMove ? inputVector : Vector2.zero;
}
