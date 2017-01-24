using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ChewingGum
{
    public class ConvertTime
    {
        private Texture2D[] timeItem;

        //private const float scale = 1.0f;

        public ConvertTime(Game game)
        {
            timeItem = new Texture2D[10];
            for(int i = 0; i < timeItem.Length; i++)
            {
                timeItem[i] = game.Content.Load<Texture2D>(@"res\img\TimeItem\" + i);
            }
            
        }
        
        public Texture2D ToImage(string time)
        {
            switch (time)
            {
                case "0":
                    return timeItem[0];

                case "1":
                    return timeItem[1];

                case "2":
                    return timeItem[2];

                case "3":
                    return timeItem[3];

                case "4":
                    return timeItem[4];

                case "5":
                    return timeItem[5];

                case "6":
                    return timeItem[6];

                case "7":
                    return timeItem[7];

                case "8":
                    return timeItem[8];

                case "9":
                    return timeItem[9];

                default:
                    return null;
            }
            
        }

    }
}
