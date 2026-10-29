using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;

    private Vector2 inputVector;
    private Vector2 joystickCenter;

    public float moveRadius = 100f;
    public float moveThreshold = 1f;


    public Vector2 InputDirection
    {
        get
        {
            const float deadZone = 0.1f;
            if (inputVector.magnitude < deadZone) return Vector2.zero;

            if (Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y))
                return new Vector2(Mathf.Sign(inputVector.x), 0f);
            else
                return new Vector2(0f, Mathf.Sign(inputVector.y));
        }
    }

    void Start()
    {
        background.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 anchoredPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out anchoredPos))
        {
            background.anchoredPosition = anchoredPos;
        }

        background.gameObject.SetActive(true);
        handle.anchoredPosition = Vector2.zero;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out position))
        {
            float magnitude = position.magnitude / moveRadius;
            Vector2 normalised = position.normalized;

            if (magnitude > moveThreshold)
            {
                Vector2 difference = normalised * (magnitude - moveThreshold) * moveRadius;
                background.anchoredPosition += difference;
            }

            position = position / moveRadius;
            inputVector = (position.magnitude > 1.0f) ? position.normalized : position;
            handle.anchoredPosition = inputVector * moveRadius;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false);
    }

    public void ForceReset()
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false);
    }
}