using UnityEngine;
using System.Collections;

// 스카이 박스를 움직이게 하는 스크립트
// 카메라를 하나 더 설치해서 그것을 메인 카메라의 맞게끔 회전시켜주고 있다.

public class RotateSkyBox : MonoBehaviour
{
    private float rotateY; // 조명의 각도
    public float rotateSpeed; // 하늘 움직이는 속도
    public GameObject targetCamera; // 현재 작동중인 카메라
    public GameObject rootLight; // 조명
  
    void OnPreRender ()
    {
        if(targetCamera != null) 
        {
            rotateY += rotateSpeed * Time.deltaTime; //항상 균인하게 움직이게하기 위해 
            camera.fieldOfView = targetCamera.camera.fieldOfView; // skyBox 카메라를 현재 타겟 카메라에 맞춤
            transform.rotation = targetCamera.transform.rotation; 

            Vector3 angle = rootLight.transform.eulerAngles; //조명의 각도를 구해서
            angle.y = -rotateY; // 현재 스카이박스 움직임에 따라 이동하게바꿈
            rootLight.transform.eulerAngles = angle; // 적용
            transform.eulerAngles += new Vector3(0, rotateY, 0);   
        }
    }
}