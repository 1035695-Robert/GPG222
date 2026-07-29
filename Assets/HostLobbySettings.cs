using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostLobbySettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInput;

    [SerializeField] private Button[] lobbyState;
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Image invalidPasswordImage;

    [SerializeField] private Button[] maxPlayers;

    private void OnEnable()
    {
        //update Lobby Name
        lobbyNameInput.onEndEdit.AddListener(UpdateLobbyName);

        foreach (Button state in lobbyState)
        {
            state.onClick.AddListener(() => ChangeLobbyState(state.name));
        }

        passwordInput.onEndEdit.AddListener(CheckAndUpdatePassword);

        foreach (Button count in maxPlayers)
        {
            count.onClick.AddListener(() => ChangeMaxPlayerState(count.name));
        }
    }

    private void CheckAndUpdatePassword(string password)
    {
        if (HostManager.Instance.privateState)
        {
            if (password.Length < 8 || password.Length > 20)
            {
                passwordInput.text = "";
                invalidPasswordImage.color = Color.red;
            }
            else
            {
                HostManager.Instance.passwordText = password;
                invalidPasswordImage.color = Color.green;
                startButton.interactable = true;
            }
        }
    }

    private void UpdateLobbyName(string text)
    {
        HostManager.Instance.lobbyName = text;
    }

    private void ChangeLobbyState(string stateName)
    {
        Debug.Log(stateName);
        switch (stateName)
        {
            case "public":
                HostManager.Instance.privateState = false;
                startButton.interactable = true;
                invalidPasswordImage.gameObject.SetActive(false);

                return;
            case "private":
                HostManager.Instance.privateState = true;
                startButton.interactable = false;
                invalidPasswordImage.gameObject.SetActive(true);
                return;
        }
    }


    private void ChangeMaxPlayerState(string count)
    {
        if (int.TryParse(count, out int result))
        {
            HostManager.Instance.maxConnections = result;
        }
    }
}