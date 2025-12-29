using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class skill : MonoBehaviour
{
    public GameObject prefab_attack;
    public GameObject prefab_defense;
    public GameObject prefab_hp;
    public GameObject prefab_in;
    public GameObject prefab_on;

    GameObject cod;
    private List<GameObject> ui_objects = new List<GameObject>();

    skill_L skill_l;
    player player;

    public List<List<int>> skill_attack;
    public List<List<int>> skill_defense;
    public List<List<int>> skill_hp;
    public List<List<int>> skill_for;

    void Start()
    {
        skill_l = GetComponent<skill_L>();
        player = GetComponent<player>();
        skill_attack = skill_l.skill_list["attaku"];
        skill_defense = skill_l.skill_list["defense"];
        skill_hp = skill_l.skill_list["hp"];
        player.SKILL = 20;
        GenerateRow();
    }

    void GenerateRow()
    {
        //1体目の座標指定
        Vector2 position = Vector2.zero;

        //3体出現させる(3回繰り返す)
        for (int i = 0; i < skill_attack.Count; i++)
        {
            on_in_prefab(skill_attack[i], position);

            //Instantiateでプレハブを複製
            cod = Instantiate(prefab_attack, position, Quaternion.identity);
            //一つ一つ複製したプレハブに名づけ
            cod.name = "attaku_skill" + i.ToString();
            
            //UIの保存
            ui_objects.Add(cod);
            //次の複製体のために間隔をあける
            position.x += 1;
        }

        position = Vector2.zero;
        position.y -= 1;
        for (int i = 0; i < skill_defense.Count; i++)
        {
            on_in_prefab(skill_defense[i], position);

            cod = Instantiate(prefab_defense, position, Quaternion.identity);
            cod.name = "defense_skill" + i.ToString();
            ui_objects.Add(cod);
            position.x += 1;
        }

        position = Vector2.zero;
        position.y -= 2;
        for (int i = 0; i < skill_hp.Count; i++)
        {
            on_in_prefab(skill_hp[i], position);

            cod = Instantiate(prefab_hp, position, Quaternion.identity);
            cod.name = "hp_skill" + i.ToString();
            ui_objects.Add(cod);
            position.x += 1;
        }
    }

    void on_in_prefab(List<int> list, Vector2 position)
    {
        Vector3 backPosition = new Vector3(position.x, position.y, 0.1f);

        GameObject statusObj = null;
        if (list[1] == 1)
        {
            cod = Instantiate(prefab_in, backPosition, Quaternion.identity);
            cod.name = "in_attack";
        }
        else if (list[0] == 1)
        {
            cod = Instantiate(prefab_on, backPosition, Quaternion.identity);
            cod.name = "on_attack";
        }
        if (statusObj != null)
        {
            ui_objects.Add(statusObj);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                Debug.Log("クリックされた2Dオブジェクト: " + hitObject.name);

                //名前を判別して処理を分ける
                if (hitObject.name.Contains("attaku_skill"))
                {
                    skill_push("attaku");
                }
                else if (hitObject.name.Contains("defense_skill"))
                {
                    skill_push("defense");
                }
                else if (hitObject.name.Contains("hp_skill"))
                {
                    skill_push("hp");
                }
            }
        }
 
    }

    public void skill_push(string typ)
    {
        int cost = 0;
        int k = 0;
        switch (typ)
        {
            case "attaku":
                skill_for = skill_attack;
                break;
            case "defense":
                skill_for = skill_defense;
                break;
            case "hp":
                skill_for = skill_hp;
                break;
            default:
                break;
        }
        foreach (var item in skill_for)
        {
            k++;
            if (item[1] == 0)
            {
                cost = k;
                break;
            }
        }

        Debug.Log(cost);
        if (player.SKILL > cost)
        {
            int i = 0;
            foreach (var item in skill_for) 
            {
                if (item[1] == 0)
                {
                    skill_l.skill_list[typ][i][1] = 1;
                    if (i + 1< skill_l.skill_list[typ].Count)
                        skill_l.skill_list[typ][i+1][0] = 1;
                    switch (typ)
                    {
                        case "attaku":
                            player.ATK += 140;
                            break;
                        case "defense":
                            player.DEF += 140;
                            break;
                        case "hp":
                            player.HP += 1800;
                            break;
                        default:
                            break;
                    }
                    player.SKILL -= cost;
                    break;
                }
                i++;
            }

            foreach (GameObject obj in ui_objects) Destroy(obj);
            {
                ui_objects.Clear();
            }
            GenerateRow();
        }
    }
}
