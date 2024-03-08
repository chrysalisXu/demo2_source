// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectHKU.Map;
using Chrysalis.Core;

namespace ProjectHKU.Gameplay
{

    
    public class GameplayManager : MonoBehaviour
    {

        // singleton, only Instance is valid and will be updated, others are for ai.
        private static GameplayManager _instance = null;
        public static GameplayManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<GameplayManager>();
                if (_instance == null) throw new Exception("Gameplay manager not exists!");
                return _instance;
            }
        }

        [NonSerialized]
        string currentBuilding = null;
        [NonSerialized]
        Building lastbuilding = null;
        [NonSerialized]
        public Dictionary <Vector2, Building> buildingList = new Dictionary <Vector2, Building>();
        [NonSerialized]
        List <Cell> cellList = new List <Cell>();
        [NonSerialized]
        public bool frameMultitaskLock = false; // skip one frame for user operation

        public float spawnCount = 0;
        public float spawnRate = 1;

        public void LogicalUpdate()
        {
            foreach (Building building in buildingList.Values)
            {
                building.LogicalUpdate();
            }
            foreach (Cell cell in cellList)
            {
                cell.LogicalUpdate();
            }
            spawnWithCheck();
        }

        public void spawnWithCheck()
        {
            spawnCount -= spawnRate;
            if (buildingList.ContainsKey(MapManager.Instance.startVeinPos)
                && buildingList[MapManager.Instance.startVeinPos].type == "vein"
                && buildingList[MapManager.Instance.startVeinPos].occupied == false
                && spawnCount <= 0
            )
            {
                cellList.Add(new Cell(MapManager.Instance.startVeinPos - new Vector2(0.5f,1.5f)));
                cellList.Last().targetPos = MapManager.Instance.startVeinPos - new Vector2(0.5f,0.5f);
                buildingList[MapManager.Instance.startVeinPos].occupied = true;
                MapManager.Instance.RenderCell(cellList.Last());
                spawnCount = 100;
            }
        }

        public void DestructBuilding(Vector2 pos)
        {
            buildingList[pos].obj.SetActive(false);
            buildingList[pos].alive = false;
        }

        public void LateUpdate()
        {
            var itemsToRemove = buildingList.Where(f => f.Value.alive == false).ToArray();
            foreach (var item in itemsToRemove)
                buildingList.Remove(item.Key);
            var cellsToRemove = cellList.Where(f => f.alive == false).ToArray();
            foreach (var item in cellsToRemove)
            {
                item.obj.SetActive(false);
                cellList.Remove(item);
            }
        }

        public void PlayerUpdate()
        {
            if (Input.GetMouseButton(0) && currentBuilding!=null) // build!
            {
                Vector2 pos = MapManager.Instance.MousePos();

                if (pos != Vector2.zero)
                {
                    if (buildingList.ContainsKey(pos))
                    {
                        if (currentBuilding == "remove")
                            DestructBuilding(pos);
                        else
                            {
                                if (lastbuilding != null) 
                                {
                                    buildingList[pos].orientation = MapManager.Instance.Direction2Angle(pos - lastbuilding.pos);
                                    lastbuilding.orientation = buildingList[pos].orientation;
                                }
                                lastbuilding = buildingList[pos];
                            }
                    }
                    else
                    {
                        bool suc = false;
                        if (currentBuilding == "remove") {}
                        else if (MapManager.Instance.CannotBuild(pos)) {}
                        else if (currentBuilding == "heart")
                        {
                            if (MapManager.Instance.NextToHeart(pos))
                                suc = true;
                            else
                                UIManager.Instance.Warning("cardiacmuscle must be placed near heart!");
                        }
                        else if (currentBuilding == "brain") {suc = true;}
                        else if (currentBuilding == "lung") {suc = true;}
                        else if (currentBuilding == "intestine") {suc = true;}
                        else if (currentBuilding == "vein") {suc = true;}

                        if (suc)
                        {
                            buildingList.Add(pos, new Building(currentBuilding, pos));
                            if (lastbuilding != null) 
                            {
                                buildingList[pos].orientation = MapManager.Instance.Direction2Angle(pos - lastbuilding.pos);
                                lastbuilding.orientation = buildingList[pos].orientation;
                            }
                            MapManager.Instance.RenderBuilding(buildingList[pos]);
                            lastbuilding = buildingList[pos];
                        }
                        else
                        {
                            if (lastbuilding != null) 
                            {
                                lastbuilding.orientation = MapManager.Instance.Direction2Angle(pos - lastbuilding.pos);
                            }
                        }
                    }
                }

            }
            if (Input.GetMouseButtonUp(0)) currentBuilding = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (this != Instance) return;
            if (frameMultitaskLock) frameMultitaskLock = !frameMultitaskLock;
            else PlayerUpdate();
            LogicalUpdate();
        }

        public void SelectBuilding(string target)
        {
            currentBuilding = target;
            frameMultitaskLock = true;
        }

        private void Start()
        {

        }
    }
}