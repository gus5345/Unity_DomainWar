using UnityEngine;
using System.Collections;

public class TankPosition : MonoBehaviour {

    private PhotonView pv; //RPC 호출을 위한 것 
    public Vector3 original;
    public bool completion = true;
    public int TankPositionChoice = 0;// 0 생성한다 , 1 리스폰한다 

    private Vector3 redPos;
    private Vector3 bluePos;
    public int pvID;
	
    void Awake () {
        pv = GetComponent<PhotonView>();
	}
	
	void Update () {
            if (!completion)
            {
                if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
                {
                    float zpos = Random.Range(50.0f, 150.0f);
                    float xpos = Random.Range(1000.0f, 1080.0f);
                    Vector3 redPos = new Vector3(xpos, 200, zpos);
                    original = redPos;
                }
                else
                {
                    float zpos = Random.Range(750.0f, 850.0f);
                    float xpos = Random.Range(100.0f, 180.0f);
                    Vector3 bluePos = new Vector3(xpos, 200, zpos);
                    original = bluePos;
                }

                Ray ray = new Ray();
                Vector3 rayPosition = new Vector3(original.x, original.y, original.z + 4.5f);
                ray.origin = rayPosition;
                ray.direction = Vector3.down;

                RaycastHit[] hits = Physics.SphereCastAll(ray, 12.0f, 250.0f); // 구형으로 체크한다.

                int nearestId = -1;
                for (int i = 0; i < hits.Length; i++)
                {

                    RaycastHit hit = hits[i];
                    // 맨 처음 hit 찾기
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
                    if (nearestId != -1)
                    {
                        if (hits[nearestId].transform.tag == "Map")
                        {
                            Vector3 UpTankPosition = new Vector3(hit.point.x, hit.point.y + 5.0f, hit.point.z);
                            switch (TankPositionChoice)
                            {
                                case 0:
                                    CreateTank(UpTankPosition);
                                    GameObject[] dieCollision = GameObject.FindGameObjectsWithTag("CollisionSmoke");
                                    for (int c = 0; c < dieCollision.Length; c++)
                                    {
                                        if(dieCollision[c].transform.parent == null)
                                        {
                                            Destroy(dieCollision[c]);
                                        }
                                    }
                                    break;
                                case 1:
                                    GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");
                                    for (int j = 0; j < tanks.Length; j++)
                                    {
                                        if (pvID == PhotonNetwork.player.ID)
                                        {
                                            tanks[j].GetComponent<TankMove>().callPosition(UpTankPosition);
                                        }
                                    }
                                    break;
                            }
                            completion = true;
                            pv.RPC("MinMapPlayer", PhotonTargets.All);
                        }
                    }
                }
            }
    }
    void CreateTank(Vector3 tankPos)
    {
        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
        {
            GameObject KoreaPlayer = PhotonNetwork.Instantiate("KoreaTank", tankPos, Quaternion.identity, 0);
        }
        else
        {
            Vector3 ro = new Vector3(0, 180, 0);
            GameObject ChinaPlayer = PhotonNetwork.Instantiate("KoreaTank", tankPos, Quaternion.Euler(ro), 0);
        }
    }
    [RPC] void MinMapPlayer()
    {
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].GetComponentInChildren<MinMapPoint>().RoomLobbyCall();
            //tanks[i].SendMessage("RoomLobbyCall");
        }
    }
}


