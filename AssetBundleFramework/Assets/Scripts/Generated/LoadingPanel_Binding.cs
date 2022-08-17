using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class LoadingPanel_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::LoadingPanel);

            field = type.GetField("m_IminTime", flag);
            app.RegisterCLRFieldGetter(field, get_m_IminTime_0);
            app.RegisterCLRFieldSetter(field, set_m_IminTime_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_IminTime_0, AssignFromStack_m_IminTime_0);
            field = type.GetField("m_SliderExp", flag);
            app.RegisterCLRFieldGetter(field, get_m_SliderExp_1);
            app.RegisterCLRFieldSetter(field, set_m_SliderExp_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_SliderExp_1, AssignFromStack_m_SliderExp_1);
            field = type.GetField("m_TxtExp", flag);
            app.RegisterCLRFieldGetter(field, get_m_TxtExp_2);
            app.RegisterCLRFieldSetter(field, set_m_TxtExp_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_TxtExp_2, AssignFromStack_m_TxtExp_2);
            field = type.GetField("m_ImgBg", flag);
            app.RegisterCLRFieldGetter(field, get_m_ImgBg_3);
            app.RegisterCLRFieldSetter(field, set_m_ImgBg_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_ImgBg_3, AssignFromStack_m_ImgBg_3);


        }



        static object get_m_IminTime_0(ref object o)
        {
            return ((global::LoadingPanel)o).m_IminTime;
        }

        static StackObject* CopyToStack_m_IminTime_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LoadingPanel)o).m_IminTime;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m_IminTime_0(ref object o, object v)
        {
            ((global::LoadingPanel)o).m_IminTime = (System.Int32)v;
        }

        static StackObject* AssignFromStack_m_IminTime_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @m_IminTime = ptr_of_this_method->Value;
            ((global::LoadingPanel)o).m_IminTime = @m_IminTime;
            return ptr_of_this_method;
        }

        static object get_m_SliderExp_1(ref object o)
        {
            return ((global::LoadingPanel)o).m_SliderExp;
        }

        static StackObject* CopyToStack_m_SliderExp_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LoadingPanel)o).m_SliderExp;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_SliderExp_1(ref object o, object v)
        {
            ((global::LoadingPanel)o).m_SliderExp = (UnityEngine.UI.Slider)v;
        }

        static StackObject* AssignFromStack_m_SliderExp_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.Slider @m_SliderExp = (UnityEngine.UI.Slider)typeof(UnityEngine.UI.Slider).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::LoadingPanel)o).m_SliderExp = @m_SliderExp;
            return ptr_of_this_method;
        }

        static object get_m_TxtExp_2(ref object o)
        {
            return ((global::LoadingPanel)o).m_TxtExp;
        }

        static StackObject* CopyToStack_m_TxtExp_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LoadingPanel)o).m_TxtExp;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_TxtExp_2(ref object o, object v)
        {
            ((global::LoadingPanel)o).m_TxtExp = (UnityEngine.UI.Text)v;
        }

        static StackObject* AssignFromStack_m_TxtExp_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.Text @m_TxtExp = (UnityEngine.UI.Text)typeof(UnityEngine.UI.Text).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::LoadingPanel)o).m_TxtExp = @m_TxtExp;
            return ptr_of_this_method;
        }

        static object get_m_ImgBg_3(ref object o)
        {
            return ((global::LoadingPanel)o).m_ImgBg;
        }

        static StackObject* CopyToStack_m_ImgBg_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LoadingPanel)o).m_ImgBg;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_ImgBg_3(ref object o, object v)
        {
            ((global::LoadingPanel)o).m_ImgBg = (TUI.TImage)v;
        }

        static StackObject* AssignFromStack_m_ImgBg_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            TUI.TImage @m_ImgBg = (TUI.TImage)typeof(TUI.TImage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::LoadingPanel)o).m_ImgBg = @m_ImgBg;
            return ptr_of_this_method;
        }



    }
}
