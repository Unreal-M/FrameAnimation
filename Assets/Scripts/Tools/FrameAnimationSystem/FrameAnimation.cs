// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-27 11:14
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

//#define DOTween
#if DOTween
using DG.Tweening;
#endif

namespace UnrealM
{
    //֡���½ӿ�
    public interface IFrameUpdater
    {
        void OnFrame(int frame);
        void OnComplete();
    }

    [Serializable]
    public class FrameAnimation
    {
        //ѭ������
        public enum LoopType
        {
            Once,
            Loop,
            PingPong
        };

        //���ŷ���
        public enum PlayOrder
        {
            Forward,
            Backwards
        };

        public float FPS = 12;

        //ѭ������
        public LoopType loopType = LoopType.Loop;

        //���ŷ���
        public PlayOrder playOrder;

        [NonSerialized]
        public int nLength = 0;

        private int nSign = 1;

        //���½ӿ�
        public IFrameUpdater iFrameUpdater;

        //ѭ�����º���
        private Action<FrameAnimation> loopTypeFun;

        public int nCurFrame { get; private set; }
        public bool isPlaying { get; private set; }
        public int nStartFrame { get; private set; }

        public void Stop()
        {
            isPlaying = false;
#if DOTween
        DOTween.Kill(this);
#else
            if (iFrameUpdater != null)
            {
                iFrameUpdater.StopSequence();
            }
#endif
        }

        public void Play()
        {
            //ֹͣ��ǰ�Ķ���
            Stop();
            if (nLength <= 0)
            {
                return;
            }

            //������˳��
            if (playOrder == PlayOrder.Forward)
            {
                nStartFrame = 0;
                nSign = 1;
            }
            else
            {
                nStartFrame = nLength - 1;
                nSign = -1;
            }

            //����ѭ��ģʽ
            switch (loopType)
            {
                case LoopType.Once:
                    loopTypeFun = UpdateFrameFuncOnce;
                    break;
                case LoopType.Loop:
                    loopTypeFun = UpdateFrameFuncLoop;
                    break;
                case LoopType.PingPong:
                    loopTypeFun = UpdateFrameFuncPingPong;
                    break;
                default:
                    break;
            }

            float interval = 1/FPS;
            nCurFrame = nStartFrame;

            //����ʹ����ActionSequenceSystem��ΪTimer��Ҳ����ʹ��DOTween��Sequence��С��GC������Ȼ��Ҳ����ʹ������Timer
#if DOTween
            DOTween.Sequence().SetId(this).SetRecyclable().AppendInterval(interval).AppendCallback(UpdateFrame).SetLoops(-1);
#else
            iFrameUpdater.Looper(interval, -1, false, UpdateFrame);
#endif

            isPlaying = true;
        }

        private void UpdateFrame()
        {
            if (iFrameUpdater != null)
            {
                iFrameUpdater.OnFrame(nCurFrame);
            }

            nCurFrame += nSign;
            loopTypeFun(this);
        }

        #region ѭ�����͵Ĳ�ͬʵ��

        //ʵ��ѭ������
        private static void UpdateFrameFuncLoop(FrameAnimation frameAnimation)
        {
            if (frameAnimation.nCurFrame >= frameAnimation.nLength || frameAnimation.nCurFrame < 0)
            {
                frameAnimation.nCurFrame = frameAnimation.nStartFrame;
            }
        }

        //ʵ�ֵ��θ���
        private static void UpdateFrameFuncOnce(FrameAnimation frameAnimation)
        {
            if (frameAnimation.nCurFrame >= frameAnimation.nLength || frameAnimation.nCurFrame < 0)
            {
                frameAnimation.Stop();
                if (frameAnimation.iFrameUpdater != null)
                {
                    frameAnimation.iFrameUpdater.OnComplete();
                }
            }
        }

        //ʵ�����ظ���
        private static void UpdateFrameFuncPingPong(FrameAnimation frameAnimation)
        {
            if (frameAnimation.nSign == 1)
            {
                if (frameAnimation.nCurFrame >= frameAnimation.nLength)
                {
                    frameAnimation.nSign = -1;
                    frameAnimation.nCurFrame -= 2;
                }
            }
            else
            {
                if (frameAnimation.nCurFrame < 0)
                {
                    frameAnimation.nSign = 1;
                    frameAnimation.nCurFrame += 2;
                }
            }
        }
        #endregion
    }
}