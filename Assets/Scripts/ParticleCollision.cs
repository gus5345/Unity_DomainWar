using UnityEngine;
using System.Collections;

// 맞은 후 발생하는 파티클 , 죽었을때 발생하는 파티클을 동기화 하기 위한 스크립트

public class ParticleCollision : MonoBehaviour {

    private PhotonView pv = null;  //PhotonView 컴포넌트를 할당할 변수
    //위치 정보를 송수신할 떄 사용할 변수 선언 및 초깃값 설정
    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;
    int parentViewid; // 부모의 photonView 아이디

    void Awake()
    {
        pv = GetComponent<PhotonView>(); //사용할 컴포넌트 변수에 할당
        currPos = transform.position;
        currRot = transform.rotation;
	}
    void Update()
    {
        if(pv.isMine)
        {
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 10.0f); //원격 플레이어의 탱크 수신을 받은 위치까지 부드럽게 이동시킴
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 10.0f); //원격 플레이어의 탱크를 수신받은 각도만큼 부드럽게 회전시킴 
        }
	}
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { 
        if (stream.isWriting) //로컬 플레이어의 위치 정보 수신
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.parent.GetComponent<PhotonView>().viewID); // 부모의 PhotonView 아이디를 동기화함
        }
        else // 원격 플레이어의 위치 정보 수신
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            parentViewid = (int)stream.ReceiveNext();
            transform.parent = PhotonView.Find(parentViewid).transform; // 받은 PhotonView 아이디를 찾아 , 그것을 부모로한다.
        }
    }
}
