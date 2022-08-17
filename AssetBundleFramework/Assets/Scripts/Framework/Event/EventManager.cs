/*
 * Title:this扩展方法的事件系统
 * Date:2020.10.20
 * 参考: 有一个问题，就是很难找到对应的双方，得加一个字典来方便寻找引用
*/

using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    public static class EventManager
    {
        //考虑是否要在嵌套一层，用一个 类型在来管理貌似不用，但是可能出现多个同时调用的一个调用的先后根据加入的先后时机吧
        public static Dictionary<EventId, List<LogicMsgHandler>> handleDict = new Dictionary<EventId, List<LogicMsgHandler>>();
        public static bool excuteState=true; //用来控制所有时间的接收
        public class VoidDelegate
        {
            public delegate void WithParams(params object[] param);
        }

        public class LogicMsgHandler
        {
            public IMsgRegister receiver;
            public VoidDelegate.WithParams callBack;
            public int excuteTime = -1;// 如果是默认值
            public LogicMsgHandler(IMsgRegister _receiver, VoidDelegate.WithParams _callBack, int _excuteTime = -1)
            {
                receiver = _receiver;
                callBack = _callBack;
                excuteTime = _excuteTime;
            }
            public bool ExcuteCallBack(params object[] param)
            {
                callBack(param);
                if (excuteTime >= 1)
                {
                    excuteTime--;
                }
                if (excuteTime == 0)
                {
                    return true;
                }
                return false;
                //excuteTime--;
            }
        }

        public class EventGroup
        {
            private string _groupName;
            public string GroupName { get { return _groupName; } }
        }

        /// <summary>
        /// 扩展方法注册事件，只需要继承IMsgSender 接口就行
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msgName"></param>
        /// <param name="callback"></param>
        public static void RegisterLogicMsg(this IMsgRegister self, EventId msgName, VoidDelegate.WithParams callback, int excuteTime = -1)
        {
            if (msgName<0)
            {
                Debug.LogWarning("RegisterMsg:" + ((Object)self).name + " is Null or Empty");
                return;
            }
            if (callback == null)
            {
                Debug.LogWarning("RegisterMsg:" + ((Object)self).name + "  msg:" + msgName + " is Null or Empty");
                return;
            }
            List<LogicMsgHandler> handleList;
            if (!handleDict.TryGetValue(msgName, out handleList))
            {
                handleList = new List<LogicMsgHandler>();
                handleDict.Add(msgName, handleList);
            }

            //考虑是否需要做一个检测,相同的接收和方法不需要再次添加
            foreach (var handler in handleList)
            {
                if (handler.receiver == self && handler.callBack == callback)
                {
                    Debug.LogWarning("RegisterMsg:" + msgName + " already Register");
                    return;
                }
            }
            handleList.Add(new LogicMsgHandler(self, callback, excuteTime));
        }

        public static void SendLogicMsg(this IMsgSender self, EventId msgName, params object[] param)
        {
            if (!excuteState)
            {
                Debug.LogWarning("事件系统停止接收事件了");
                return;
            }
            if (msgName<0)
            {
                Debug.LogWarning(msgName + " is Null or Empty");
                return;
            }

            if (!handleDict.ContainsKey(msgName))
            {
                Debug.LogWarning(msgName + ": is UnRegister");
                return;
            }
            //有考虑事件注册删除的情况只执行一次和永久注册

            var handlers = handleDict[msgName];
            for (int i = 0; i < handlers.Count; i++)
            {
                var handler = handlers[i];
                if (handler.receiver != null)
                {
                    bool isFinsh = handler.ExcuteCallBack(param);
                    //这里方法都是一次性的不能这样
                    if (isFinsh)
                    {
                        //清理已经不进行接受的方法
                        handlers.RemoveAt(i);
                        i = i - 1;
                    }
                }
                else
                {
                    handlers.RemoveAt(i);
                    i = i - 1;
                }
            }
        }

        /// <summary>
        /// 取消所有特定字符的接收事件
        /// </summary>
        /// <param name="msg"></param>
        public static void ClearLogicMsg(EventId msg)
        {
            List<LogicMsgHandler> handleList;
            handleDict.TryGetValue(msg,out handleList);
            if (handleList == null)
            {
                Debug.LogError(msg+"  在事件系统中不存在");
                return;
            }
            handleList.Clear();
            handleDict.Remove(msg);
        }

        /// <summary>
        /// 取消当前注册对象上单个事件
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msg"></param>
        public static void ClearSingleLogicMsg(this IMsgRegister self, EventId msg)
        {
            List<LogicMsgHandler> handlers;
            handleDict.TryGetValue(msg, out handlers);
            if (handlers == null)
            {
                Debug.LogError(msg + "  在事件系统中不存在");
                return;
            }
            for (int i = 0; i < handlers.Count; i++)
            {
                var handler = handlers[i];
                if (handler.receiver ==self)
                {
                    handlers.RemoveAt(i);
                    i = i - 1;
                }
            }
        }

        public static void ClearEvent()
        {
            handleDict.Clear();
        }

        public static void ExcuteState(bool state)
        {
            excuteState = state;
        }
    }

    /// <summary>
    /// 扩展的接受接口
    /// </summary>
    public interface IMsgSender
    {

    }
    /// <summary>
    /// 扩展的注册接口
    /// </summary>
    public interface IMsgRegister
    {

    }
}