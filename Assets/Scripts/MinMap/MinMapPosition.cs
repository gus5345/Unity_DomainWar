using UnityEngine;
using System.Collections;

// 미니맵의 위치가 유니티의 해상도가 변경되어도 항상 같은 위치에 있게끔하는 스크립트

public class MinMapPosition : MonoBehaviour
{
    public Camera uiCamera;
        // w = h * aspect
        // h = uiCamera.orthographicSize
        // aspect = w/h

	void Update () {
        float width = uiCamera.orthographicSize * uiCamera.aspect; // 넓이
        float x = width - 1.5f; // x쪽으로 위치
        float y = -uiCamera.orthographicSize + 1.5f; // y쪽으로의 위치
        transform.position = new Vector3(x, y, 0); // 적용 (오른쪽 하단)
	}
}
