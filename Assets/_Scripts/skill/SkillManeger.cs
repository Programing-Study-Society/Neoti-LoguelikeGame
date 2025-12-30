using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManeger : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Button> AttackSkillButton = new List<Button>();
    public List<Button> DefenseSkillButton = new List<Button>();
    public List<Button> hpSkillButton = new List<Button>();

    public int AttackSkillPoint = 0;
    public int DefenseSkillPoint = 0;
    public int HpSkillPoint = 0;

    public player player;

    
    void OnEnable()
    {
        player = FindObjectOfType<player>();
        if (player == null)
        {
            Debug.LogError("Player がシーン内に存在しません");
        }
        ButtonActiveUpdate();
        //それぞれのボタンを押したときの処理を付与
        for(int i = 0; i < AttackSkillButton.Count; i++)
        {
            AttackSkillButton[i].onClick.AddListener(() => OnClickAttackSkillButton());
        }
        for (int i = 0; i < DefenseSkillButton.Count; i++)
        {
            DefenseSkillButton[i].onClick.AddListener(() => OnClickDefenseSkillButton());
        }
        for (int i = 0; i < hpSkillButton.Count; i++)
        {
            hpSkillButton[i].onClick.AddListener(() => OnClickHpSkillButton());
        }

        StatasUp();

    }

    public void OnClickAttackSkillButton()
    {
        if (player.SKILL <= 0) return;
        AttackSkillPoint++;
        player.SKILL -= AttackSkillPoint;
        ButtonActiveUpdate();
        StatasUp();
    }
    public void OnClickDefenseSkillButton()
    {
        if (player.SKILL <= 0) return;
        DefenseSkillPoint++;
        player.SKILL -= DefenseSkillPoint;
        ButtonActiveUpdate();
        StatasUp();
    }
    public void OnClickHpSkillButton()
    {
        if (player.SKILL <= 0) return;
        HpSkillPoint++;
        player.SKILL -= HpSkillPoint;
        ButtonActiveUpdate();
        StatasUp();
    }

    public void StatasUp()
    {
        player.ATK = 100;
        player.DEF = 100;
        player.HP = 2000;
        for(int i = 0; i < AttackSkillPoint; i++)
        {
            player.ATK += 140;
        }
        for (int i = 0; i < DefenseSkillPoint; i++)
        {
            player.DEF += 140;
        }
        for (int i = 0; i < HpSkillPoint; i++)
        {
            player.HP += 1800;
        }
        player.MAXHP = player.HP;
    }

    public void ButtonActiveUpdate()
    {
        for(int i = 0; i < AttackSkillButton.Count; i++)
        {
            if(i == AttackSkillPoint)
            {
                AttackSkillButton[i].interactable = true;
            }
            else
            {
                AttackSkillButton[i].interactable = false;
            }
        }

        for (int i = 0; i < DefenseSkillButton.Count; i++)
        {
            if (i == DefenseSkillPoint)
            {
                DefenseSkillButton[i].interactable = true;
            }
            else
            {
                DefenseSkillButton[i].interactable = false;
            }
        }

        for (int i = 0; i < hpSkillButton.Count; i++)
        {
            if (i == HpSkillPoint)
            {
                hpSkillButton[i].interactable = true;
            }
            else
            {
                hpSkillButton[i].interactable = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
