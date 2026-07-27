using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinedPlayer : NetworkBehaviour
{
    [SerializeField] private Button joinButton;
    [SerializeField] private Button[] bodyColourButtons;
    [SerializeField]private Button[] handColourButtons;
    private NetworkSceneLoader _sceneLoader;
    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        joinButton.interactable = true;
        _sceneLoader = GameObject.Find("MenuManager").GetComponent<NetworkSceneLoader>();
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(Clicked);
        }
        foreach (var colourButton in bodyColourButtons)
        {
            colourButton.onClick.AddListener(() =>PlayerColourManager.Instance.BodyColour(colourButton.gameObject.name, OwnerClientId,"body"));
        }
        foreach (var colourButton in handColourButtons)
        {
            colourButton.onClick.AddListener(() =>PlayerColourManager.Instance.BodyColour(colourButton.gameObject.name, OwnerClientId,"hand"));
        }
    }
    

    
    
    void Clicked()
    {
        joinButton.interactable = false;
        _sceneLoader.OnLevelSelection("GameHub");
    }
}
