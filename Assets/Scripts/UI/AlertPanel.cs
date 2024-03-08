// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Chrysalis.Core;

namespace Chrysalis.Core
{
    public class AlertPanel : UIElement
    {
        [SerializeField]
        public Text text;
        [SerializeField]
        public Image background;

        public override void UIUpdate()
        {
            if (background.color.a == 0) return;
            if (background.color.a <= 0.1)
            {
                gameObject.SetActive(false);
                background.color = new Color(1,0,0,0);
            }
            else
                background.color = new Color(1, 0, 0, background.color.a * 0.95f);
        }

        public void Highlight(string msg)
        {
            text.text = msg;
            gameObject.SetActive(true);
            background.color = new Color(1,0,0,1);
        }
    }
}
