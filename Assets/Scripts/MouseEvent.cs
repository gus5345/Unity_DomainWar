using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//이벤트 훅으로 사용할 스크립트
//UI에서 마우스가 클릭시 포탄이 나가는것을 방지하기 위한 것

public class MouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public bool isUIHover = false; // 마우스 커서의 ui항목에 대한 Hover 여부

    public void OnPointerEnter(PointerEventData data)
    {
        isUIHover = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        isUIHover = false;
    }
}
