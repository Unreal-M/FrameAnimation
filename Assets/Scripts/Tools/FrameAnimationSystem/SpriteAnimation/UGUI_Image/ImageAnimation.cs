// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-27 11:15
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/

using UnityEngine.UI;

namespace UnrealM
{
    public class ImageAnimation : SpriteAnimation
    {
        public Image image;

        private void Awake()
        {
            image.enabled = !isHiddenAtAwake;
        }

        public override void OnFrame(int nCurFrame)
        {
            //DOTween有BUG，有时候会越界
            //if (nCurFrame < curSprites.Length)
            //{
            image.sprite = curSprites[nCurFrame];

            //}
        }

#if UNITY_EDITOR
        private void Reset()
        {
            image = GetComponent<Image>();
            if (image.sprite)
            {
                SpriteAnimationClip.CreateSpriteAnimationClip(this, image.sprite);
                if (clips.Length > 0)
                {
                    if (clips.clips[0].sprites.Length < 3)
                    {
                        frameAnimation.FPS = 3;
                    }
                }
            }
        }
#endif
    }
}