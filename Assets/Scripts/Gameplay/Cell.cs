// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectHKU.Map;
using Chrysalis.Core;

namespace ProjectHKU.Gameplay
{
    public class Cell
    {

        public Vector2 pos;
        public GameObject obj;
        public float oxygen = 0;
        public float nutrition = 0;
        public bool alive = true;
        public Vector2 targetPos;
        float velocity = 0.1f;

        public Cell(Vector2 posOfCell)
        {
            pos = posOfCell;
            oxygen = 1;
            nutrition = 1;
            alive = true;
            targetPos = Vector2.zero;
            velocity = 0.05f;
        }

        public void LogicalUpdate()
        {
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            if (targetPos!= Vector2.zero)
            {
                var direction = (targetPos - pos);
                if (direction.magnitude <= velocity)
                {
                    pos = targetPos;
                    targetPos = Vector2.zero;
                }
                else
                {
                    direction.Normalize();
                    pos += direction * velocity;
                }
            }
            else
            {
                Vector2 buildingPos = MapManager.Instance.Pos2Building(pos);
                if (GameplayManager.Instance.buildingList.ContainsKey(buildingPos))
                {
                    var curBuilding = GameplayManager.Instance.buildingList[buildingPos];
                    if (curBuilding.type!= "vein")
                    {
                        alive = false;
                        return;
                    }
                    refreshResources();
                    Vector2 nextPos = MapManager.Instance.Angle2Direction(curBuilding.orientation) + buildingPos;
                    if (nextPos == MapManager.Instance.endVeinPos)
                    {
                        curBuilding.occupied = false;
                        targetPos = nextPos - new Vector2(0.5f, 0.5f);
                    }
                    else if (GameplayManager.Instance.buildingList.ContainsKey(nextPos) 
                        && !GameplayManager.Instance.buildingList[nextPos].occupied
                        && GameplayManager.Instance.buildingList[nextPos].type == "vein"
                    )
                    {
                        curBuilding.occupied = false;
                        GameplayManager.Instance.buildingList[nextPos].occupied = true;
                        targetPos = nextPos - new Vector2(0.5f, 0.5f);
                    }
                }
                else 
                {
                    alive = false;
                    return;
                }
            }
        }

        public void refreshResources()
        {
            Vector2 buildingPos = MapManager.Instance.Pos2Building(pos);
            Vector2[] possibleDir = new Vector2[]{
                new Vector2(0,1), new Vector2(0,-1),
                new Vector2(1,0), new Vector2(-1,0)
            };
            foreach (var a in possibleDir)
            {
                if (GameplayManager.Instance.buildingList.ContainsKey(buildingPos+a))
                {
                    var building = GameplayManager.Instance.buildingList[buildingPos+a];
                    if (building.type != "vein")
                    {
                        if (building.type=="lung")
                        {
                            oxygen = 4;
                        }
                        else
                        {
                            building.oxygen = (oxygen + building.oxygen) / 2;
                            oxygen = building.oxygen;
                        }
                        if (building.type=="intestine")
                        {
                            nutrition = 4;
                        }
                        else
                        {
                            building.nutrition = (nutrition + building.nutrition) / 2;
                            nutrition = building.nutrition;
                        }
                    }
                }
            }
        }
    }
}