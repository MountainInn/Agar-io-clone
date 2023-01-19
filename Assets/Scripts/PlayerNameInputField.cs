using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    private const string playerNameKey = "PlayerName";

    private TMP_InputField inputField;

    private void Start()
    {
        this.inputField = GetComponent<TMP_InputField>();
       
        if (PlayerPrefs.HasKey(playerNameKey))
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString(playerNameKey);
        }

        inputField.onValueChanged.AddListener(SetPlayerName);
    }


    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player name is null or empty");
            return;
        }

        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNameKey, value);
    }
}
