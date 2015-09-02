using UnityEngine;
using System.Collections;

//날아가는 포탄 스크립트

public class Cannon : MonoBehaviour
{
    private Vector3 previousPos; //Connon의 transform
    [HideInInspector] // 밖으로 노출을 안시키기위해서 사용
    public BulletManager.Bullet bullet; // 총알이 어떤 폭약을 가지고 있는지
    [HideInInspector]
    public bool collisionTest; // 자기 자신인지 테스트하기 위한 변수
	public GameObject owner; // 포탄의 주인
    int ownerID; // 부모 오브젝트로 부토 아이디를 받기 위한 변수

    void Start()
    {
        previousPos = transform.position;
        rigidbody.AddForce(transform.forward * bullet.speed);  //물리적 힘을 가한다. 앞으로 * speed 만큼
        Destroy(this.gameObject,5.0f); // 내 자신의 오브젝트를 삭제함 5초 후 (포탄)
        ownerID = owner.GetComponentInParent<PhotonView>().ownerId; // 부모의 아이디
    }
    void FixedUpdate()
    {
        //↓↓↓↓↓ 줌 카메라가 탱크의 포신 안쪽으로 삽입되어있기 때문에 자기 포탄에 자기가 맞는걸 방지하는 부분
        Ray ray = new Ray();
        Vector3 pos = transform.position;
        Vector3 dir = pos - previousPos;
        ray.origin = previousPos;
        ray.direction = dir;
        
        RaycastHit[] hits = Physics.SphereCastAll(ray, 0.3f, dir.magnitude);
        
        int nearestId = -1;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == owner.GetComponent<FireCannon>().turretCollider) continue;
            if (nearestId == -1)
            {
                nearestId = i;
            }
            else
            {
                float dist = hit.distance;
                if (dist >= hits[nearestId].distance) continue;
                nearestId = i;
            }
        }
        if (nearestId != -1)
        {
            ExplosionCannon(hits[nearestId].point + hits[nearestId].normal * 0.5f);
        }

        previousPos = transform.position;
    }
 
    void ExplosionCannon(Vector3 position)
    {
        if(collisionTest)
        {
            Collider[] colliders = Physics.OverlapSphere(position,bullet.scope); // 위치에서 반지름 8만큼 탐색 충돌 배열에 넣는다.

           for(int i = 0; i < colliders.Length; i++)
            {
                TankDamage td = ((Collider)colliders.GetValue(i)).GetComponentInParent<TankDamage>(); //부모에서 찾는다
                if (td == null) continue; // 값이 없다면 다시 실행
                td.DoDamage(bullet.damage, position, owner.transform.position, ownerID); // 데미지,위치,주인의 위치,주인아이디 전달
            }
            for(int j = 0; j < colliders.Length; j++) // 한번에 중복 데미지를 주는 것을 방지하는 곳
            {
                TankDamage td = ((Collider)colliders.GetValue(j)).GetComponentInParent<TankDamage>(); //부모에서 찾는다
                if (td == null) continue; // 값이 없다면 다시 실행
                td.ClearDamaged();
            }
        }
        GameObject obj = (GameObject)Instantiate(bullet.effect, position, Quaternion.identity); //피격 파티클 생성
        Transform particle = transform.GetChild(0); // 첫번째 자식, 포탄쏘면 나오는 꼬리 파티클
        particle.parent = null; // 부모는 없다
        particle.particleSystem.enableEmission = false; // 파티클 끄는부분

        Destroy(particle.gameObject, 2.0f);
        Destroy(obj, bullet.effectTimer);
        Destroy(this.gameObject);
    }
}

