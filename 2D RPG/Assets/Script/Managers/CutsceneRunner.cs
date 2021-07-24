using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CutsceneRunner : MonoBehaviour {

    public static CutsceneRunner instance { get; private set; }

    [SerializeField]
    DialogueBox m_dialogueBox;
    /** Special int to store the value of a dialogue choice */
    int m_dialogueChoice = -1;

    public delegate IEnumerator Cutscene();
    public delegate void OnCutsceneStart();
    public delegate void OnCutsceneEnd();
    public event OnCutsceneStart onCutsceneStart;
    public event OnCutsceneEnd onCutsceneEnd;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartCutscene(Cutscene cutscene)
    {
        StartCoroutine(CutsceneCoroutine(cutscene));
    }

    IEnumerator CutsceneCoroutine(Cutscene cutscene)
    {
        onCutsceneStart();
        Debug.Log("Cutscene start!");
        yield return StartCoroutine(cutscene());
        Debug.Log("Cutscene end!");
        onCutsceneEnd();
    }

    /** SCRIPT FUNCTIONS */

    void SetInt(string varName, int value)
    {
        VarManager.SetVar(varName, value);
    }

    int GetInt(string varName)
    {
        return VarManager.GetVar(varName);
    }

    /** Pop up a dialogue box and wait for the player to advance it */
    IEnumerator Dialogue(string dialogue)
    {
        // Open the dialogue box and set it up
        m_dialogueBox.gameObject.SetActive(true);
        m_dialogueBox.StartDialogue(dialogue);
        // Wait for the player to close the dialogue
        while (!m_dialogueBox.IsComplete())
        {
            yield return null;
        }
    }

    /** Pop up a dialogue box and wait for the player to advance it */
    IEnumerator Dialogue(string dialogue, string name)
    {
        // Open the dialogue box and set it up
        m_dialogueBox.gameObject.SetActive(true);
        m_dialogueBox.StartDialogue(dialogue, name);
        // Wait for the player to close the dialogue
        while (!m_dialogueBox.IsComplete())
        {
            yield return null;
        }
    }

    IEnumerator CloseDialogue()
    {
        // Set the dialogue box inactive
        m_dialogueBox.gameObject.SetActive(false);
        yield return null;  // TODO: Animate this away
    }

    /** Pop up a choice and wait for the player to respond, then store the choice */
    IEnumerator GiveChoice(string dialogue, string name, string[] choices)
    {
        m_dialogueBox.gameObject.SetActive(true);
        m_dialogueBox.StartChoice(dialogue, choices, name);
        while (!m_dialogueBox.IsComplete())
        {
            yield return null;
        }
        m_dialogueChoice = m_dialogueBox.GetChoiceIndex();
    }

    int GetChoice()
    {
        return m_dialogueChoice;
    }

    /** SCRIPTS */

    public IEnumerator FrogInteract()
    {
        if(GetInt("frogTest") >= 10)
        {
            yield return StartCoroutine(FrogInteractBranch1());
        }
        else
        {
            yield return StartCoroutine(FrogInteractBranch2());
        }
    }

    public IEnumerator FrogInteractBranch1()
    {
        yield return StartCoroutine(Dialogue("Hello. You have found my secret. Congratulations.", "Frog"));
        yield return StartCoroutine(Dialogue("Here is your prize.", "Frog"));
        yield return StartCoroutine(CloseDialogue());
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(Dialogue("..."));
        yield return StartCoroutine(Dialogue("........"));
        yield return StartCoroutine(GiveChoice("..............", null, new[] { "...?", "Where is the prize?" }));
        int choice = GetChoice();
        if(choice == 0)
        {
            yield return StartCoroutine(Dialogue("...Ribbit.", "Frog"));
            SetInt("frogTest", 0);
        }
        else
        {
            yield return StartCoroutine(Dialogue("I do not have it yet.", "Frog"));
            SetInt("frogWin", 1);
        }
        yield return StartCoroutine(CloseDialogue());
    }

    public IEnumerator FrogInteractBranch2()
    {
        yield return StartCoroutine(Dialogue("Ribbit!", "Frog"));
        yield return StartCoroutine(GiveChoice("Ribbit ribbit?", "Frog", new[] { "Ribbit", "Okay?" }));
        int choice = GetChoice();
        if (choice == 0)
        {
            yield return StartCoroutine(Dialogue("Ribbit!"));
            SetInt("frogTest", GetInt("frogTest") + 1);
        }
        else
        {
            yield return StartCoroutine(Dialogue("Croak...", "Frog"));
        }
        yield return StartCoroutine(CloseDialogue());
    }
}
