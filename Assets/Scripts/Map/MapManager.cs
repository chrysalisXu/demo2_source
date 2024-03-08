// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectHKU.Gameplay;

namespace ProjectHKU.Map
{
    [Serializable]
    public struct BuildingImage
    {
        public string name;
        public Sprite image;
    }

    public class MapManager : MonoBehaviour
    {

        // singleton, for other system to call
        private static MapManager _instance = null;
        public static MapManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<MapManager>();
                if (_instance == null) throw new Exception("Map not exists!");
                return _instance;
            }
        }

        [NonSerialized]
        public bool initiated = false;

        public GameObject linePrefab;
        public GameObject buildingPrefab;
        public GameObject veinPrefab;
        public GameObject cellPrefab;
        public int width, height;
        [SerializeField]
        public BuildingImage[] images;

        [NonSerialized]
        public Vector2 startVeinPos = new Vector2 (7,10);
        [NonSerialized]
        public Vector2 endVeinPos = new Vector2 (6,9);

        public Vector2 MousePos()
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = (int)MathF.Floor(pos.x);
            int y = (int)MathF.Floor(pos.y);
            if (x<0 || y<0 || x>= width || y>= height) return Vector2.zero;
            return new Vector2(x+1,y+1);
        }

        public Vector2 Pos2Building(Vector2 pos)
        {
            int x = (int)MathF.Floor(pos.x);
            int y = (int)MathF.Floor(pos.y);
            return new Vector2(x+1,y+1);
        }

        public bool NextToHeart(Vector2 pos)
        {
            if (pos.x == 5 || pos.x == 8)
                return pos.y < 10 && pos.y > 7;
            if (pos.y == 7 || pos.y == 10)
                return pos.x < 8 && pos.x > 5;
            return false;
        }

        public bool CannotBuild(Vector2 pos)
        {
            if (pos.x < 8 && pos.x > 5 && pos.y > 7 && pos.y < 10) return true;
            return false;
        }

        public float Direction2Angle(Vector2 pos)
        {
            if (pos.x < 0)
                return 90;
            if (pos.y < 0)
                return 180;
            if (pos.y > 0)
                return 0;
            return 270;
        }

        public Vector2 Angle2Direction(float angle)
        {
            if (angle == 0)
                return new Vector2(0,1);
            if (angle == 270)
                return new Vector2(1,0);
            if (angle == 180)
                return new Vector2(0,-1);
            return new Vector2(-1,0);
        }

        public void RenderBuilding(Building building)
        {
            if (building.type == "vein")
            {
                var obj = Instantiate(veinPrefab, Vector3.zero, Quaternion.identity);
                building.obj = obj;
                obj.transform.position = new Vector3(building.pos.x - 0.5f, building.pos.y - 0.6f, 0f);
                return;
            }
            else
            {
                var obj = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);
                building.obj = obj;
                obj.transform.position = new Vector3(building.pos.x - 0.5f, building.pos.y - 0.5f, 0f);
                foreach (BuildingImage setting in images)
                {
                    if (setting.name == building.type)
                        obj.GetComponent<SpriteRenderer>().sprite = setting.image;
                }
            }
        }

        public void RenderCell(Cell cell)
        {
            var obj = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
            cell.obj = obj;
            obj.transform.position = new Vector3(cell.pos.x, cell.pos.y, 0f);
        }

        public bool GenerateMap()
        {

            for (int x=0; x<width+1; x++)
            {
                var obj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
                Vector3[] points = new Vector3[] {
                    new Vector3(x, height, 0),
                    new Vector3(x, 0, 0),
                };
                lineRenderer.SetPositions(points);
            }
            for (int y=0; y<height+1; y++)
            {
                var obj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
                Vector3[] points = new Vector3[] {
                    new Vector3(0, y, 0),
                    new Vector3(width, y, 0),
                };
                lineRenderer.SetPositions(points);
            }

            return true;
        }

        void Start()
        {
            // initiate a map
            GenerateMap();
            Application.targetFrameRate = 45;
        }

        void Update() 
        {
            
        }
    }

}
