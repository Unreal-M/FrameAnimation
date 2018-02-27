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

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UnrealM
{
    [CustomEditor(typeof(ImageAnimation), true)]
    [CanEditMultipleObjects]
    public class ImageAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (targets.Length == 1)
            {
                if (GUILayout.Button("Find/Create Clip By Image.sprite"))
                {
                    ImageAnimation s = target as ImageAnimation;
                    Image image = s.GetComponent<Image>();
                    if (!image)
                    {
                        Debug.LogWarning("Image if null");
                        return;
                    }

                    if (!image.sprite)
                    {
                        Debug.LogWarning("Image.sprite if null");
                        return;
                    }

                    SpriteAnimationClip.CreateSpriteAnimationClip(s, image.sprite);
                }
            }

            if (GUILayout.Button("Setup First Frame"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    ImageAnimation s = targets[i] as ImageAnimation;
                    s.image = s.GetComponent<Image>();
                    if (s.image)
                    {
                        Undo.RecordObject(s.image, "Setup First Frame");
                        s.image.sprite = s.clips[0].sprites[0];
                        s.image.SetNativeSize();
                    }
                }
            }

            base.OnInspectorGUI();
        }
    }
}