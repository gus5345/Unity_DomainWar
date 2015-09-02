using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 플레이어 아이디를 노출시켜주는 스크립트

public class DisplayUserId : MonoBehaviour {
	public Text userId;
	private PhotonView pv = null;


	// Use this for initialization
	void Start () {
		pv = GetComponent<PhotonView> ();
		userId.text = pv.owner.name;
	}
}
