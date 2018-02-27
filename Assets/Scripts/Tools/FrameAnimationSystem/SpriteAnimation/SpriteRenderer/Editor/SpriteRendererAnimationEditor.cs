// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-27 11:16
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

using UnityEditor;
using UnityEngine;

namespace UnrealM
{
    [CustomEditor(typeof(SpriteRendererAnimation), true)]
    [CanEditMultipleObjects]
    public class SpriteRendererAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (targets.Length == 1)
            {
                if (GUILayout.Button("Find/Create Clip By SpriteRenderer.sprite"))
                {
                    SpriteRendererAnimation s = target as SpriteRendererAnimation;
                    SpriteRenderer spriteRenderer = s.GetComponent<SpriteRenderer>();
                    s.spriteRenderer = spriteRenderer;
                    if (!spriteRenderer)
                    {
                        Debug.LogWarning("SpriteRenderer if null");
                        return;
                    }

                    if (!spriteRenderer.sprite)
                    {
                        Debug.LogWarning("SpriteRenderer.sprite if null");
                        return;
                    }

                    SpriteAnimationClip.CreateSpriteAnimationClip(s, spriteRenderer.sprite);
                }
            }

            if (GUILayout.Button("Setup First Frame"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    SpriteRendererAnimation s = targets[i] as SpriteRendererAnimation;
                    s.spriteRenderer = s.GetComponent<SpriteRenderer>();
                    if (s.spriteRenderer)
                    {
                        s.spriteRenderer.sprite = s.clips[0].sprites[0];
                    }
                }
            }

            base.OnInspectorGUI();
        }
    }
}