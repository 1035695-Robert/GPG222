using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostLobbySettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInput;

    [SerializeField] private Toggle isPrivate;
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Image invalidPasswordImage;

    [SerializeField] private Button[] maxPlayers;

    private void OnEnable()
    {
        //update Lobby Name
        lobbyNameInput.onEndEdit.AddListener(UpdateLobbyName);

        isPrivate.onValueChanged.AddListener(ChangeLobbyState);

        foreach (Button count in maxPlayers)
        {
            count.onClick.AddListener(() => ChangeMaxPlayerState(count.name));
        }
    }


    private void UpdateLobbyName(string text)
    {
        HostManager.Instance.lobbyName = text;
    }

    private void ChangeLobbyState(bool state)
    {
        HostManager.Instance.privateState = state;
        HostManager.Instance.joinCodeText.gameObject.SetActive(state);
    }


    private void ChangeMaxPlayerState(string count)
    {
        if (int.TryParse(count, out int result))
        {
            HostManager.Instance.maxConnections = result;
        }
    }
}