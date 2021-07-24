using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CGLifeVisual : MonoBehaviour {

    [SerializeField]
    Text m_lifeText;

    int m_life = 20;

    public void SetLife(int life)
    {
        m_life = life;
        m_lifeText.text = m_life.ToString();
        PulseLife();

        // @TODO: Different effects for whether life increased or decreased (e.g. green glow or red glow, pulse or shake)
    }

    void PulseLife()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.2f, 0.3f));
        seq.Append(transform.DOScale(1.0f, 0.3f));
    }
}
