using UnityEngine;
using System.Collections;

//메인카메라 스크립트

public class SmoothCam : MonoBehaviour
{
    public Transform target; //카메라가 따라가는 객체
    public float distance = 0.0f; //객체와 카메라의 거리
    public Vector3 offset;  // 카메라 상대적 높이
    public float rotationDamping = 0.0f;   //카메라 이동속도

    void LateUpdate()
    {
        if (!target)
            return;
        //변경되는 최종 방향값(타겟의 방향값)
        float wantedRotationAngle = target.eulerAngles.y; 
        float wantedRotationAngle2 = target.eulerAngles.x;
        //현재 카메라의 방향값
        float currentRotationAngle = transform.eulerAngles.y;
        float currentRotationAngle2 = transform.eulerAngles.x;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        currentRotationAngle2 = Mathf.LerpAngle(currentRotationAngle2, wantedRotationAngle2, rotationDamping * Time.deltaTime);
        Quaternion currentRotation = Quaternion.Euler(currentRotationAngle2, currentRotationAngle, 0);

        transform.position = target.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        transform.position += offset;
        //카메라는 바라본다 (타겟)
        transform.LookAt(target.position + offset); 
                                               
        //메인카메라에서 마우스 커서의 위치로 캐스팅되는 Ray를 생성
        Ray ray = new Ray();
        ray.origin = target.position + offset; //목표점
        Vector3 relative = transform.position - ray.origin; // 시작점 . 방향
        ray.direction = relative;

        RaycastHit hit; //카메라가 이동할 곳
        if (Physics.Raycast(ray, out hit, relative.magnitude + 0.5f, (1 << 8))) // Map,Terrain 8번 레이어에 위치하기 때문에, 카메라가 장애물에 닿았을시 유저쪽으로 이동
        {
            Vector3 dist = hit.point - ray.origin;
            float distlen = dist.magnitude;
            transform.position = ray.origin + dist * ((distlen - 0.5f) / distlen);
        }
    }
}
