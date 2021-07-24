using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour, IInteractible {
    
    public void OnInteract()
    {
        VarManager.CreateVar("frogTest", 0);
        CutsceneRunner.instance.StartCutscene(CutsceneRunner.instance.FrogInteract);
    }
}
