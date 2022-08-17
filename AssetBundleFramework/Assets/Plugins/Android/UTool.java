package com.Unity.Tools;

import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.util.Log;

import androidx.core.content.FileProvider;

import com.unity3d.player.UnityPlayer;
import android.util.Log;


import java.io.File;

public class UTool {
    public   boolean installApk(String apkPath){
        File apkFile = new File(apkPath);
        if (apkFile.exists()) {
            Intent intent = new Intent(Intent.ACTION_VIEW);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                intent.setFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
                Uri contentUri = FileProvider.getUriForFile(UnityPlayer.currentActivity, UnityPlayer.currentActivity.getPackageName()+".fileprovider", apkFile);
                intent.setDataAndType(contentUri, "application/vnd.android.package-archive");
            } else {
                intent.setDataAndType(Uri.fromFile(apkFile), "application/vnd.android.package-archive");
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            }
            UnityPlayer.currentActivity.startActivity(intent);
            return true;
        } else {
            Log.d("TAG", "文件不存在"+apkPath);
            return false;
        }
    }

    public final static String mUnityCallBackHandler = "GameStart";

    //Java层Log的Tag
    private final static String LogTag = "JNI";

    public static void d(String msg)
    {
        Log.d(LogTag, msg);
        UnityPlayer.UnitySendMessage(mUnityCallBackHandler,"resJavaLog", msg);
    }

    public static void i(String msg)
    {
        Log.i(LogTag, msg);
        UnityPlayer.UnitySendMessage(mUnityCallBackHandler,"resJavaLog", msg);
    }

    public static void e(String msg)
    {
        Log.e(LogTag, msg);
        UnityPlayer.UnitySendMessage(mUnityCallBackHandler,"resJavaLog", msg);
    }
}

// v4的方法 需要更换组件使用
 
// import android.content.Context;
// import android.content.Intent;
// import android.net.Uri;
// import android.os.Build;
// import android.support.v4.content.FileProvider;
// import android.widget.Toast;
// import java.io.File;
 
// public  class UTool {
//     private static UTool _instance;
//     public static UTool instance()
//     {
//         if(null == _instance)
//             _instance = new UTool();
//         return _instance;
//     }
//     private Context context;
 
//     public  void Init(Context context)
//     {
//         Toast.makeText(context, "init", Toast.LENGTH_LONG).show();
//         this.context = context;
//     }

   
//     public  void installApk(String apkFullPath)
//     {
//           System.out.println("SJJ"+apkFullPath);
//         try
//         {
//             File file = new File(apkFullPath);
//             if (null == file){
//                 return;
//             }
//             if (!file.exists()){
//                 return;
//             }
//             Intent intent = new Intent(Intent.ACTION_VIEW);
 
//             Uri apkUri =null;
//             if(Build.VERSION.SDK_INT>=24)
//             {
//                 intent.setFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
//                 intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
//                 apkUri = FileProvider.getUriForFile(context, context.getPackageName()+".fileprovider", file);
//                 //intent.setDataAndType(apkUri, "application/vnd.android.package-archive");
//                 System.out.println("SJJ  24+ ");

//             }else{
//                 apkUri = Uri.fromFile(file);
//                 intent.setFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
//                 intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
//              System.out.println("SJJ  24- ");

//             }
//             intent.setDataAndType(apkUri, "application/vnd.android.package-archive");
//             Toast.makeText(context, apkUri.getPath(), Toast.LENGTH_LONG).show();
//             context.startActivity(intent);
//             System.out.println("SJJ"+apkUri.getPath());
//             System.out.println("SJJ  resp sucess "+"   "+apkFullPath);
//         }
//         catch (Exception e)
//         {

//             System.out.println("SJJ  resp fail "+e.getMessage());
//             Toast.makeText(context, e.getMessage(), Toast.LENGTH_LONG).show();
//             e.printStackTrace();
//         }
//     }


// }
 
