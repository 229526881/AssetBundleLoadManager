/*
 * Description:             LoaderManager.cs
 * Author:                  TONYTANG
 * Create Date:             2021/10/13
 */

/// <summary>
/// LoaderManager.cs
/// ����������������
/// </summary>
public class LoaderManager : SingletonTemplate<LoaderManager>
{
    /// <summary>
    /// ��һ����Ч����UID
    /// </summary>
    private int mNextRequestUID;

    /// <summary>
    /// AssetBundle����UID Map<����UID,AssetBundle·��>
    /// </summary>
    private Dictionary<int, string> mAssetBundleRequestUIDMap;

    public LoaderManager()
    {
        mNextRequestUID = 1;
        mAssetBundleRequestUIDMap = new Dictionary<int, string>();
    }

    /// <summary>
    /// ��ȡ��һ����Ч����UID
    /// </summary>
    /// <returns></returns>
    public int GetNextRequestUID()
    {
        return mNextRequestUID++;
    }
    
    /// <summary>
    /// ���ָ��AssetBundle·�������UID
    /// </summary>
    /// <param name="requestUID"></param>
    /// <param name="assetBundlePath"></param>
    /// <returns></returns>
    public bool addAssetBundleRequestUID(int requestUID, string assetBundlePath)
    {
        if(!mAssetBundleRequestUIDMap.ContainKey(requestUID))
        {
            mAssetBundleRequestUIDMap.Add(requestUID, assetBundlePath);
            return true;
        }
        else
        {
            Debug.LogError($"�ظ������ͬ����UID:{requestUID},���AssetBundle:{assetBundlePath}����ʧ��!");
            return false;
        }
    }

    /// <summary>
    /// �Ƴ�ָ������UID��AssetBundle����
    /// </summary>
    /// <param name="requestUID"></param>
    /// <returns></returns>
    public bool removeAssetBundleRequestUID(int requestUID)
    {
        if(mAssetBundleRequestUIDMap.Remove(requestUID))
        {
            return true;
        }
        else
        {
            Debug.LogError($"δ���AssetBundle����UID:{requestUID},�Ƴ�AssetBundle����UIDʧ��!");
            return false;
        }
    }

    /// <summary>
    /// ȡ��ָ������UID��AssetBundle����
    /// </summary>
    /// <param name="requestUID"></param>
    /// <returns></returns>
    public bool cancelAssetBundleRequest(int requestUID)
    {
        string assetBundlePath = getAssetBundleByRequestUID(requestUID);
        if(string.IsNullOrEmpty(assetBundlePath))
        {
            Debug.LogError($"δ�ҵ�����UID:{requestUID}��AssetBundle��������,ȡ������ʧ��!")
            return false;
        }
        var assetBundleLoader = getAssetBundleLoader(assetBundlePath);
        return assetBundleLoader != null ? assetBundleLoader.cancelRequest(requestUID) : false;
    }

    /// <summary>
    /// ��ȡָ��AssetBundle·���ļ�����
    /// </summary>
    /// <param name="assetBundlePath"></param>
    /// <returns></returns>
    public AssetBundleLoader getAssetBundleLoader(string assetBundlePath)
    {
        AssetBundleLoader abLoader;
        //if(AllAssetBundleLoaderMap.TryGetValue(assetBundlePath, out abLoader))
        //{
        //    return abLoader;
        //}
        return null;
    }

    /// <summary>
    /// ��ȡָ������UID��AssetBundle·��
    /// </summary>
    /// <param name="requestUID"></param>
    /// <returns></returns>
    private string getAssetBundleByRequestUID(int requestUID)
    {
        string assetBundlePath;
        if(mAssetBundleRequestUIDMap.TryGetValue(requestUID, out assetBundlePath))
        {
            return assetBundlePath;
        }
        else
        {
            Debug.LogError($"AssetBundleδ�������UID:{requestUID}������,��ȡAssetBundle·��ʧ��!");
            return null;
        }
    }
}