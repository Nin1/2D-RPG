using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
using UnityEngine.UI;
#endif

public class CGC_PhaseTransition : CGCommand
{
    string m_text;

    public CGC_PhaseTransition(string mes)
    {
        m_text = mes;
    }

    public CGC_PhaseTransition(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }


    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.PHASE_TRANSITION, 0, 16);
        packet.Write(m_text.Length, 0, 8);
        packet.Write(m_text.ToCharArray(), 0, m_text.Length);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        int textLength;
        packet.Read(out textLength, 0, 8);
        char[] text = new char[textLength];
        packet.Read(text, 0, textLength);
        m_text = new string(text);
    }
    
    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        // Nothing to do for AI
    }

#if CLIENT
    public override void ExecuteCommand()
    {
        PhaseTransitionSetup();
    }

    void PhaseTransitionSetup()
    {
        Text text = m_visualManager.GetPhaseTransitionText();
        text.text = m_text;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(text.transform.DOMoveX(3000.0f, 0));
        sequence.AppendCallback(PhaseTransition1);
    }

    void PhaseTransition1()
    {
        Image darkness = m_visualManager.GetPhaseTransitionBackground();
        Text text = m_visualManager.GetPhaseTransitionText();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(text.transform.DOMoveX(0.0f, 0.8f))
            .SetEase(Ease.OutSine);
        sequence.Insert(0, darkness.DOColor(new Color(0.05f, 0.05f, 0.05f, 0.8f), 0.5f));
        sequence.AppendCallback(PhaseTransition2);
    }

    void PhaseTransition2()
    {
        Image darkness = m_visualManager.GetPhaseTransitionBackground();
        Text text = m_visualManager.GetPhaseTransitionText();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(text.transform.DOMoveX(-3000.0f, 0.8f))
            .SetEase(Ease.InSine);
        sequence.Insert(0, darkness.DOColor(new Color(0.05f, 0.05f, 0.05f, 0.0f), 0.5f));
        sequence.AppendCallback(FinishCommand);
    }
#endif
}
