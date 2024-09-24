using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyCoopServer;
using static Il2CppParadoxNotion.Services.Logger;
using UnityEngine;

namespace SkyCoop
{
    public class ClientSend
    {
        public static void SendToHost(NetDataWriter writer)
        {
            if (ModMain.Client != null && ModMain.Client.m_Instance != null)
            {
                ModMain.Client.SendToHost(writer);
            }
        }

        public static void Welcome()
        {
            //TODO: Send here MAC address and nick name.

            string Message = "I am connected!";
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.Welcome);
            writer.Put(Message.Length);
            writer.Put(Message);
            SendToHost(writer);
        }

        public static void SendPosition(Vector3 Position)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientPosition);
            writer.Put(Position.x);
            writer.Put(Position.y);
            writer.Put(Position.z);
            SendToHost(writer);
        }

        public static void SendRotation(Quaternion Rotation)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientRotation);
            writer.Put(Rotation.x);
            writer.Put(Rotation.y);
            writer.Put(Rotation.z);
            writer.Put(Rotation.w);
            SendToHost(writer);
        }

        public static void SendScene(string Scene)
        {
            NetDataWriter writer = new NetDataWriter();

            if(Scene == null)
            {
                Logger.Log(ConsoleColor.DarkMagenta,"SendScene is null WHAT THE FUCK? ");
            }

            Logger.Log("SendScene "+ Scene);

            writer.Put((int)Packet.Type.ClientScene);
            writer.Put(Scene.Length);
            writer.Put(Scene);
            SendToHost(writer);
        }
    }
}
