using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientConnectionInterface : MonoBehaviour {

    private static ClientConnectionInterface instance;

    public CGVisualManager m_visualManager;

    /** Editor variables */
    public string m_ipAddress = "localhost";
    public int m_port = 8080;
    public bool m_offline = false;
    public string m_deckFileName = "FirstDeck.json";

    ClientConnectionManager m_client;
    LocalServerRunner m_localServerRunner;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        Application.runInBackground = true;
        DontDestroyOnLoad(this);
        if (m_offline)
        {
            PlayOffline();
        }
        if(m_visualManager)
        {
            m_visualManager.SetClientConnectionInterface(this);
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_client != null && m_client.IsConnected())
        {
            if (m_visualManager == null)
            {
                SetUpVisualManager();
            }

            CGCommand command = m_client.ReceiveCommand();
            if (command != null)
            {
                command.m_visualManager = m_visualManager;
                command.OnReceived();
                m_visualManager.AddCommand(command);
            }
        }
        // Run AI client if it exists
        if(m_offline && m_localServerRunner != null)
        {
            m_localServerRunner.AiUpdate();
        }
    }

    void SetUpVisualManager()
    {
        m_visualManager = FindObjectOfType<CGVisualManager>();
        if (m_visualManager != null)
        {
            m_visualManager.SetClientConnectionInterface(this);
        }
    }

    public void TransmitFromClient(BKSystem.IO.BitStream data)
    {
        m_client.TransmitStream(data);
    }

    public void SetNewIP(string ip)
    {
        m_ipAddress = ip;
    }

    public void SetNewPort(string port)
    {
        m_port = int.Parse(port);
    }

    public void SetDeckFileName(string fileName)
    {
        m_deckFileName = fileName;
    }

    public void ConnectButton()
    {
        m_client = new ClientConnectionManager();
        bool connected = m_client.AttemptConnection(m_ipAddress, m_port);
        if (connected)
        {
            // load new scene
            SceneManager.LoadScene("Scenes/CardGame");
        }
    }

    /** Start an offline server within the application and start a game  */
    public void PlayOffline()
    {
        m_offline = true;
        m_localServerRunner = new LocalServerRunner();
        m_client = new ClientConnectionManager();
        m_localServerRunner.StartServer(m_client);
        if (m_client.IsConnected() && m_visualManager == null)  // Check for visual manager to know if we are already in-game - Hacky!!
        {
            SceneManager.LoadScene("Scenes/CardGame");
        }
    }
}
