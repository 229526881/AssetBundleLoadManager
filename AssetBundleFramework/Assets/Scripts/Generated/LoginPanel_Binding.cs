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
    unsafe class LoginPanel_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::LoginPanel);

            field = type.GetField("m_BtnLogin", flag);
            app.RegisterCLRFieldGetter(field, get_m_BtnLogin_0);
            app.RegisterCLRFieldSetter(field, set_m_BtnLogin_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_BtnLogin_0, AssignFromStack_m_BtnLogin_0);


        }



        static object get_m_BtnLogin_0(ref object o)
        {
            return ((global::LoginPanel)o).m_BtnLogin;
        }

        static StackObject* CopyToStack_m_BtnLogin_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LoginPanel)o).m_BtnLogin;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_BtnLogin_0(ref object o, object v)
        {
            ((global::LoginPanel)o).m_BtnLogin = (TUI.TButton)v;
        }

        static StackObject* AssignFromStack_m_BtnLogin_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            TUI.TButton @m_BtnLogin = (TUI.TButton)typeof(TUI.TButton).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::LoginPanel)o).m_BtnLogin = @m_BtnLogin;
            return ptr_of_this_method;
        }



    }
}
