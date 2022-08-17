using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace ILRuntimeDemo
{   
    public class UIWindowAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(global::UIWindow);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : global::UIWindow, CrossBindingAdaptorType
        {
            CrossBindingFunctionInfo<global::UIMsgID, System.Object[], System.Boolean> mOnMessage_0 = new CrossBindingFunctionInfo<global::UIMsgID, System.Object[], System.Boolean>("OnMessage");
            CrossBindingMethodInfo<System.String, SFramework.UIType, System.Boolean, System.Object> mInit_1 = new CrossBindingMethodInfo<System.String, SFramework.UIType, System.Boolean, System.Object>("Init");
            CrossBindingMethodInfo mAwake_2 = new CrossBindingMethodInfo("Awake");
            CrossBindingMethodInfo mOnShow_3 = new CrossBindingMethodInfo("OnShow");
            CrossBindingMethodInfo<SFramework.UIType> mOnTop_4 = new CrossBindingMethodInfo<SFramework.UIType>("OnTop");
            CrossBindingMethodInfo mOnOpenEnd_5 = new CrossBindingMethodInfo("OnOpenEnd");
            CrossBindingMethodInfo mOnDisable_6 = new CrossBindingMethodInfo("OnDisable");
            CrossBindingMethodInfo mOnUpdate_7 = new CrossBindingMethodInfo("OnUpdate");
            CrossBindingMethodInfo mOnClose_8 = new CrossBindingMethodInfo("OnClose");

            bool isInvokingToString;
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override System.Boolean OnMessage(global::UIMsgID msgID, System.Object[] paralist)
            {
                if (mOnMessage_0.CheckShouldInvokeBase(this.instance))
                    return base.OnMessage(msgID, paralist);
                else
                    return mOnMessage_0.Invoke(this.instance, msgID, paralist);
            }

            public override void Init(System.String name, SFramework.UIType type, System.Boolean resources, System.Object uiArgs)
            {
                if (mInit_1.CheckShouldInvokeBase(this.instance))
                    base.Init(name, type, resources, uiArgs);
                else
                    mInit_1.Invoke(this.instance, name, type, resources, uiArgs);
            }

            public override void Awake()
            {
                if (mAwake_2.CheckShouldInvokeBase(this.instance))
                    base.Awake();
                else
                    mAwake_2.Invoke(this.instance);
            }

            public override void OnShow()
            {
                if (mOnShow_3.CheckShouldInvokeBase(this.instance))
                    base.OnShow();
                else 
                    mOnShow_3.Invoke(this.instance);
            }

            public override void OnTop(SFramework.UIType type)
            {
                if (mOnTop_4.CheckShouldInvokeBase(this.instance))
                    base.OnTop(type);
                else
                    mOnTop_4.Invoke(this.instance, type);
            }

            public override void OnOpenEnd()
            {
                if (mOnOpenEnd_5.CheckShouldInvokeBase(this.instance))
                    base.OnOpenEnd();
                else
                    mOnOpenEnd_5.Invoke(this.instance);
            }

            public override void OnDisable()
            {
                if (mOnDisable_6.CheckShouldInvokeBase(this.instance))
                    base.OnDisable();
                else
                    mOnDisable_6.Invoke(this.instance);
            }

            public override void OnUpdate()
            {
                if (mOnUpdate_7.CheckShouldInvokeBase(this.instance))
                    base.OnUpdate();
                else
                    mOnUpdate_7.Invoke(this.instance);
            }

            public override void OnClose()
            {
                if (mOnClose_8.CheckShouldInvokeBase(this.instance))
                    base.OnClose();
                else
                    mOnClose_8.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    if (!isInvokingToString)
                    {
                        isInvokingToString = true;
                        string res = instance.ToString();
                        isInvokingToString = false;
                        return res;
                    }
                    else
                        return instance.Type.FullName;
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

