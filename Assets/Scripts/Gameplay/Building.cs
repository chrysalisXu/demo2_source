// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectHKU.Map;
using Chrysalis.Core;

namespace ProjectHKU.Gameplay
{
    public class Building
    {
        static string [] buildingType = new string[] {
            "heart",
            "brain",
            "lung",
            "intestine",
            "vein"
        };

        public string type;
        public Vector2 pos;
        public GameObject obj;
        public float orientation = 0;

        public float oxygen = 1;
        public float nutrition = 1;
        public bool alive = true;
        public bool occupied = false;

        public Building(string typeOfBuilding, Vector2 posOfBuilding)
        {
            type = typeOfBuilding;
            pos = posOfBuilding;
            oxygen = 1;
            nutrition = 1;
            alive = true;
            occupied = false;
        }

        public void LogicalUpdate()
        {
            if (type!="vein")
            {
                if (type!="lung")
                {
                    oxygen -= 0.004f;
                    if (oxygen < 0)
                    {
                        GameplayManager.Instance.DestructBuilding(pos);
                        UIManager.Instance.Warning($"{type} dead of lack oxygen!");
                    }
                }
                if (type!="intestine")
                {
                    nutrition -= 0.001f;
                    if (nutrition < 0)
                    {
                        GameplayManager.Instance.DestructBuilding(pos);
                        UIManager.Instance.Warning($"{type} dead of lack nutrition!");
                    }
                }
            }
            if (type == "vein")
                obj.transform.rotation = Quaternion.Euler(0, 0, orientation);
            else
                obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (float)Math.Min(oxygen, nutrition));
        }
    }
}