using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Simple host/join UI for development and testing.
// Attach to a canvas GameObject and wire up the UI references.
public class NetworkHUD : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private GameObject connectedPanel;

    [Header("Connect Panel")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField addressInput;

    [Header("Connected Panel")]
    [SerializeField] private Button leaveButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
        leaveButton.onClick.AddListener(StopConnection);

        ShowConnectPanel();
    }

    private void StartHost()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
        ShowConnectedPanel("Hosting — waiting for players...");
    }

    private void StartClient()
    {
        string address = string.IsNullOrEmpty(addressInput.text)
            ? "localhost"
            : addressInput.text;

        InstanceFinder.ClientManager.StartConnection(address);
        ShowConnectedPanel("Connecting to " + address + "...");
    }

    private void StopConnection()
    {
        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.StopConnection(true);

        InstanceFinder.ClientManager.StopConnection();
        ShowConnectPanel();
    }

    private void ShowConnectPanel()
    {
        connectPanel.SetActive(true);
        connectedPanel.SetActive(false);
    }

    private void ShowConnectedPanel(string status)
    {
        connectPanel.SetActive(false);
        connectedPanel.SetActive(true);

        if (statusText != null)
            statusText.text = status;
    }
}
