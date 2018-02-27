// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-27 16:05
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
using System.Collections.Generic;
using UnityEngine;

namespace UnrealM
{
    //������������
    public class ActionSequence
    {
        internal static readonly ObjectPool<ActionSequence> opSequences = new ObjectPool<ActionSequence>(64);

        //�ڵ��б�Ĭ�ϰ�����������Ϊ8
        public readonly List<ActionNode> nodes = new List<ActionNode>(8);

        //��ǰִ�еĽڵ�����
        private int curNodeIndex = 0;

        //ʱ����
        public float timeAxis;

        //Ŀ�������������ٵ�ʱ�򣬱���������Ҳ��Ӧ����
        public Component id { get; private set; }
        private bool isWithoutID = false;

        //��Ҫѭ���Ĵ���
        public int loopTime { get; private set; }

        //�Ѿ����еĴ���
        public int cycles { get; private set; }

        //�Ƿ��Ѿ�������
        public bool isFinshed { get; private set; }

#if UNITY_EDITOR
        public static void GetObjectPoolInfo(out int countActive, out int countAll)
        {
            countActive = opSequences.countActive;
            countAll = opSequences.countAll;
        }
#endif

        //����ֹͣ
        public void Stop()
        {
            id = null;
            isFinshed = true;
            cycles = 0;
            loopTime = 0;
        }

        //����һ�����нڵ�
        public ActionSequence Interval(float interval)
        {
            nodes.Add(ActionNodeInterval.Get(interval));
            return this;
        }

        //����һ�������ڵ�
        public ActionSequence Action(Action action)
        {
            nodes.Add(ActionNodeAction.Get(action));
            return this;
        }

        //����һ����ѭ�������Ķ����ڵ�
        public ActionSequence Action(Action<int> action)
        {
            ActionNodeAction actionNodeAction = ActionNodeAction.Get(action);
            nodes.Add(actionNodeAction);
            return this;
        }

        //����һ�������ڵ�
        public ActionSequence Condition(Func<bool> condition)
        {
            nodes.Add(ActionNodeCondition.Get(condition));
            return this;
        }

        //����ѭ��
        public ActionSequence Loop(int loopTime = -1)
        {
            if (loopTime > 0)
            {
                this.loopTime = loopTime - 1;
                return this;
            }

            this.loopTime = loopTime;
            return this;
        }

        //��������
        private ActionSequence Start(Component id)
        {
            this.id = id;
            curNodeIndex = 0;
            isFinshed = false;
            cycles = 0;
            timeAxis = 0;
            isWithoutID = false;
            return this;
        }

        private ActionSequence Start()
        {
            isWithoutID = true;
            curNodeIndex = 0;
            isFinshed = false;
            cycles = 0;
            timeAxis = 0;
            return this;
        }

        //���и���
        public void Update(float deltaTime)
        {
            //�������Ѿ�Stop��
            if (isFinshed)
            {
                return;
            }

            //����������id��������
            if (!isWithoutID && id == null)
            {
                isFinshed = true;
                return;
            }

            //��������û�м��κνڵ�
            if (nodes.Count == 0)
            {
                isFinshed = true;
                return;
            }

            timeAxis += deltaTime;

            //���������½ڵ�
            if (nodes[curNodeIndex].Update(this))
            {
                curNodeIndex++;
                if (curNodeIndex >= nodes.Count)
                {
                    //����ѭ���Ľڵ�
                    if (loopTime < 0)
                    {
                        Restart();
                        return;
                    }

                    //ѭ���Ľڵ���Ҫ�������������д���++
                    if (loopTime > cycles)
                    {
                        Restart();
                        return;
                    }

                    //���д���>=ѭ�������ˣ���ֹͣ
                    isFinshed = true;
                }
            }
        }

        //�������У����������еĽڵ�
        internal void Release()
        {
            cycles = 0;
            opSequences.Release(this);
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Release();
            }

            nodes.Clear();
        }

        internal void UpdateTimeAxis(float interval)
        {
            timeAxis -= interval;
        }

        //��������
        private void Restart()
        {
            cycles++;
            curNodeIndex = 0;
            timeAxis = 0;
        }

        internal static ActionSequence GetInstance(Component component)
        {
            return opSequences.Get().Start(component);
        }

        internal static ActionSequence GetInstance()
        {
            return opSequences.Get().Start();
        }
    }
}