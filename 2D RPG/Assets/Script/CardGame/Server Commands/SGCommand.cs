using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

public enum SGCommandID:ushort
{
    NULL,
    CAST_SPELL_FROM_0,
    PLAY_CARD_FROM_HAND,
    SEND_DECK,
    REFRESH_TIMEOUT
}

public abstract class SGCommand {

    public abstract void ExecuteCommand(CardGameManager cgm);

    // Packing/Unpacking functions implemented separately for each command
    public abstract BKSystem.IO.BitStream PackCommand();
    public abstract void UnpackCommand(BKSystem.IO.BitStream packet);

    protected SGCommandID m_ID = SGCommandID.NULL;

    protected SGCommand()
    {
    }

    public SGCommandID GetID() { return m_ID; }

    public static SGCommand CreateCommandFromPacket(BKSystem.IO.BitStream packet, int playerID)
    {
        if (packet.Length < 0)
        {
            return null;
        }

        ushort commandID;
        packet.Position = 0;
        packet.Read(out commandID, 0, 16);
        Debug.Log("Read packet with server command ID: " + commandID);

        SGCommand command;

        switch (commandID)
        {
            case 1:
                command = new SGC_CastSpellFromChannel0(packet, playerID);
                break;
            case 2:
                command = new SGC_PlayCardFromHand(packet, playerID);
                break;
            case 3:
                command = new SGC_SendDeck(packet, playerID);
                break;
            case 4:
                command = new SGC_RefreshTimeout(packet, playerID);
                break;
            default:
                command = null;
                break;
        }

        command.m_ID = (SGCommandID)commandID;

        return command;
    }
}
