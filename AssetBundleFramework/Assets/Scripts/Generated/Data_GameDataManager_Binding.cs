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
    unsafe class Data_GameDataManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Data.GameDataManager);
            args = new Type[]{};
            method = type.GetMethod("Gett_AuthorInfoList", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Gett_AuthorInfoList_0);

            field = type.GetField("Singleton", flag);
            app.RegisterCLRFieldGetter(field, get_Singleton_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Singleton_0, null);


        }


        static StackObject* Gett_AuthorInfoList_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Data.GameDataManager instance_of_this_method = (Data.GameDataManager)typeof(Data.GameDataManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Gett_AuthorInfoList();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_Singleton_0(ref object o)
        {
            return Data.GameDataManager.Singleton;
        }

        static StackObject* CopyToStack_Singleton_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Data.GameDataManager.Singleton;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
