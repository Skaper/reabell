using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class VRInput : BaseInputModule
{
    [SerializeField] private VRPointer pointer = null;
    public PointerEventData Data { get; private set; } = null;

    private GameObject m_CurrentObject = null;
    


    protected override void Awake()
    {
        Data = new PointerEventData(eventSystem);
        Data.position = new Vector2(pointer.Camera.pixelWidth / 2, pointer.Camera.pixelHeight / 2);

    }
    public override void Process()
    {

        eventSystem.RaycastAll(Data, m_RaycastResultCache);
        Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);

        HandlePointerExitAndEnter(Data, Data.pointerCurrentRaycast.gameObject);

        ExecuteEvents.Execute(Data.pointerDrag, Data, ExecuteEvents.dragHandler);

        if (Input.GetKeyDown(InputManager.Attack1))
            Press();

        if (Input.GetKeyUp(InputManager.Attack1))
            Release();
    }



    public void Press()
    {
        Data.pointerPressRaycast = Data.pointerCurrentRaycast;

        Data.pointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(Data.pointerPressRaycast.gameObject);
        Data.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(Data.pointerPressRaycast.gameObject);

        ExecuteEvents.Execute(Data.pointerPress, Data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(Data.pointerDrag, Data, ExecuteEvents.beginDragHandler);

    }

    public void Release()
    {
        GameObject pointerRelease = ExecuteEvents.GetEventHandler<IPointerClickHandler>(Data.pointerCurrentRaycast.gameObject);
        
        if (Data.pointerPress == pointerRelease)
            ExecuteEvents.Execute(Data.pointerPress, Data, ExecuteEvents.pointerClickHandler);
        
        ExecuteEvents.Execute(Data.pointerPress, Data, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(Data.pointerDrag, Data, ExecuteEvents.endDragHandler);

        Data.pointerDrag = null;
        Data.pointerPress = null;
        Data.pointerCurrentRaycast.Clear();
    }
}
