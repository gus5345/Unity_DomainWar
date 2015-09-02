using UnityEngine;
using System.Collections;

// 포탄을 쏘는 스크립트

public class FireCannon : MonoBehaviour
{
    private PhotonView pv = null; //Photon View 변수
    private int rebound = 0; // 반동

    public GameObject bulletGameObject;
    public GameObject MuzzleFlash; // 발사와 동시에 포신 입구 파티클
    public GameObject Cannon; //포신
    public Transform firePos; //공격 발사 지점
    public Collider turretCollider;
    public MouseEvent mouseEvent; // 마우스 객체 할당 변수
    public MouseEvent mouseEventTwo; // 마우스 객체 할당 변수

    void Start()
    {
        pv = GetComponent<PhotonView>();
        mouseEvent = GameObject.Find("OnOff").GetComponent<MouseEvent>(); // 마우스 훅 스크립트를 가지고있는 게임오브젝트
        mouseEventTwo = GameObject.Find("Chatting").GetComponent<MouseEvent>(); // 마우스 훅 스크립트를 가지고있는 게임오브젝트
    }

    void Update() {
        if (!pv.isMine) return; // 자기 자신이 아니라면 나가라
     
        Vector3 oldCannon = Cannon.transform.localEulerAngles; // 케논의 로컬 각도
        float SIZE = Random.Range(-10.0f, -15.0f); // 반동 각도

        BulletManager bm = BulletManager.instance; // 싱글톤 디자인패턴 적용하는 부분

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            bm.selectBullet(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            bm.selectBullet(1);
        }
        BulletManager.Bullet sel = bm.selected;
        if (!mouseEvent.isUIHover && !mouseEventTwo .isUIHover && Input.GetMouseButtonDown(0) && Time.time > sel.nextFire)
        {
            sel.nextFire = Time.time + sel.fireRate;
            pv.RPC("Fire", PhotonTargets.All, sel.id); //모두에게 실행하라는 명령
            oldCannon.x += SIZE;
        }
        if (rebound < 100) //반동 후 서서히 내리는부분 
        {
            rebound++;
            oldCannon.x += 0.1f;
            Cannon.transform.localEulerAngles = oldCannon;
        }
    }
    [RPC]void Fire(int no)
    {
        BulletManager bm = BulletManager.instance;
        BulletManager.Bullet sbul = bm.bullet[no];
        GameObject b = (GameObject)Instantiate(bulletGameObject, firePos.position, firePos.rotation);
        Cannon cannon = b.GetComponent<Cannon>();
        cannon.collisionTest = pv.isMine; // 나자신의 유무
        cannon.bullet = sbul; // 포탄의 종류
        cannon.owner = gameObject; // 포탄의 주인
        GameObject c = (GameObject)Instantiate(sbul.trail, firePos.position, firePos.rotation); // 포탄 꼬리 생성
        c.transform.parent = cannon.transform;  // 부모 포탄

        PlayAudioAtPoint(sbul.fireSound, transform.position); // 쏠때 사운드 설정
        rebound = 0;
        
        
        GameObject obj = (GameObject)Instantiate(MuzzleFlash, firePos.position, Quaternion.identity); // 포탄 입구에서 포탄 쏜후 나오는 불꽃파티클 생성
        Destroy(obj, 0.5f);
    }
    AudioSource PlayAudioAtPoint(AudioClip clip, Vector3 pos) // 사운드 생성 부분
    {
        GameObject tempGO = new GameObject(); 
        tempGO.name = "Sound Effect"; // 사운드 파일찾기
        tempGO.transform.position = pos;  // 사운드 파일의 위치 = 현재 오브젝트의 위치
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); 

        //↓↓↓ 사운드 설정
        aSource.rolloffMode = AudioRolloffMode.Custom;
        aSource.maxDistance = 350;
        aSource.clip = clip;                    
        aSource.Play();
        Destroy(tempGO, clip.length); 
        return aSource; 
    }
}
