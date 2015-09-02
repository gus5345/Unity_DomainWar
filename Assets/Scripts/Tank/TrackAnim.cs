using UnityEngine;
using System.Collections;

// 애니메이션을 따로 만든 것이 아니라, 바퀴의 텍스쳐를 돌려주는 스크립트

public class TrackAnim : MonoBehaviour
{
    public float direction = 1; //텍스처의 이동 방향 1:전진 / -1 : 후진 / 0 : 정지
    public float scrollSpeed = 1.0f; //텍스처의 회전 속도

    void Update()
    {
        var offset = Time.time * scrollSpeed * direction;
        renderer.material.SetTextureOffset("_MainTex", new Vector2(0, -offset)); //기본 텍스처의 Y 오프셋 값 변경
        // 텍스쳐가 뒤집어져 있어 , -로 처리함
        //renderer.material.SetTextureOffset("_BumpMap", new Vector2(0, offset)); //노말 텍스처의 Y 오프셋 값 변경
    }
}
