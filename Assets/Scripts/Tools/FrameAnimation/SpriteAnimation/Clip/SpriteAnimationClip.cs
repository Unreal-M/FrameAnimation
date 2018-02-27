// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-27 11:19
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
#if UNITY_EDITOR
using UnityEditor;
using System.IO;

#endif

namespace UnrealM
{
    public class SpriteAnimationClip : ScriptableObject
    {
        [Serializable]
        public class Clip
        {
            public Sprite[] sprites = new Sprite[1];
        }

        public int Length { get { return clips.Length; } }
        public Clip[] clips = new Clip[1];
        public Clip this[int index] { get { return clips[index]; } }

#if UNITY_EDITOR

        //对序列帧排序
        //public void Sort()
        //{
        //    for (int i = 0; i < clips.Length; i++)
        //    {
        //        if (clips[i].sprites.Length > 1)
        //        {
        //            Array.Sort(clips[i].sprites, (n1, n2) => n1.name.CompareTo(n2.name));
        //        }
        //    }
        //}

        public static int CheckSpriteNameIsFormat(Sprite sprite)
        {
            string strName = sprite.name;
            string strNumber = strName.Remove(0, strName.Length - 2);
            int nOut = 0;
            if (int.TryParse(strNumber, out nOut))
            {
                return 2;
            }

            strNumber = strName.Remove(0, strName.Length - 1);
            return int.TryParse(strNumber, out nOut) ? 1 : 0;
        }

        private const string strFormat1 = "{0}{1}.png";
        private const string strFormat2 = "{0}{1:00}.png";

        //用第一帧去加载其他帧
        public void LoadByFirstFrame()
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].sprites.Length == 0) //没有第一帧的跳过
                {
                    continue;
                }

                int nIndex = 0;
                bool bGetOne = false;
                int nStartIndex = 0;
                int nEndNumCount = CheckSpriteNameIsFormat(clips[i].sprites[0]);

                //缓存数组
                Sprite[] spritesCache = new Sprite[100]; //应该没有可能超过100张图的动画吧

                //制作路径
                string strFirstFramePath = AssetDatabase.GetAssetPath(clips[i].sprites[0]);
                string strPathFormat = strFirstFramePath.Remove(strFirstFramePath.LastIndexOf('.') - nEndNumCount);
                Debug.Log("Load Path: " + strPathFormat);
                string strFormat = nEndNumCount == 1 ? strFormat1 : strFormat2;
                while (nIndex < 100)
                {
                    string strFramePath = string.Format(strFormat, strPathFormat, nIndex);
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(strFramePath);
                    if (!sprite)
                    {
                        if (bGetOne)
                        {
                            break;
                        }

                        nStartIndex++;
                        nIndex++; //下一张图
                        continue;
                    } //中断

                    bGetOne = true;
                    spritesCache[nIndex - nStartIndex] = sprite;
                    nIndex++; //下一张图
                }

                //新建数组并填充
                clips[i].sprites = new Sprite[nIndex - nStartIndex];
                for (int j = 0; j < clips[i].sprites.Length; j++)
                {
                    clips[i].sprites[j] = spritesCache[j];
                }
            }

            EditorUtility.SetDirty(this);
        }

        public static bool CreateSpriteAnimationClip(SpriteAnimation spriteAnimation, Sprite sprite)
        {
            int nEndNumCount = CheckSpriteNameIsFormat(sprite);
            if (nEndNumCount == 0)
            {
                Debug.Log("Sprite naming is not in Format! Check file name is end with \"_xx.png\"(xx is Number)");
                return false;
            }

            string assetPath = AssetDatabase.GetAssetPath(sprite);
            string assetFile = assetPath.Remove(assetPath.LastIndexOf('.'));
            string assetDatabaseFile = assetFile.Remove(assetFile.Length - nEndNumCount) + ".asset";
            string file = Application.dataPath.Remove(Application.dataPath.LastIndexOf('/') + 1) + assetDatabaseFile;
            SpriteAnimationClip clipFile;

            // 如果文件已经存在
            if (File.Exists(file))
            {
                Debug.Log("SpriteAnimationClip file existed");

                //Debug.Log(assetDatabaseFile);
                clipFile = AssetDatabase.LoadAssetAtPath<SpriteAnimationClip>(assetDatabaseFile);
                clipFile.clips[0].sprites[0] = sprite;
                clipFile.LoadByFirstFrame();
                SetClip(spriteAnimation, clipFile);
                return true;
            }

            // 实例化类
            ScriptableObject clip = CreateInstance<SpriteAnimationClip>();

            // 如果实例为空，返回
            if (!clip)
            {
                Debug.LogWarning("SpriteAnimationClip not found");
                return false;
            }

            // 如果项目总不包含该路径，创建一个
            string assetDatabaseDirectory = file.Remove(file.LastIndexOf('/') + 1);
            if (!Directory.Exists(assetDatabaseDirectory))
            {
                Directory.CreateDirectory(assetDatabaseDirectory);
            }

            // 生成自定义资源到指定路径
            AssetDatabase.CreateAsset(clip, assetDatabaseFile);
            clipFile = AssetDatabase.LoadAssetAtPath<SpriteAnimationClip>(assetDatabaseFile);
            clipFile.clips[0].sprites[0] = sprite;
            clipFile.LoadByFirstFrame();
            SetClip(spriteAnimation, clipFile);
            return true;
        }

        private static void SetClip(SpriteAnimation spriteAnimation, SpriteAnimationClip clip)
        {
            //Selection.activeObject = clip;
            EditorGUIUtility.PingObject(clip);
            if (spriteAnimation)
            {
                spriteAnimation.clips = clip;
            }
        }

        public static bool CreateSpriteAnimationClip(string name)
        {
            // 自定义资源保存路径
            string file = Application.dataPath + "/_Media/SpriteAnimationClips/" + name + ".asset";
            string assetDatabaseFile = "Assets/_Media/SpriteAnimationClips/" + name + ".asset";
            SpriteAnimationClip clipFile;

            // 如果文件已经存在
            if (File.Exists(file))
            {
                Debug.Log("SpriteAnimationClip file existed");
                clipFile = AssetDatabase.LoadAssetAtPath<SpriteAnimationClip>(assetDatabaseFile);
                SetClip(null, clipFile);
                return true;
            }

            // 实例化类
            ScriptableObject clip = CreateInstance<SpriteAnimationClip>();

            // 如果实例为空，返回
            if (!clip)
            {
                Debug.LogWarning("SpriteAnimationClip not found");
                return false;
            }

            // 自定义资源保存路径
            string path = Application.dataPath + "/_Media/SpriteAnimationClips";

            // 如果项目总不包含该路径，创建一个
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 生成自定义资源到指定路径
            AssetDatabase.CreateAsset(clip, assetDatabaseFile);
            clipFile = AssetDatabase.LoadAssetAtPath<SpriteAnimationClip>(assetDatabaseFile);
            SetClip(null, clipFile);
            return true;
        }
#endif
    }
}