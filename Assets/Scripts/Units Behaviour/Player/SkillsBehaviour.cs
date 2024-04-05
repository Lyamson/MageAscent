using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillsBehaviour : NetworkBehaviour
{
    [SerializeField] SkillsManager skillsManager;
    [SerializeField] Player player;

    private void Update()
    {
        if (!IsOwner) return;
        //if (Input.GetKeyDown(KeyCode.Alpha1))

        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown((KeyCode)(49 + i)))
            {
                skillsManager.ActivateSpell((KeyCode)(49 + i));
            }
        }
        if (Input.GetMouseButtonDown(1) && skillsManager.activeSpell != null)
        {
            MyDebug.Log(this, "mouse pressed");
            skillsManager.activeSpell.Use();
        }
    }
}
