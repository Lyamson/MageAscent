using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySkillsController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        foreach (Skill skill in gameObject.GetComponents<Skill>())
        {
            StartCoroutine(TryToCastSkill(skill));
        }
    }

    private IEnumerator TryToCastSkill(Skill skill)
    {
        MyDebug.Log(this, "EnemySkillsController.TryToCastSkill Callback");
        yield return new WaitUntil(() => { return skill.IsReadyToAutoCast(); });

        //print("Use");

        skill?.AutoUse();

        //yield return new WaitForSeconds(2);
        StartCoroutine(TryToCastSkill(skill));
    }
}