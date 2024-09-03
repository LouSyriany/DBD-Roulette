using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class MiddleScrollRect : ScrollRect
{
    Vector3 startPos = Vector3.zero;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Vector3 startPos = Input.mousePosition;
            eventData.button = PointerEventData.InputButton.Left;
            base.OnBeginDrag(eventData);
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnEndDrag(eventData);
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnEndDrag(eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if((Input.mousePosition - startPos).magnitude > .5f)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnDrag(eventData);
            }
        }

        //if (eventData.button == PointerEventData.InputButton.Middle)
        //{
        //    eventData.button = PointerEventData.InputButton.Left;
        //    base.OnDrag(eventData);
        //}

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnDrag(eventData);
        }
    }
}