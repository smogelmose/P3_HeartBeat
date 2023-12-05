using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;

public class BPMReceiver : MonoBehaviour
{
    public int port = 12345;
    private TcpListener listener;
    private Thread listenerThread;
    private float latestBPM;

    void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        listenerThread = new Thread(new ThreadStart(ListenForData));
        listenerThread.Start();
    }

    void ListenForData()
    {
        while (true)
        {
            if (!listener.Pending())
            {
                Thread.Sleep(100);
                continue;
            }

            TcpClient client = listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);
            string receivedData = Encoding.ASCII.GetString(data, 0, bytesRead);
                        
            BPMData bpmData = JsonUtility.FromJson<BPMData>(receivedData);
            Debug.Log("Received BPM: " + bpmData.bpm);
                       
            latestBPM = bpmData.bpm;

            client.Close();
        }
    }
     
    public float GetLatestBPM()
    {
        return latestBPM;
    }
}

[System.Serializable]
public class BPMData
{
    public float bpm;
}
