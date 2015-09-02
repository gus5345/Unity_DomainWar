using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//탱크를 움직이게하는 스크립트

public class TankMove : MonoBehaviour
{
    private PhotonView pv = null;  //PhotonView 컴포넌트를 할당할 변수
    //위치 정보를 송수신할 떄 사용할 변수 선언 및 초깃값 설정
    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;
    //키보드 입력 값
    private float h = 0.0f;
    private float v = 0.0f;
    //wheel 의 토크와 방향
    [HideInInspector]
    public float torque = 0.0f;
    private float steerAngle = 40.0f;
    private bool tankRotation = false;


    public float maxSpeed = 0.0f; //탱크 최대 속도
    public float rotSpeed = 0.0f; //회전 최대 속도
    //각 바퀴
    public WheelCollider FLWheel;
    public WheelCollider FRWheel;
    public WheelCollider RLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RCWheel;
    public WheelCollider LCWheel;
    public ParticleSystem palticle;
    public ParticleSystem palticleTwo;
    public TrackAnim lTrackAnim; //탱크의 무한궤도의 애니메이션 처리르 위한 변수
    public TrackAnim rTrackAnim;
    public Text fKey;
    private float audioSound = 0;
    public bool audioTest = false;

    void Awake()
    {
        pv = GetComponent<PhotonView>(); //사용할 컴포넌트 변수에 할당
        fKey = GameObject.Find("TankTurnOut").GetComponent<Text>();

        //pv.observed = this;
        if (pv.isMine)
        {
            rigidbody.centerOfMass = new Vector3(-1.0f, 0.0f, 5.5f); //탱크의 중심 설정
            //↑ 모델의 중심이 맞지않아, 무게중심이 뒤쪽으로 치우쳐져 있어 방향전환이 되지않아 강제로 모델의 중심을 수정함
        }
        else
        {
            rigidbody.isKinematic = true;  //원격 네트워크 탱크 물리력 이용하지 않음
            GetComponent<AudioListener>().enabled = false; // 원격 네트워크 탱크의 사운드 리스너를 사용하지 않음
        }
        
        ////원격 탱크의 위치 및 회전 값을 처리하는 변수의 초깃값 설정
        //currPos = transform.position;
        //currRot = transform.rotation;
    }
    void Start()
    {
        StartCoroutine("StartAudio");
    }
    void FixedUpdate()
    {
        if (!pv.isMine) return;
        
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (audioTest == true)
        {
            audioSound = rigidbody.velocity.magnitude / maxSpeed;
            
        }
        else
        {
            audioSound = 0;
        }
        audio.volume = audioSound;
        //탱크의 최대 속도 제한
        if (rigidbody.velocity.magnitude < maxSpeed) torque = maxSpeed * v; //탱크의 속도가 최대 속도 이하일 경우 계속 가속
        else torque = 0.0f; //탱크의 속도가 최대 속도일 경우 회전력을 제거

        float angle = this.steerAngle * h;     //좌우 화살표 키로 회전 각도를 계산

        //제자리에서 회전할 경우
        if (Mathf.Abs(v) < 0.1f && Mathf.Abs(h) > 0.1f)
        {
            FLWheel.steerAngle = 0;
            FRWheel.steerAngle = 0;

            float leftTorque = 0.0f; //왼쪽 바퀴의 회전력 변수
            float rightTorque = 0.0f; //오른쪽 바퀴의 회전력 변수
            float sideTorque = maxSpeed * Mathf.Abs(h) / 2; //제자리 회전의 경우 조금 더 증가시킴

            //회전 방향에 따라 각 바퀴의 각도를 변경
            FLWheel.steerAngle = 0; //angle;
            FRWheel.steerAngle = 0; //angle;
            RLWheel.steerAngle = 0; //-angle;
            RRWheel.steerAngle = 0; //-angle;

            //왼쪽으로 회전할 경우의 회전력 계산
            //왼쪽 바퀴는 역방향 회전 / 오른쪽 바퀴는 정방향 회전
            if (h <= -0.1f)
            {
                leftTorque = -sideTorque;
                rightTorque = +sideTorque;
            }
            //오른쪽으로 회전할 경우의 회전력 계산
            //왼쪽 바퀴는 정방향 회전 / 오른쪽 바퀴는 역방향 회전
            if (h >= 0.1f)
            {
                leftTorque = +sideTorque;
                rightTorque = -sideTorque;
            }
            //각 바퀴에 회전력을 전달
            FLWheel.motorTorque = leftTorque;
            LCWheel.motorTorque = leftTorque;
            RLWheel.motorTorque = leftTorque;
            FRWheel.motorTorque = rightTorque;
            RCWheel.motorTorque = rightTorque;
            RRWheel.motorTorque = rightTorque;
        }
        else
        {
            //이동하며 회전할 경우 앞바퀴만 방향을 조절
            FLWheel.steerAngle = angle;
            FRWheel.steerAngle = angle;
            RLWheel.steerAngle = 0;
            RRWheel.steerAngle = 0;
            RCWheel.steerAngle = 0;
            LCWheel.steerAngle = 0;

            //각 바퀴에 동일한 회전력을 전달
            FLWheel.motorTorque = torque;
            FRWheel.motorTorque = torque;
            RLWheel.motorTorque = torque;
            RRWheel.motorTorque = torque;

            RCWheel.motorTorque = torque;
            LCWheel.motorTorque = torque;
        }

        lTrackAnim.direction = v; // v 변수값으로 전/후진을 판단
        rTrackAnim.direction = v; // v 변수값으로 전/후진을 판단
        Braking(Mathf.Abs(v) <= 0.1f && Mathf.Abs(h) <= 0.1f); //키보드 입력이 없을 경우 정지력 전달
    }
    public void Braking(bool isBrake) //속도 감속을 위한 브레이크 로직
    {
        float emrate = isBrake ? 0 : 100;
        palticle.emissionRate = emrate;
        palticleTwo.emissionRate = emrate;

        float brakeVal = isBrake ? 50.0f : 0.0f;
        //각 Wheel에 브레이크 힘 설정
        FLWheel.brakeTorque = brakeVal;
        FRWheel.brakeTorque = brakeVal;
        RLWheel.brakeTorque = brakeVal;
        RRWheel.brakeTorque = brakeVal;

        RCWheel.brakeTorque = brakeVal;
        LCWheel.brakeTorque = brakeVal;
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) //로컬 플레이어의 위치 정보 수신
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(palticle.emissionRate); // 바퀴 연기 파티클
            stream.SendNext(lTrackAnim.direction); // 바퀴 애니메이션
            stream.SendNext(rTrackAnim.direction);
            stream.SendNext(audio.volume);
        }
        else // 원격 플레이어의 위치 정보 수신
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            palticle.emissionRate = (float)stream.ReceiveNext();
            palticleTwo.emissionRate = palticle.emissionRate;
            lTrackAnim.direction = (float)stream.ReceiveNext();
            rTrackAnim.direction = (float)stream.ReceiveNext();
            audio.volume = (float)stream.ReceiveNext(); // 볼륨
        }
    }
    void Update()
    {
        if (pv.isMine)
        {
            Vector3 tankNormal = transform.rotation * Vector3.up; // up.Vector 만듬으로써 탱크가 뒤집어졌는지 판단하는 변수

            if (tankNormal.y <= 0) // 뒤집어졌다면
            {
                GetComponent<FireCannon>().enabled = false; // 포사격 끄기
                palticle.enableEmission = false; // 파티클 끄기
                palticleTwo.enableEmission = false;
                fKey.enabled = true; // f키 누루면 뒤집어진다고 알려주는 변수 켜기

                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (tankRotation) return;
                    tankRotation = true;
                    fKey.enabled = false;
                    StartCoroutine(TimeStop()); // 2초후
                }
            }
            else
            {
                GetComponent<FireCannon>().enabled = true;
                palticle.enableEmission = true;
                palticleTwo.enableEmission = true;
                tankRotation = false;
                fKey.enabled = false;
            }
        }
        else // 원격 플레이어의 수행
        {
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 10.0f); //원격 플레이어의 탱크 수신을 받은 위치까지 부드럽게 이동시킴
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 10.0f); //원격 플레이어의 탱크를 수신받은 각도만큼 부드럽게 회전시킴
        }
    }

    IEnumerator TimeStop()
    {
        yield return new WaitForSeconds(2.0f);
        transform.rotation = Quaternion.identity;
        GetComponent<FireCannon>().enabled = true;
        palticle.enableEmission = true;
        palticleTwo.enableEmission = true;
    }
    IEnumerator StartAudio()
    {
        yield return new WaitForSeconds(0.7f);
        audioTest = true;
    }
    public void callPosition(Vector3 pos)
    {
        transform.position = pos;
        pv.RPC("NewTankPosition", PhotonTargets.Others, pos);
        StartCoroutine("StartAudio");
    }
    [RPC] void NewTankPosition(Vector3 pos)
    {
        if(pv.isMine)
        {

        }
        else
        {
            currPos = pos;
            transform.position = pos;   
        }
    }
}
