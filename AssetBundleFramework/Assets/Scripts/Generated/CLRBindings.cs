using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UIWindow_Binding.Register(app);
            SceneData_Binding.Register(app);
            System_Diagnostics_Stopwatch_Binding.Register(app);
            Data_GameDataManager_Binding.Register(app);
            System_Collections_Generic_List_1_t_AuthorInfo_Binding.Register(app);
            System_Int32_Binding.Register(app);
            System_String_Binding.Register(app);
            DIYLog_Binding.Register(app);
            t_AuthorInfo_Binding.Register(app);
            System_Object_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            LoadingPanel_Binding.Register(app);
            UnityEngine_UI_Slider_Binding.Register(app);
            UnityEngine_AsyncOperation_Binding.Register(app);
            System_Math_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            System_Single_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            SingletonTemplate_1_AtlasManager_Binding.Register(app);
            AtlasManager_Binding.Register(app);
            LoginPanel_Binding.Register(app);
            SingletonTemplate_1_GameSceneManager_Binding.Register(app);
            GameSceneManager_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
