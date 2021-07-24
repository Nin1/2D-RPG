using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarManager : MonoBehaviour {
    
    private static Dictionary<string, int> m_ints = new Dictionary<string, int>();

    /** Creates a new var at the given value if it does not already exist */
    public static void CreateVar(string varName, int initialValue)
    {
        if (!m_ints.ContainsKey(varName))
        {
            m_ints[varName] = initialValue;
        }
    }

    public static void SetVar(string varName, int value)
    {
        m_ints[varName] = value;
    }

    public static int GetVar(string varName)
    {
        if (m_ints.ContainsKey(varName))
        {
            return m_ints[varName];
        }
        Debug.LogWarning("Var accessed before creation.");
        return -1;
    }
}
