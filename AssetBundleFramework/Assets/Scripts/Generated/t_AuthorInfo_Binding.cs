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
    unsafe class t_AuthorInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::t_AuthorInfo);

            field = type.GetField("Id", flag);
            app.RegisterCLRFieldGetter(field, get_Id_0);
            app.RegisterCLRFieldSetter(field, set_Id_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Id_0, AssignFromStack_Id_0);
            field = type.GetField("age", flag);
            app.RegisterCLRFieldGetter(field, get_age_1);
            app.RegisterCLRFieldSetter(field, set_age_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_age_1, AssignFromStack_age_1);


        }



        static object get_Id_0(ref object o)
        {
            return ((global::t_AuthorInfo)o).Id;
        }

        static StackObject* CopyToStack_Id_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::t_AuthorInfo)o).Id;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Id_0(ref object o, object v)
        {
            ((global::t_AuthorInfo)o).Id = (System.Int32)v;
        }

        static StackObject* AssignFromStack_Id_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @Id = ptr_of_this_method->Value;
            ((global::t_AuthorInfo)o).Id = @Id;
            return ptr_of_this_method;
        }

        static object get_age_1(ref object o)
        {
            return ((global::t_AuthorInfo)o).age;
        }

        static StackObject* CopyToStack_age_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::t_AuthorInfo)o).age;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_age_1(ref object o, object v)
        {
            ((global::t_AuthorInfo)o).age = (System.Int32)v;
        }

        static StackObject* AssignFromStack_age_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @age = ptr_of_this_method->Value;
            ((global::t_AuthorInfo)o).age = @age;
            return ptr_of_this_method;
        }



    }
}
