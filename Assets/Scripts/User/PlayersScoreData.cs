using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayersScoreData : MonoBehaviour{

    public string playerID;
    public Text playersid;
    public Text myID;
    public Text kills;
    public Text dies;
    public int kill;
    public int die;
    public int userIntID;

    public void DispPlayerDate()
    {
        if (PhotonNetwork.player.ID != userIntID)
        {
            playersid.text = playerID;
        }
        else
        {
            myID.color = Color.green;
            myID.text = PhotonNetwork.player.name;
        }
        kills.text = kill.ToString();
        dies.text = die.ToString();
    }
}
