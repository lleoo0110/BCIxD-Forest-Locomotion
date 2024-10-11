using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UDPReceiverInt : MonoBehaviour
{
    UdpClient udpClient;
    public int receivedInt;
    int listenPort = 12345;

    [Header("イベント登録")]
    [SerializeField] private UnityEvent<int> _callEvent;

    private Queue<int> receivedDataQueue = new Queue<int>();
    private bool isRunning = true;

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        BeginReceive();
    }

    void BeginReceive()
    {
        if (!isRunning)
            return;

        try
        {
            udpClient.BeginReceive(new AsyncCallback(OnReceived), null);
        }
        catch (ObjectDisposedException)
        {
            Debug.LogWarning("UdpClient has been disposed, cannot begin receive.");
        }
        catch (Exception e)
        {
            Debug.LogError("BeginReceive exception: " + e.Message);
        }
    }

    void OnReceived(IAsyncResult result)
    {
        if (!isRunning)
            return;

        try
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEP);
            // バイト配列を整数に変換
            if (receivedBytes.Length == 4)
            {
                receivedInt = BitConverter.ToInt32(receivedBytes, 0);
                lock (receivedDataQueue)
                {
                    receivedDataQueue.Enqueue(receivedInt);
                }
            }

            // 再度受信を開始
            BeginReceive();
        }
        catch (ObjectDisposedException)
        {
            Debug.LogWarning("UdpClient has been disposed, cannot end receive.");
        }
        catch (Exception e)
        {
            Debug.LogError("OnReceived exception: " + e.Message);
            // 再度受信を開始
            BeginReceive();
        }
    }

    void Update()
    {
        lock (receivedDataQueue)
        {
            while (receivedDataQueue.Count > 0)
            {
                int data = receivedDataQueue.Dequeue();
                Debug.Log("Received int: " + data);
                _callEvent.Invoke(data);
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        udpClient.Close();
    }
}