using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSummoner : MonoBehaviour
{
    public void SummonSkill(GameObject skill, Vector3 pos)
    {
        Instantiate(skill, pos, Quaternion.identity);

    }
}
