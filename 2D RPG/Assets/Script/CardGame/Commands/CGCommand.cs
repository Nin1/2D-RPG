using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public enum CGCommandID:ushort
{
    NULL,
    CAST_SPELL,
    CHANNEL_SPELL,
    MOVE_CARD_TO_GRAVEYARD,
    OPPONENT_DRAW_CARD,
    OPPONENT_PLAY_CARD_FROM_HAND,
    PLAYER_DRAW_CARD,
    PLAYER_PLAY_CARD_FROM_HAND,
    SET_LIFE,
    WAIT_FOR_CAST_SELECTION,
    WAIT_FOR_PLAY_FROM_HAND,
    SET_PLAYER_ID,
    PHASE_TRANSITION,
    REQUEST_DECK,
    REFRESH_TIMEOUT
};

public abstract class CGCommand {

#if CLIENT

    public CGVisualManager m_visualManager;
    CGCommandRunner m_commandRunner;

    /** Happens immediately on receiving the command */
    public virtual void OnReceived() { }
    public virtual void OnReceivedAi(AiPlayer aiPlayer, ClientConnectionManager aiConnection) { }

    /** Happens once all previous commands have been executed */
    public abstract void ExecuteCommand();
    public abstract void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection);

    /** Call once all animations are finished */
    protected void FinishCommand()
    {
        m_commandRunner.CommandFinished();
    }

    public void SetCommandRunner(CGCommandRunner runner)
    {
        m_commandRunner = runner;
    }

#endif

    // Packing/Unpacking functions implemented separately for each command
    public abstract BKSystem.IO.BitStream PackCommand();
    public abstract void UnpackCommand(BKSystem.IO.BitStream packet);

    public static CGCommand CreateCommandFromPacket(BKSystem.IO.BitStream packet)
    {
        if (packet.Length < 0)
        {
            return null;
        }

        ushort commandID;
        packet.Position = 0;
        packet.Read(out commandID, 0, 16);
        Debug.Log("Read packet with command ID: " + commandID);

        CGCommand command;

        switch(commandID)
        {
            case (ushort)CGCommandID.CAST_SPELL:
                command = new CGC_CastSpell(packet);
                break;
            case (ushort)CGCommandID.CHANNEL_SPELL:
                command = new CGC_ChannelSpell(packet);
                break;
            case (ushort)CGCommandID.MOVE_CARD_TO_GRAVEYARD:
                command = new CGC_MoveCardToGraveyard(packet);
                break;
            case (ushort)CGCommandID.OPPONENT_DRAW_CARD:
                command = new CGC_OpponentDrawCard(packet);
                break;
            case (ushort)CGCommandID.OPPONENT_PLAY_CARD_FROM_HAND:
                command = new CGC_OpponentPlayCardFromHand(packet);
                break;
            case (ushort)CGCommandID.PLAYER_DRAW_CARD:
                command = new CGC_PlayerDrawCard(packet);
                break;
            case (ushort)CGCommandID.PLAYER_PLAY_CARD_FROM_HAND:
                command = new CGC_PlayerPlayCardFromHand(packet);
                break;
            case (ushort)CGCommandID.SET_LIFE:
                command = new CGC_SetLife(packet);
                break;
            case (ushort)CGCommandID.WAIT_FOR_CAST_SELECTION:
                command = new CGC_WaitForCastSelection(packet);
                break;
            case (ushort)CGCommandID.WAIT_FOR_PLAY_FROM_HAND:
                command = new CGC_WaitForPlayFromHand(packet);
                break;
            case (ushort)CGCommandID.SET_PLAYER_ID:
                command = new CGC_SetPlayerID(packet);
                break;
            case (ushort)CGCommandID.PHASE_TRANSITION:
                command = new CGC_PhaseTransition(packet);
                break;
            case (ushort)CGCommandID.REQUEST_DECK:
                command = new CGC_RequestDeck(packet);
                break;
            case (ushort)CGCommandID.REFRESH_TIMEOUT:
                command = new CGC_RefreshTimeout(packet);
                break;
            default:
                command = null;
                break;
        }

        return command;
    }
}
