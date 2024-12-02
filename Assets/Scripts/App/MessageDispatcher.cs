using fi;
using UnityEngine;
using System;

public class MessageDispatcher : fi.MessageDispatcher
{
    protected override void Update()
    {
        base.Update();
    }

    public override void onServerMessage(Message message)
    {
        Debug.Log(string.Format("{0} | Server | Received Message From Server: {1}", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), message.Info));
        App.LogMessage(string.Format("{0} | Server | Received Message From Server: {1}", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), message.Info));
        base.onServerMessage(message);
    }
} 
