// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-27 11:13
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

using System;
using UnityEngine;

namespace UnrealM
{
    public abstract class SpriteAnimation : MonoBehaviour, IFrameUpdater
    {
        public FrameAnimation frameAnimation = new FrameAnimation();

        //自动播放
        public bool autoPlay = true;

        public bool isHiddenAtAwake;

        //播放结束模式
        public enum EndMode
        {
            DoNothing,
            Hide,
            Destory,
            DisplayFirstFrame
        };

        public EndMode endMode;
        public Action onComplete;

        [HideInInspector]
        public int nCurClipIndex = 0;

        public SpriteAnimationClip clips;

        public bool isPlaying { get { return frameAnimation.isPlaying; } }
        protected Sprite[] curSprites { get { return clips[nCurClipIndex].sprites; } }

        private void OnEnable()
        {
            if (autoPlay)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Stop()
        {
            frameAnimation.Stop();
        }

        public void Play(int nClipIndex)
        {
            nCurClipIndex = nClipIndex;
            Play();
        }

        public void Play()
        {
            //停止当前的动画
            frameAnimation.iFrameUpdater = this;
            frameAnimation.Stop();
            frameAnimation.nLength = curSprites.Length;
            frameAnimation.Play();
        }

        public abstract void OnFrame(int nCurFrame);

        public void OnComplete()
        {
            //Debug.Log("OnPlayComplete");
            switch (endMode)
            {
                case EndMode.Hide:
                    gameObject.SetActive(false);
                    break;
                case EndMode.Destory:
                    Destroy(gameObject);
                    break;
                case EndMode.DisplayFirstFrame:
                    OnFrame(frameAnimation.nStartFrame);
                    break;
                case EndMode.DoNothing:
                default:
                    break;
            }

            if (onComplete != null)
            {
                onComplete();
            }
        }

        private void OnDestroy()
        {
            Stop();
        }
    }
}