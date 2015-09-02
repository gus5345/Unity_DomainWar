using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
 // try , cateh 쓸때 필요

// 탱크가 맞았을시 작동하는 스크립트

public class TankDamage : MonoBehaviour
{
    private HitPoint hitPoint; // 스크립트 선언,정의를 함으로써 여러번 찾는걸 방지
    private TankMove tankMove;
    public TurretCtrl turretCtrl;
    public CannonCtrl cannonCtrl;
    private FireCannon fireCannon;
    public GameMgr mar;
    TankPosition tankPosition;
    private GameObject zoomImage; // 죽고나서 줌 이미지를 끄기 위한 변수
    private bool damaged; // 중복 데미지를 주는 것을 방지하기 위한변수

    private PhotonView pv = null; //Photon View 변수
    public Transform camPivot; //메인 카메라가 추적할 목표
    
    
    public RotateSkyBox rotator; // 줌이미지에 있는 것을 다시 메인으로 돌려주기 위한 변수 , 스카이박스의 타겟을
    public GameObject expEffect = null; //탱크 폭발 효과 프리팹을 연결할 변수
    public GameObject collisionFire; // 맞았을시 나타나는 파티클
    public Canvas hudCanvas; //탱크 이름,HP 담고있는 곳
    public Text textHp; // 탱크 HP 숫자 표기
    public Text respawn; // 리스폰 텍스트
    public Text respawnTextTime; // 리스폰 시간 텍스트
    public Text killDeathLoMsg; // 누가 누구를 죽였는지 로그출력할때 쓰는 텍스트
    public RectTransform scrollContents; // 로그창을 올리기 위한 스크롤
    

    public int tankHp = 100; //탱크의 초기 생명력
    public int nowtankHp = 0; //탱크의 현재 생명력
    public Vector3 attackPosition; // 맞은 위치
    public float respawnTime = 0; // 실질적 리스폰 시간
    public int intRespawnTime = 0; //Time.deltaTime int형 바로 변환시 0 으로만 표현되, 두번 나눠서 캐스팅하는 변수
    int roomKill = 0; // 목표 킬수
    float hpColor = 1; // 체력의 양에 따라 색깔을 바꿔주는 변수
    bool dieTest = false; // 죽었는지 확인하는 변수
    int smokeNumber = 0;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        mar = GameObject.Find("GameManager").GetComponent<GameMgr>();
        tankPosition = GameObject.Find("GameManager").GetComponent<TankPosition>();
        killDeathLoMsg = GameObject.Find("KillDeathLoMsg ").GetComponent<Text>();
        scrollContents = GameObject.Find("KDLoMsgScroll").GetComponent<RectTransform>();
        roomKill = (int)PhotonNetwork.room.customProperties["aimKill"];
        rotator = GameObject.Find("SkyCamera").GetComponent<RotateSkyBox>();
        textHp = GameObject.Find("HP").GetComponent<Text>();
        nowtankHp = tankHp; //현재 생명력을 초기 생명력으로 초기화
        textHp.text = nowtankHp.ToString();
        textHp.color = Color.green;
    }
    void Start()
    {
        hitPoint = GetComponent<HitPoint>();
        tankMove = GetComponent<TankMove>();
        fireCannon = GetComponent<FireCannon>();
        zoomImage = GameObject.Find("ZoomImage");
        respawn = GameObject.Find("Respawn").GetComponent<Text>();
        respawnTextTime = GameObject.Find("RespawnTime").GetComponent<Text>();

        if (pv.isMine)
        {
            Camera.main.GetComponent<SmoothCam>().target = camPivot;  //메인 카메라에 추가된 스크립트에 추적 대상을 연결
        }
    }
    void Update()
    {
        if (pv.isMine)
        {
            //↓↓↓↓ 자신의 HP가 0이 됐을때 화면에 띄워주는 부분 시간초
            if (!GameMgr.reset)
            {
                if (nowtankHp <= 0)
                {
                    respawn.enabled = true;
                    respawnTextTime.enabled = true;
                    respawnTime += Time.deltaTime;
                    intRespawnTime = (int)respawnTime;
                    respawnTextTime.text = intRespawnTime.ToString() + "초";
                }
                else
                {
                    respawnTime = 0;
                }
            }
        }
        else return;
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        stream.Serialize(ref nowtankHp); // 한번에 두개를 다 처리하는 부분 , HP 동기화 부분

        //↓↓↓↓↓ HP게이지와 텍스트 HP를 HP양의 따라 색깔별로 구별하는 부분
        if(pv.isMine) // 자기 자신만 하게끔 , 이걸 안하면 다른 플레이어의 HP가 한번 들어옴
        {
            textHp.text = nowtankHp.ToString();
            hpColor = (float)nowtankHp / (float)tankHp;
            if (hpColor <= 0.4f)
            {
                textHp.color = Color.red;
            }
            else if (hpColor <= 0.6f)
            {
                textHp.color = Color.magenta;
            }
            else if (hpColor <= 1.0f)
            {
                textHp.color = Color.green;
            }
        }
    }
    public void ClearDamaged()
    {
        damaged = false; // 한번에 여러번 데미지 주는것을 방지하기 위한 부분
    }
    public void DoDamage(int damage, Vector3 position, Vector3 attackPosition, int photonViewId)
    {
        if (damaged) return;
        damaged = true;
        //↓↓↓ 원래는 각 포탄마다의 데미지가 있고, 설정할 수 있으나, 재미없어 랜덤으로 적용하였다.
        // 부분마다 데미지를 달리 줄려고 했으나, 탱크라는 요소를 살리다보니 탱크가 다른 곳에 맞아도 데미지는 같을 것 같아 그냥 랜덤으로 구현하였다.
        float randomDamage = Random.Range(30.0f, 60.0f);
        damage = (int)randomDamage;
        pv.RPC("DoDamageRPC", PhotonTargets.All, damage, position, attackPosition, photonViewId);
    }
 
    [RPC]void DoDamageRPC(int damage, Vector3 position, Vector3 attackPosition, int photonViewId)
    {
        if (!dieTest)
        {
            if (pv.ownerId != photonViewId) // 자기 자신이 자폭하는것을 막기위한 것
            {
                nowtankHp -= damage;
                if (pv.isMine)
                {
                    Collider[] colliders = GetComponentsInChildren<Collider>();
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        smokeNumber++;
                        if (smokeNumber < 3)
                        {
                            //↓↓↓ 나중에 들어온 클라이언트에게는 맞았을때 맞은위치에 발생하는 파티클이 보이지않아, 무리가 됨에도 불과하고 탱크처럼 동기화함
                            // pv.isMine 때문에 자기 자신 화면에서만 부모가 되기때문에 ParticleCollision 스크립트에서 OnPhotonSerializeView로 부모를 동기화한다.
                            PhotonNetwork.Instantiate("CollisionSmoke", ((Collider)colliders.GetValue(i)).ClosestPointOnBounds(position), Quaternion.identity, 0).transform.parent = transform;
                        }
                    }
                    //↓↓↓ 맞았을때 쏜 탱크의 쏜 위치를 찾아 그 방향으로 hitPoint를 띄워주기위해 값을찾아 전달하는 부분
                    Vector3 thisPosition = transform.InverseTransformPoint(attackPosition);
                    float angle = Mathf.Atan2(thisPosition.x, thisPosition.z) * Mathf.Rad2Deg;
                    hitPoint.attackRotation = angle;
                    hitPoint.start = true; // 맞았을시 화면 가운데를 중심으로 상대 초기값을 표현
                }
            }
            if (nowtankHp <= 0)
            {
                nowtankHp = 0;
                dieTest = true;
                NoControl();
                //↓↓↓ 킬수를 더하는 부분
                if (PhotonNetwork.player.GetTeam() != pv.owner.GetTeam()) // 자기 자신과 같은 팀이 아니라면
                {
                    if (PhotonNetwork.player.ID == photonViewId) // 자기 자신 ID 와 쏜 탱크의 ID 비교, 같다면
                    {
                        AddKill();
                        string msg = "\n<color=#FF0084>[처치] [" + PhotonNetwork.player.name + "]  </color>" + "<color=#FF4500>[죽음] [" + pv.owner.name + "]</color>";
                        pv.RPC("KDMsg", PhotonTargets.All, msg);
                    }
                }
                mar.SetKD(); // 킬데스 추가
                mar.UserSetScore(); // 스코어 갱신부분
            }
        }

        
        if (PhotonNetwork.player.GetTeam() != pv.owner.GetTeam()) // 상대를 맞췄을때 미니맵에서 상대를 3초간 보이게하는 부분
        {
            transform.FindChild("MinMapPoint").GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine(MinMapPlayerEnabled());
        }
    
    }
    //[RPC] void Parenting(int child, int parent)
    //{
    //    PhotonView.Find(child).transform.parent = PhotonView.Find(parent).transform;
    //}
    [RPC] void KDMsg(string Msg) // 죽이거나, 죽었을시 로그를 출력하는 부분
    {
        killDeathLoMsg.text = killDeathLoMsg.text + Msg; //로그 메시지 Text에 누적시켜 표시
        var height = killDeathLoMsg.rectTransform.rect.height;
        scrollContents.sizeDelta = new Vector2(0, 20 + height);
    }
    void AddKill()
    {
        ++GameObject.Find("GameManager").GetComponent<GameMgr>().playerKill;
    }
    void NoControl() // 죽었을시 컨트롤 못하게 하는 함수
    {
        // 바퀴 회전력과 , 파티클 , 리스폰시 피격 포인트 제어
        tankMove.torque = 0; // 회전력
        tankMove.Braking(true); // 멈추는 함수의 매개변수 전달
        tankMove.lTrackAnim.direction = 0; //죽었을시 자동으로 움직이는 탱크의 이동을 방지하는 것
        tankMove.rTrackAnim.direction = 0; 
        
        if (pv.isMine)
        {
            PhotonNetwork.Instantiate("DieFire", transform.position, Quaternion.identity, 0).transform.parent = transform; //폭발 효과 생성 , 생성과 동시에 부모를 정함
            ++GameObject.Find("GameManager").GetComponent<GameMgr>().playerDie;

            hitPoint.start = false; // 재생성시 처음에 피격 이미지 뜨는것을 방지하기 위한 것
            cannonCtrl.mainCamera.camera.enabled = true; // 메인 켜고
            cannonCtrl.zoomCamera.camera.enabled = false; // 줌 끄기

            tankMove.enabled = false; // 탱크 이동스크립트 끄기
            fireCannon.enabled = false;  // 탱크 포탄 스크립트 끄기
            turretCtrl.enabled = false; // 탱크 터렛 컨트롤 스크립트 끄기
            cannonCtrl.enabled = false; // 탱크 케논 스크립트 끄기
            hitPoint.enabled = false; // 탱크 피격시 어디서 쐇는지 위치표현하는 스크립트 끄기
            zoomImage.renderer.enabled = false; // 줌 이지미 끄기
        }
        StartCoroutine("NewTank"); // 탱크 새로 생성하는 부분
    }
    public void GameEnd() // 게임이 끝났을시 호출되는 함수
    {
        StopCoroutine("NewTank"); //코루팀 함수 막기
        tankMove.torque = 0;
        tankMove.Braking(true);
        tankMove.lTrackAnim.direction = 0;
        tankMove.rTrackAnim.direction = 0;
        tankMove.audio.volume = 0;

        if (pv.isMine)
        {
            cannonCtrl.mainCamera.camera.enabled = true;
            cannonCtrl.zoomCamera.camera.enabled = false;
            hitPoint.start = false;
            rotator.enabled = false;
            tankMove.enabled = false;
            fireCannon.enabled = false;
            turretCtrl.enabled = false;
            cannonCtrl.enabled = false;
            hitPoint.enabled = false;
            zoomImage.renderer.enabled = false;
        }
    }
    [RPC] void EvenyoneNewTank() // 새로 들어온 클라이언트 때문에 RPC로만듬
    {
        tankMove.audioTest = false;
        nowtankHp = tankHp; // 리스폰 시 생명력 초기화
        textHp.color = Color.green;
        dieTest = false;
        smokeNumber = 0;
        tankPosition.pvID = pv.ownerId;
        tankPosition.completion = false;
        tankPosition.TankPositionChoice = 1;

        Transform[] trans = GetComponentsInChildren<Transform>(); // 맞았을때 발생하는 파티클, 죽었을때 발생하는 파티클 제거하는 부분
        for (int i = 0; i < trans.Length; i++)
        {
            if(trans[i].tag == "CollisionSmoke")
            {
                Destroy(trans[i].gameObject);
            }
        }
        if (pv.isMine)
        {
            cannonCtrl.rotator.targetCamera = cannonCtrl.mainCamera;
            cannonCtrl.CannonMesh.enabled = true;
            respawn.enabled = false;
            respawnTextTime.enabled = false;
            tankMove.enabled = true;
            fireCannon.enabled = true;
            turretCtrl.enabled = true;
            cannonCtrl.enabled = true;
            hitPoint.enabled = true;
            GetComponent<StartTank>().enabled = true;
        }
    }
    IEnumerator NewTank()
    {
        yield return new WaitForSeconds(15.0f);
        pv.RPC("EvenyoneNewTank", PhotonTargets.All); // 탱크가 죽은후, 들어온 클라한테 죽은 파티클을 삭제시키기 위해 RPC로 호출
    }
    IEnumerator MinMapPlayerEnabled() // 상대를 맞췄을때 3초후 미니맵 다시 끄는 부분
    {
        yield return new WaitForSeconds(3.0f);
        transform.FindChild("MinMapPoint").GetComponent<MeshRenderer>().enabled = false;
    }
}