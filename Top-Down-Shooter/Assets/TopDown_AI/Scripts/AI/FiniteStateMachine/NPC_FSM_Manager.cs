using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_FSM_Manager : MonoBehaviour
{
    public Text statesLoggerText;

    NPC_Enemy_FSM[] fsms;

    // Start is called before the first frame update
    void Start()
    {
        fsms = GetComponentsInChildren<NPC_Enemy_FSM>();
    }

    // Update is called once per frame
    void Update()
    {
        if(statesLoggerText)
        {
            statesLoggerText.text = "";
            foreach(NPC_Enemy_FSM fsm in fsms)
                if(fsm == null)
                    statesLoggerText.text += "Dead\n";
                else
                    statesLoggerText.text += fsm.GetStateText() + "\n";
        }
    }
}