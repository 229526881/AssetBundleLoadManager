/*
 * Title:Unity的工具类方法
 * Date:2021.1.14
 * 参考:
*/

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SFramework
{
    public class Utility
    {
        public static void SetParent(Transform p, Transform c)
        {
            if (p == null || c == null)
            {
                DIYLog.LogError("设置父类，其中一个为空");
                return;
            }
            c.SetParent(p);
            c.localScale = Vector3.one;
            c.localPosition = Vector3.zero;
        }


        /// <summary>设置父物体 重置缩放</summary>
        public static void SetParentNoPos(Transform p, GameObject c)
        {
            if (c == null || p == null)
            {
                DIYLog.LogError("设置父类，其中一个为空");
                return;
            }
            c.transform.parent = p;
            c.transform.localScale = Vector3.one;

        }

        /// <summary>设置父物体 不重置位置、不重置缩放</summary>
        public static void SetParentOnly(Transform p, Transform c)
        {
            if (c == null || p == null)
            {
                DIYLog.LogError("设置父类，其中一个为空");
                return;
            }
            c.parent = p;
        }

        public static bool GetMaxDepth(Transform layerobj, out int maxDepth)
        {
            maxDepth = 0;
            if (layerobj == null || layerobj.transform.parent == null)
            {
                return false;
            }
            maxDepth = layerobj.transform.childCount;
            return true;
        }

        public static T GetObject<T>(Transform parent, string path) where T : UnityEngine.Object
        {
            if (parent == null) return null;
            Transform transfrom = parent ? parent.Find(path) : parent;
            if (transfrom == null) return null;

            if (typeof(T) == typeof(Transform)) return transfrom as T;

            if (typeof(T) == typeof(GameObject)) return transfrom.gameObject as T;

            return transfrom.GetComponent(typeof(T)) as T;
        }

        public static T[] GetObjects<T>(Transform parent, string path) where T : UnityEngine.Object
        {
            if (parent == null) return null;

            Transform transfrom = parent ? parent.Find(path) : parent;
            if (transfrom == null) return null;
            return transfrom.GetComponentsInChildren(typeof(T)) as T[];
        }


        /// <summary>
        /// 深度寻找子节点transfrom
        /// </summary>
        /// <param name="_target"></param>
        /// <param name="_childName"></param>
        /// <returns></returns>
        public static Transform FindDeepChild(GameObject _target, string _childName)
        {
            Transform transform = _target.transform.Find(_childName);
            if (transform == null)
            {
                foreach (object obj in _target.transform)
                {
                    Transform transform2 = obj as Transform;
                    transform = FindDeepChild(transform2.gameObject, _childName);
                    if (transform != null)
                    {
                        return transform;
                    }
                }
            }
            return transform;
        }

        public static void RemoverAllChildren(GameObject target)
        {
            if (target.transform.childCount <= 0)
            {
                return;
            }
            for (int i = 0; i < target.transform.childCount; i++)
            {
                GameObject.Destroy(target.transform.GetChild(i));
            }
        }

        /// <summary>
        /// 深度寻找子组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_target"></param>
        /// <param name="_childName"></param>
        /// <returns></returns>
        public static T FindDeepChild<T>(GameObject _target, string _childName) where T : Component
        {
            Transform transform = FindDeepChild(_target, _childName);
            if (transform != null)
            {
                return transform.gameObject.GetComponent<T>();
            }
            return (T)((object)null);
        }

        /// <summary>
        /// 计算向量的夹角，360范围,使用unity 自带的方法计算
        /// </summary>
        /// <param name="ve1"></param>
        /// <param name="ve2"></param>
        /// <returns></returns>
        public static float GetAngle(Vector3 ve1, Vector3 ve2)
        {
            float angle = Vector3.Angle(ve1, ve2);
            float sign = Mathf.Sign(Vector3.Cross(ve1, ve2).z);
            if (sign > 0)
            {
                angle = 360 - angle;
            }

            return angle;
        }

        /// <summary>
        /// 返回-180-180的夹角
        /// </summary>
        /// <param name="ve1"></param>
        /// <param name="ve2"></param>
        /// <returns></returns>
        public static float GetAngleHalf(Vector3 ve1, Vector3 ve2)
        {
            float angle = Vector3.Angle(ve1, ve2);
            float sign = Mathf.Sign(Vector3.Cross(ve1, ve2).z);
            if (sign > 0)
            {
                angle = 360 - angle;
            }
            return sign > 0 ? -angle : angle;
        }

        /// <summary>
        /// 使用原生的方法计算360的角度 注意acos得到的是弧度
        /// </summary>
        /// <param name="ve1"></param>
        /// <param name="ve2"></param>
        /// <returns></returns>
        public static float GetAngleRaw(Vector3 ve1, Vector3 ve2)
        {
            float angle = Mathf.Acos(Dot(ve1, ve2)) * Mathf.Rad2Deg;
            float sign = Mathf.Sign(Cross(ve1, ve2).z);
            if (sign > 0)
            {
                angle = 360 - angle;
            }
            return angle;
        }

        /// <summary>
        /// 使用原生的方法计算-180-180的角度 注意acos得到的是弧度
        /// </summary>
        /// <param name="ve1"></param>
        /// <param name="ve2"></param>
        /// <returns></returns>
        public static float GetAngleRawHalf(Vector3 ve1, Vector3 ve2)
        {
            float angle = Mathf.Acos(Dot(ve1, ve2)) * Mathf.Rad2Deg;
            float sign = Mathf.Sign(Cross(ve1, ve2).z);
            return sign > 0 ? -angle : angle;
        }

        public static bool CheckRequestState(UnityWebRequest req)
        {
            bool state = true;
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                state = false;
            }
            return state;
        }

        /// <summary>
        /// 获取当前时间的毫秒级
        /// </summary>
        /// <returns></returns>
        public static long GetTimeNow()
        {
            DateTime time = DateTime.Now.ToUniversalTime();
            return (time.Ticks - 621355968000000000) / 10000;
        }

        public static string GetCurTime()
        {
            DateTime NowTime = DateTime.Now.ToLocalTime();
            return NowTime.ToString("yyyy-MM-dd-HH-mm");
        }


        /// <summary>
        /// 当前时间与目标时间间隔
        /// </summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static string GetTimeDes(long endTime)
        {
            DateTime orginDate = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime startDate = orginDate.AddMilliseconds(GetTimeNow());
            DateTime endDate = orginDate.AddMilliseconds(endTime);

            TimeSpan beginTs = new TimeSpan(startDate.Ticks);
            TimeSpan endTs = new TimeSpan(endDate.Ticks);
            TimeSpan result = endTs.Subtract(beginTs).Duration();

            return result.ToString("c").Substring(0, 8);
        }

        /// <summary>
        /// 根据时间间隔来展示,目前只有分和秒
        /// </summary>
        /// <returns></returns>
        public static string GetTimeDes(int delta)
        {
            int m = delta / 60;
            int s = delta % 60;
            string mes = (m < 10 ? "0" : "") + m + ":" + (s < 10 ? "0" : "") + s;
            return mes;
        }

        //判断时间是否在白天之间
        public static bool CheckIsNight(long time)
        {
            time /= 1000;
            Int64 begtime = Convert.ToInt64(time) * 10000000;//100毫微秒为单位,textBox1.text需要转化的int日期
            DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTim
          
            //time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            string _strWorkingDayAM = "06:00";
            string _strWorkingDayPM = "19:00";
            TimeSpan dspWorkingDayAM = DateTime.Parse(_strWorkingDayAM).TimeOfDay;
            TimeSpan dspWorkingDayPM = DateTime.Parse(_strWorkingDayPM).TimeOfDay;
            //string time1 = "2017-2-17 8:10:00";
            TimeSpan dspNow = dt.TimeOfDay;
            if (dspNow > dspWorkingDayAM && dspNow < dspWorkingDayPM)
            {
                return false;
            }
            return true;
        }

        #region  Math


        ///// <summary>
        ///// 获得随机数左闭右开
        ///// </summary>
        ///// <returns></returns>
        //public static int GetRandomInt(int min,int max)
        //{
        //     int UnityEngine.Random.Range(min,max);
        //}
        public static Vector3 Cross(Vector3 ve1, Vector3 ve2)
        {
            return new Vector3(ve1.y * ve2.z - ve1.z * ve2.y, ve1.z * ve2.x - ve1.x * ve2.z, ve1.x * ve2.y - ve2.x * ve1.y);
        }

        public static float Dot(Vector3 ve1, Vector3 ve2)
        {
            return ve1.x * ve2.x + ve1.y * ve2.y + ve1.z * ve2.z;
        }

        public static string GetPercent(float rate)
        {
            return Mathf.Floor(rate * 100) + "%";
        }

        ///// <summary>
        ///// 检测2个矩形是否相交,只适用于平行于边平行于X轴和Y轴的四边形
        ///// </summary>
        ///// <param name="ve1"></param>
        ///// <param name="ve2"></param>
        ///// <returns></returns>
        //public static bool CheckAABB(Vector4 ve1, Vector4 ve2)
        //{
        //    //首先得获得每个四边形其中左下角和 右上角2个顶点
        //    return CheckAABB(new RectPoint(ve1), new RectPoint(ve2));
        //}

        //public static bool CheckAABB(RectPoint point1, RectPoint point2)
        //{
        //    //判断最大的依据就是，较大的左下角的X小于较大的右上角的X，较小的右上角的Y大于
        //    bool res1 = Mathf.Max(point1.leftBottom.x, point2.leftBottom.x) < Mathf.Min(point1.rightTop.x, point2.rightTop.x);
        //    bool res2 = Mathf.Min(point1.rightTop.y, point2.rightTop.y) > Mathf.Max(point1.leftBottom.y, point2.leftBottom.y);
        //    return res1 && res2;
        //}

        public static Vector2[] GetVertexPoint(Bounds bounds)
        {
            //要从左下角到右上角的顺序过去
            Vector2[] points = new Vector2[4];
            points[0] = bounds.center - bounds.extents;
            points[1] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y);
            points[2] = bounds.center + bounds.extents;
            points[3] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y);
            return points;
        }
        #endregion



        #region IO

        /// <summary>
        /// 创建文件，如果上层目录不存在，则自动生成文件夹
        /// </summary>
        public static void CheckFileAuto(string path)
        {
            //首先判断是一个文件路径，然后获得上层路径

            //因为包名有.会影响这个逻辑处理
            int lastIndex = path.LastIndexOf('/');
            int pointIndex = path.IndexOf('.');
            if (pointIndex > lastIndex && pointIndex > 0)
            {
                path = path.Substring(0, path.LastIndexOf('.'));
            }
            path = path.Substring(0, path.LastIndexOf('/'));
            CreateAutoDir(path);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void CreateAutoDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("开始创建文件夹" + path);
            }
        }


        /// <summary>
        /// 将一个文件夹下的所有东西复制到另一个文件夹 (可备份文件夹)
        /// </summary>
        /// <param name="sourceDire">源文件夹全名</param>
        /// <param name="destDire">目标文件夹全名</param>
        /// <param name="backupsDire">备份文件夹全名</param>
        public static void CopyDireToDire(string sourceDire, string destDire, string backupsDire = @"F:\AllProject\备份")
        {
            //还需要忽略.meta文件
            sourceDire = sourceDire.Replace("/", "\\");
            destDire = destDire.Replace("/", "\\");
            if (Directory.Exists(sourceDire) && Directory.Exists(destDire))
            {
                DirectoryInfo sourceDireInfo = new DirectoryInfo(sourceDire);
                FileInfo[] fileInfos = sourceDireInfo.GetFiles();
                foreach (FileInfo fInfo in fileInfos) 
                {
                    string sourceFile = fInfo.FullName;
                    //Debug.Log(sourceFile);

                    if (sourceFile.Contains(".meta"))
                    {
                        continue;
                    }
                    // sourceFile= sourceFile.Replace("\\", "/");
                    //Debug.Log(sourceFile);
                    string destFile = sourceFile.Replace(sourceDire, destDire);
                    if (!string.IsNullOrEmpty(backupsDire) && File.Exists(destFile))
                    {
                        Directory.CreateDirectory(backupsDire);
                        string backFile = destFile.Replace(destDire, backupsDire);
                        File.Copy(destFile, backFile, true);
                    }
                    File.Copy(sourceFile, destFile, true);
                }
                DirectoryInfo[] direInfos = sourceDireInfo.GetDirectories();
                foreach (DirectoryInfo dInfo in direInfos)
                {
                    string sourceDire2 = dInfo.FullName;
                    string destDire2 = sourceDire2.Replace(sourceDire, destDire);
                    string backupsDire2 = null;
                    if (backupsDire != null)
                    {
                        backupsDire2 = sourceDire2.Replace(sourceDire, backupsDire);
                    }
                    Directory.CreateDirectory(destDire2);
                    CopyDireToDire(sourceDire2, destDire2, backupsDire2);
                }
            }
        }

        /// <summary>
        /// 放置到手机沙盒目录里会有不同，如果
        /// </summary>
        /// <param name="sourceDire"></param>
        /// <param name="destDire"></param>
        /// <param name="backupsDire"></param>
        public static void CopyDireToDataPath(string sourceDire, string destDire, string backupsDire = @"F:\AllProject\备份")
        {
            sourceDire = sourceDire.Replace("/", "\\");
            destDire = destDire.Replace("/", "\\");
           // destDire =unit
            if (Directory.Exists(sourceDire) && Directory.Exists(destDire))
            {
                DirectoryInfo sourceDireInfo = new DirectoryInfo(sourceDire);
                FileInfo[] fileInfos = sourceDireInfo.GetFiles();
                foreach (FileInfo fInfo in fileInfos)
                {
                    string sourceFile = fInfo.FullName;
                    //Debug.Log(sourceFile);

                    if (sourceFile.Contains(".meta"))
                    {
                        continue;
                    }
                    // sourceFile= sourceFile.Replace("\\", "/");
                    Debug.Log(sourceFile);
                    string destFile = sourceFile.Replace(sourceDire, destDire);
                    if (!string.IsNullOrEmpty(backupsDire) && File.Exists(destFile))
                    {
                        Directory.CreateDirectory(backupsDire);
                        string backFile = destFile.Replace(destDire, backupsDire);
                        File.Copy(destFile, backFile, true);
                    }
                    File.Copy(sourceFile, destFile, true);
                }
                DirectoryInfo[] direInfos = sourceDireInfo.GetDirectories();
                foreach (DirectoryInfo dInfo in direInfos)
                {
                    string sourceDire2 = dInfo.FullName;
                    string destDire2 = sourceDire2.Replace(sourceDire, destDire);
                    string backupsDire2 = null;
                    if (backupsDire != null)
                    {
                        backupsDire2 = sourceDire2.Replace(sourceDire, backupsDire);
                    }
                    Directory.CreateDirectory(destDire2);
                    CopyDireToDire(sourceDire2, destDire2, backupsDire2);
                }
            }
        }
        #endregion


        /// <summary>
        /// 判断是否在范围内
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="sqrDis"></param>
        /// <returns></returns>
        public static bool CheckIsClose(Vector3 point1,Vector3 point2,float sqrDis)
        {
            Vector3 point = point1 - point2;
            return point.sqrMagnitude<=sqrDis;
        }

        #region  Net相关


        /// <summary>
        /// 加载网络资源，支持断点续传
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath">主要要带文件名，也可以自己读路径得到</param>
        /// <param name="progressEvent"></param>
        /// <param name="endEvent"></param>
        /// <returns></returns>
        public static IEnumerator DownLoadAsset(string url, string filePath, Action<float> progressEvent = null, Action endEvent = null)
        {
            float progress = 0;
            UnityWebRequest headRequest = UnityWebRequest.Head(url);

            yield return headRequest.SendWebRequest();

            if (!CheckRequestState(headRequest))
            {
                Debug.LogError(url + "资源下载错误");
                yield break;
            }
            string fileName = url.Substring(url.LastIndexOf("/"));
            if (filePath.IndexOf(".") < 0 || Path.GetExtension(filePath) == null)
            {
                filePath += fileName;
            }
            long totalLength = long.Parse(headRequest.GetResponseHeader("Content-Length"));

            var dirPath = Path.GetDirectoryName(filePath);
            //先创建文件夹
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            bool isStop = false;
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                long fileLength = fs.Length;
                if (fileLength < totalLength)
                {
                    fs.Seek(fs.Length, SeekOrigin.Begin);

                    var request = UnityWebRequest.Get(url);
                    request.SetRequestHeader("Range", "bytes=" + fileLength + "-" + totalLength);
                    request.SendWebRequest();

                    var index = 0;
                    while (!request.isDone)
                    {
                        if (isStop) break;
                        yield return null;
                        var buff = request.downloadHandler.data;
                        if (buff != null)
                        {
                            var length = buff.Length - index;
                            fs.Write(buff, index, length);
                            index += length;
                            fileLength += length;

                            if (fileLength == totalLength)
                            {
                                progress = 1f;
                                isStop = true;
                                //此时应该是下载结束了
                                endEvent?.Invoke();
                            }
                            else
                            {
                                progress = fileLength / (float)totalLength;
                                progressEvent?.Invoke(progress);
                            }
                        }

                    }
                }
            }
        }


        /// <summary>
        /// 加载网络图片
        /// </summary>
        /// <param name="url"></param>
        public static IEnumerator LoadNetSprite(string url,Action<Sprite> endEvent)
        {
            UnityWebRequest wr = new UnityWebRequest(url);
            DownloadHandlerTexture texD1 = new DownloadHandlerTexture(true);
            wr.downloadHandler = texD1;
            yield return wr.SendWebRequest();
            if (CheckRequestState(wr))
            {
                Texture2D tex = texD1.texture;
                //保存本地          
                // Byte[] bytes = tex.EncodeToPNG();
                //File.WriteAllBytes(Application.dataPath + "/02.png", bytes); //这是写在本地了，实际不需要这么做
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                endEvent?.Invoke(sprite);
            }
        }

        public static IEnumerator LoadLocalAsset(string url, Action<string> endEvent)
        {
            UnityWebRequest req =  UnityWebRequest.Get(url);
            yield return req.SendWebRequest();
            if (!CheckRequestState(req))
            {
                DIYLog.LogError("获取Json失败"+url);
                yield break;
            }
            string text = req.downloadHandler.text;
            endEvent?.Invoke(text);
        }
    }
    #endregion
}