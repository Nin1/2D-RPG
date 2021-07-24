using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;
using System;

// Empty packet to refresh the given player's timeout

public class SGC_RefreshTimeout : SGCommand
{
    public SGC_RefreshTimeout()
    {
    }

    public SGC_RefreshTimeout(BKSystem.IO.BitStream packet, int playerID)
    {
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)SGCommandID.REFRESH_TIMEOUT, 0, 16);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
    }

    public override void ExecuteCommand(CardGameManager cgm)
    {
    }
}
