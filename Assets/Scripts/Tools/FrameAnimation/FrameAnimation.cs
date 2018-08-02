// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Date: 2018-07-28 16:30
//  ****************************************************************************/

using System;
namespace UnrealM
{
    //帧更新接口
    public interface IFrameUpdater
    {
        void OnFrame(int frame);
        void OnComplete();
    }

    [Serializable]
    public class FrameAnimation : ActionSequenceHandle
    {
        //循环类型
        public enum LoopType
        {
            Once,
            Loop,
            PingPong
        };

        //播放方向
        public enum PlayOrder
        {
            Forward,
            Backwards
        };

        public float FPS = 12;

        //循环类型
        public LoopType loopType = LoopType.Loop;

        //播放方向
        public PlayOrder playOrder = PlayOrder.Forward;

        [NonSerialized]
        public int nLength = 0;

        private int nSign = 1;

        //更新接口
        public IFrameUpdater iFrameUpdater;

        //循环更新函数
        private Action<FrameAnimation> loopTypeFun;
        public int nCurFrame { get; private set; }
        public bool isPlaying { get; private set; }
        public int nStartFrame { get; private set; }

        public void Stop()
        {
            isPlaying = false;
            this.StopSequence();
        }

        public void Play()
        {
            //停止当前的动画
            Stop();
            if (nLength <= 0)
            {
                return;
            }

            //处理播放顺序
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

            //处理循环模式
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

            float interval = 1 / FPS;
            nCurFrame = nStartFrame;
            isPlaying = true;

            //开启定时器
            //这里使用了ActionSequenceSystem作为Timer，也可以使用DOTween的Sequence，当然您也可以使用您的Timer
            this.Looper(interval, -1, false, UpdateFrame);
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

        #region 循环类型的不同实现
        //实现循环更新
        private static void UpdateFrameFuncLoop(FrameAnimation frameAnimation)
        {
            if (frameAnimation.nCurFrame >= frameAnimation.nLength || frameAnimation.nCurFrame < 0)
            {
                frameAnimation.nCurFrame = frameAnimation.nStartFrame;
            }
        }

        //实现单次更新
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

        //实现来回更新
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