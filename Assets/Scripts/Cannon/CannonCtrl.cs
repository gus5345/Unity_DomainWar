using UnityEngine;
using System.Collections;

// 포신을 상하 움죽이게 해주는 스크립트

public class CannonCtrl : MonoBehaviour
{
    private PhotonView pv = null;  //Photon View 변수
    private Quaternion currRot = Quaternion.identity;  //원격 네트워크 탱크의 포신 각도 변수
    private GameObject _renderer; //카메라 줌 이미지 
    private bool cameraZoom = false;
    private float destYAngle; // 마우스에 따라 이동하는 각도를 조절하기 위한 변수

    public float rotSpeed = 0.0f; // 회전 속도
    public GameObject zoomCamera;
    public GameObject mainCamera;
    public RotateSkyBox rotator; // 스카이박스의 회전
    public MeshRenderer CannonMesh; // cannon 메쉬

    void Awake ()
    {
        pv = GetComponent<PhotonView>(); 
        pv.observed = this; //PhotonView 의 observed 속성을 이 스크립트로 지정
        pv.synchronization = ViewSynchronization.Unreliable;  //PhotonView 의 observed option 속성설정
        currRot = transform.localRotation;   //회전 초기값 설정
    }
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        rotator = GameObject.Find("SkyCamera").GetComponent<RotateSkyBox>();
        _renderer = GameObject.Find("ZoomImage");
        _renderer.renderer.enabled = false;
    }
    void Update () {
        if (pv.isMine)   //자신의 탱크일때만 조정
        {
            if (Input.GetMouseButtonDown(1)) //마우스 우클릭시 줌카메라,메인카메라 변경
            {
                cameraZoom = !cameraZoom;
                _renderer.renderer.enabled = cameraZoom;
                CannonMesh.enabled = !cameraZoom;
                mainCamera.camera.enabled = !cameraZoom;
                zoomCamera.camera.enabled = cameraZoom;
                rotator.targetCamera = cameraZoom ? zoomCamera : mainCamera; //스카이 박스 카메라 타겟카메라를 변경
            }
            destYAngle -= Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed;
            if (destYAngle > 7) destYAngle = 7;
            else if (destYAngle < -30) destYAngle = -30;  // cannon의 높이 조절
            
            Vector3 rot = transform.localEulerAngles;
            float angleDelta = Mathf.DeltaAngle(rot.x, destYAngle) * 0.03f;
            rot.x += angleDelta;
            transform.localEulerAngles = rot;
        }
        //현재 회전각도에서 수신받은 실시간 회전 각도로 부드럽게 회전
        else transform.localRotation = Quaternion.Slerp(transform.localRotation, currRot,Time.deltaTime * 10.0f);
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // 동기화 
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
