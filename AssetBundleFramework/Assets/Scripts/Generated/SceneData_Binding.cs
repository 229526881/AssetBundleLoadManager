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
    unsafe class SceneData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::SceneData);

            field = type.GetField("operation", flag);
            app.RegisterCLRFieldGetter(field, get_operation_0);
            app.RegisterCLRFieldSetter(field, set_operation_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_operation_0, AssignFromStack_operation_0);
            field = type.GetField("bgPath", flag);
            app.RegisterCLRFieldGetter(field, get_bgPath_1);
            app.RegisterCLRFieldSetter(field, set_bgPath_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_bgPath_1, AssignFromStack_bgPath_1);


        }



        static object get_operation_0(ref object o)
        {
            return ((global::SceneData)o).operation;
        }

        static StackObject* CopyToStack_operation_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SceneData)o).operation;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_operation_0(ref object o, object v)
        {
            ((global::SceneData)o).operation = (UnityEngine.AsyncOperation)v;
        }

        static StackObject* AssignFromStack_operation_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.AsyncOperation @operation = (UnityEngine.AsyncOperation)typeof(UnityEngine.AsyncOperation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::SceneData)o).operation = @operation;
            return ptr_of_this_method;
        }

        static object get_bgPath_1(ref object o)
        {
            return ((global::SceneData)o).bgPath;
        }

        static StackObject* CopyToStack_bgPath_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SceneData)o).bgPath;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_bgPath_1(ref object o, object v)
        {
            ((global::SceneData)o).bgPath = (System.String)v;
        }

        static StackObject* AssignFromStack_bgPath_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @bgPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::SceneData)o).bgPath = @bgPath;
            return ptr_of_this_method;
        }



    }
}
