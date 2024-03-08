// Author: Chrysalis shiyuchongf@gmail.com

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace Chrysalis.Core
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance = null;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<UIManager>();
                if (_instance == null) throw new Exception("UI not found");
                return _instance;
            }
        }

        [NonSerialized]
        public Dictionary<string, UIElement> ComponentsList = new Dictionary<string, UIElement>();

        public void Warning(string msg)
        {
            ((AlertPanel)ComponentsList["alert"]).Highlight(msg);
            Debug.Log(msg);
        }
    }
}
