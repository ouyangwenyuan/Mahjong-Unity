using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GamePlus.bean
{
    public class NativeAdInfo
    {
        //标题
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        //描述
        private string socialContext;

        public string SocialContext
        {
            get { return socialContext; }
            set { socialContext = value; }
        }
        //按钮文字
        private string installTxt;

        public string InstallTxt
        {
            get { return installTxt; }
            set { installTxt = value; }
        }

        //封面图片
        private Sprite coverImage;

        public Sprite CoverImage
        {
            get { return coverImage; }
            set { coverImage = value; }
        }
        //宣传图标
        private Sprite iconImage;

        public Sprite IconImage
        {
            get { return iconImage; }
            set { iconImage = value; }
        }
    }
}
