using UnityEngine;
using System.Collections;

//싱글톤 
//포탄의 종류를 조절할 수 있는 스크립트, 클래스로 이루어져있다

public class BulletManager : MonoBehaviour
{
    [System.Serializable] // 클래스를 외부에 노출시키기 위해 필요
    public class Bullet
    {
        public int id; // 몇번 포탄인지
        public MeshRenderer skillDelay;
        public MeshRenderer skillBorder;
        public float effectTimer;
        [HideInInspector]
        public float nextFire = 0.0f; // 공격 후 딜레이
        public float fireRate; // 스킬 쿨타임 설정 4.5, 30
        public AudioClip fireSound; // 피격 후 사운드
        public GameObject effect;
        public int damage; // 데미지
        public int scope; //범위
        public int speed; //속도
        public GameObject trail; // 발사후 꼬리
    }

    public Bullet[] bullet;
    [HideInInspector]
    public Bullet selected;
    public static BulletManager instance; // 다른 곳에서 참조하지 못하도록 static 변수로 선언

    void Awake()
    {
        instance = this;
        bullet[0].skillBorder.enabled = true;
        bullet[1].skillBorder.enabled = false;
        selected = bullet[0];
    }
    void Update()
    {
        for (int i = 0; i < bullet.Length; i++)
        {
            float skillTime = ((BulletManager.Bullet)bullet.GetValue(i)).nextFire - Time.time;
            ((BulletManager.Bullet)bullet.GetValue(i)).skillDelay.material.SetFloat("_FillRate", (skillTime / ((BulletManager.Bullet)bullet.GetValue(i)).fireRate * Mathf.PI) * 2 - Mathf.PI);
        }
    }
    public void selectBullet(int n)
    {
        for(int i = 0; i < bullet.Length; i++)
        {
            ((Bullet)bullet.GetValue(i)).skillBorder.enabled = false;
        }
        selected = bullet[n];
        selected.skillBorder.enabled = true;
    }
}
