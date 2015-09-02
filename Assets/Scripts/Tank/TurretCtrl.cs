using UnityEngine;
using System.Collections;

//탱크의 머리를 컨트롤하는 스크립트

public class TurretCtrl : MonoBehaviour {

    private PhotonView pv = null;
    private Quaternion currRot = Quaternion.identity; //원격 네트워크 터렛 회전값
    private float destYAngle; //마우스 회전값

    public float rotSpeed = 0.0f;  //터렛의 회전 속도

    void Start()
    {
        pv = GetComponent<PhotonView>();
        pv.observed = this; //Photon view 의 observed 속성을 이 스크립트로 지정
        pv.synchronization = ViewSynchronization.Unreliable; //Photon view 의 observed option 속성을 설정
        currRot = transform.localRotation; //회전 초기값 설정
        destYAngle = transform.localRotation.eulerAngles.y;
    }
   
    void Update () {
        if(pv.isMine)
        {
            //↓↓↓ 터렛의 각도 제한하는 부분
            destYAngle += Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed;
            Vector3 rot = transform.localEulerAngles;

            if (destYAngle > 180) destYAngle -= 360;
            if (destYAngle < -90) destYAngle = -90;
            if (destYAngle > 90) destYAngle = 90;
            float angleDelta = Mathf.DeltaAngle(rot.y, destYAngle) * 0.03f;
      
            rot.y += angleDelta;
            transform.localEulerAngles = rot;
            
        }

        else //원격 네트워크 플레이어의 탱크일 경우
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, currRot, Time.deltaTime * 10.0f);  //현재 회전각도에서 수신받은 실시간 회전각도로 부드럽게 회전
        }
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //송수신 콜백함수
    {
        if(stream.isWriting)
        {
            stream.SendNext(transform.localRotation);
        }
        else
        {
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
