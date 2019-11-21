/*
 * Description:             AssetOperationWindow.cs
 * Author:                  TANGHUAN
 * Create Date:             2019//11/21
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Asset������
/// </summary>
public class AssetOperationWindow : EditorWindow
{
    /// <summary>
    /// Asset��������
    /// </summary>
    public enum EAssetOperationType
    {
        Invalide = 1,                               // ��Ч����
        AssetDependencyBrowser,                     // Asset�����ļ��鿴����
        AssetBuildInResourceRefAnalyze,             // Asset������Դ����ͳ������
        AssetBuildInResourceRefExtraction,          // Asset������Դ������ȡ����
        ShaderVariantsCollection,                   // Shader�����Ѽ�����
    }

    /// <summary>
    /// Asset��������
    /// </summary>
    private EAssetOperationType mAssetOperationType = EAssetOperationType.Invalide;

    /// <summary>
    /// ����λ��
    /// </summary>
    private Vector2 uiScrollPos;

    [MenuItem("Tools/Assets/Asset��ش�����", false)]
    public static void dpAssetBrowser()
    {
        var assetoperationwindow = EditorWindow.GetWindow<AssetOperationWindow>();
        assetoperationwindow.Show();
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        includeIndirectDp = GUILayout.Toggle(includeIndirectDp, "�Ƿ�����������");
        GUILayout.Label("��Դ��׺����:");
        postFixFilter = GUILayout.TextField(postFixFilter, GUILayout.MaxWidth(200.0f));
        if (GUILayout.Button("�鿴ѡ��Asset����", GUILayout.MaxWidth(150.0f)))
        {
            mAssetOperationType = EAssetOperationType.AssetDependencyBrowser;
            refreshAssetDepBrowserSelections();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ͳ��������Դ����Asset", GUILayout.MaxWidth(150.0f)))
        {
            mAssetOperationType = EAssetOperationType.AssetBuildInResourceRefAnalyze;
            analyzeBuildInResourceReferenceAsset();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("��ȡѡ�ж���������Դ", GUILayout.MaxWidth(150.0f)))
        {
            mAssetOperationType = EAssetOperationType.AssetBuildInResourceRefExtraction;
            var selectiongo = Selection.activeGameObject;
            if (selectiongo != null)
            {
                extractBuildInResource(selectiongo);
            }
            else
            {
                Debug.Log("����ѡ����Ч��ȡ����,ֻ֧��GameObject!");
            }
        }
        GUILayout.Space(10);
        if (GUILayout.Button("�Ѽ�Shader����", GUILayout.MaxWidth(150.0f)))
        {
            mAssetOperationType = EAssetOperationType.ShaderVariantsCollection;
            collectAllShaderVariants();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(10);
        displayAssetOperationResult();
    }

    /// <summary>
    /// ��ʾ������Դ���ý��
    /// </summary>
    private void displayAssetOperationResult()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Asset��������:" + mAssetOperationType);
        GUILayout.EndHorizontal();
        switch (mAssetOperationType)
        {
            case EAssetOperationType.AssetDependencyBrowser:
                displayAssetDependencyBrowserResult();
                break;
            case EAssetOperationType.AssetBuildInResourceRefAnalyze:
                displayAssetBuildInResourceRefAnalyze();
                break;
            case EAssetOperationType.AssetBuildInResourceRefExtraction:
                displayAssetBuildInResourceRefExtractionResult();
                break;
            case EAssetOperationType.ShaderVariantsCollection:
                displayShaderVariantsCollectionResult();
                break;
            default:
                displayInvalideResult();
                break;
        }
    }

    #region Asset�����ļ��鿴
    /// <summary>
    /// ������Դӳ��map
    /// KeyΪѡ��Asset��·����Value����Asset��·���б�
    /// </summary>
    private Dictionary<string, List<string>> dpAssetInfoMap = new Dictionary<string, List<string>>();

    /// <summary>
    /// �Ƿ�����������
    /// </summary>
    private bool includeIndirectDp = false;

    /// <summary>
    /// ��Դ��׺����
    /// </summary>
    private string postFixFilter = string.Empty;

    /// <summary>
    /// ��ʾAsset������Դ������
    /// </summary>
    private void displayAssetDependencyBrowserResult()
    {
        GUILayout.BeginVertical();
        uiScrollPos = GUILayout.BeginScrollView(uiScrollPos);
        foreach (var dpassetinfo in dpAssetInfoMap)
        {
            showAssetDpUI(dpassetinfo.Key, dpassetinfo.Value);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// ��ʾ������Դ��ϢUI
    /// </summary>
    /// <param name="assetpath"></param>
    /// <param name="dpassetpath"></param>
    private void showAssetDpUI(string assetpath, List<string> dpassetpathlist)
    {
        GUILayout.BeginVertical();
        uiScrollPos = GUILayout.BeginScrollView(uiScrollPos, GUILayout.MaxWidth(2000.0f), GUILayout.MaxHeight(800.0f));
        GUILayout.Label("��Asset·��:");
        GUILayout.Label(assetpath);
        GUILayout.Label("����Asset·��:");
        foreach (var dpassetpath in dpassetpathlist)
        {
            if (postFixFilter.Equals(string.Empty))
            {
                GUILayout.Label(dpassetpath);
            }
            else
            {
                if (dpassetpath.EndsWith(postFixFilter))
                {
                    GUILayout.Label(dpassetpath);
                }
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// ˢ��ѡ��Asset�����������
    /// </summary>
    private void refreshAssetDepBrowserSelections()
    {
        dpAssetInfoMap.Clear();
        var selections = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets);
        foreach (var selection in selections)
        {
            var selectionassetpath = AssetDatabase.GetAssetPath(selection);
            var dpassets = AssetDatabase.GetDependencies(selectionassetpath, includeIndirectDp);
            if (!dpAssetInfoMap.ContainsKey(selectionassetpath))
            {
                dpAssetInfoMap.Add(selectionassetpath, new List<string>());
            }
            foreach (var dpasset in dpassets)
            {
                dpAssetInfoMap[selectionassetpath].Add(dpasset);
            }
        }
    }
    #endregion

    #region Asset������Դ����ͳ��
    /// <summary>
    /// ����������Դ��Assetӳ��map
    /// KeyΪѡ��Asset��·����ValueΪAssetʹ����������Դ�Ľڵ�������Դ�����б�
    /// </summary>
    private Dictionary<string, List<KeyValuePair<string, string>>> mReferenceBuildInResourceAssetMap = new Dictionary<string, List<KeyValuePair<string, string>>>();

    /// <summary>
    /// ��ʾ������Դ����ͳ�ƽ��
    /// </summary>
    private void displayAssetBuildInResourceRefAnalyze()
    {
        GUILayout.BeginVertical();
        uiScrollPos = GUILayout.BeginScrollView(uiScrollPos);
        foreach (var referenceasset in mReferenceBuildInResourceAssetMap)
        {
            showBIResourceReferenceAssetUI(referenceasset.Key, referenceasset.Value);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// ��ʾʹ����������Դ��ϢUI
    /// </summary>
    /// <param name="assetpath"></param>
    /// <param name="biassetinfolist"></param>
    private void showBIResourceReferenceAssetUI(string assetpath, List<KeyValuePair<string, string>> biassetinfolist)
    {
        GUILayout.Label("Asset·��:");
        GUILayout.Label(assetpath);
        if (biassetinfolist.Count > 0)
        {
            GUILayout.Label("������Դʹ����Ϣ:");
            foreach (var biassetinfo in biassetinfolist)
            {
                GUILayout.Label(string.Format("�ڵ���:{0}, ��Դ��:{1}", biassetinfo.Key, biassetinfo.Value));
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    /// <summary>
    /// ͳ��������Դ����Asset
    /// </summary>
    private void analyzeBuildInResourceReferenceAsset()
    {
        //  ��Ҫ������¼������棺
        //  1. Graphic��Texture��Matetial
        //  2. MeshRenderer��Material
        //  3. ParticleSystem��Material
        //  4. Material��Shader
        mReferenceBuildInResourceAssetMap.Clear();
        var allmatfiles = Directory.GetFiles("Assets/", "*.mat", SearchOption.AllDirectories);
        var assetinfolist = new List<KeyValuePair<string, string>>();
        foreach (var matfile in allmatfiles)
        {
            var matasset = AssetDatabase.LoadAssetAtPath<Material>(matfile);
            if (isUsingBuildInResource<Material>(matasset, ref assetinfolist))
            {
                tryAddBuildInShaderReferenceAsset(matfile, assetinfolist);
            }
        }
        var allprefabfiles = Directory.GetFiles("Assets/", "*.prefab", SearchOption.AllDirectories);
        foreach (var prefabfile in allprefabfiles)
        {
            var prefabasset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabfile);
            if (isUsingBuildInResource<GameObject>(prefabasset, ref assetinfolist))
            {
                tryAddBuildInShaderReferenceAsset(prefabfile, assetinfolist);
            }
        }
    }

    /// <summary>
    /// ָ�������Ƿ�ʹ����������Դ
    /// </summary>
    /// <param name="asset"></param>
    /// <param name="assetinfolist"></param>
    /// <returns></returns>
    private bool isUsingBuildInResource<T>(T asset, ref List<KeyValuePair<string, string>> assetinfolist) where T : UnityEngine.Object
    {
        assetinfolist.Clear();
        if (typeof(T) == typeof(Material))
        {
            var mat = asset as Material;
            if (mat.shader != null && EditorResourceUtilities.isBuildInResource(mat.shader))
            {
                assetinfolist.Add(new KeyValuePair<string, string>("��", mat.shader.name));
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (typeof(T) == typeof(GameObject))
        {
            var prefab = asset as GameObject;
            // ����ģ�ͺ�������Ч
            var allrenders = prefab.GetComponentsInChildren<Renderer>();
            var isusingbuildinresource = false;
            foreach (var render in allrenders)
            {
                //�õ�������asset��(����ƴ�յķ�ʽ e.g. Default.mat Sprite.png)
                var usedassetname = string.Empty;
                if (render.sharedMaterial != null)
                {
                    if (EditorResourceUtilities.isBuildInResource(render.sharedMaterial))
                    {
                        usedassetname += render.sharedMaterial.name;
                        isusingbuildinresource = true;
                    }
                    if (render.sharedMaterial.shader != null)
                    {
                        if (EditorResourceUtilities.isBuildInResource(render.sharedMaterial.shader))
                        {
                            usedassetname += " " + render.sharedMaterial.shader.name;
                            isusingbuildinresource = true;
                        }
                    }
                    if (!usedassetname.IsNullOrEmpty())
                    {
                        assetinfolist.Add(new KeyValuePair<string, string>(render.transform.name, usedassetname));
                    }
                }
            }
            // UI���
            var allgraphics = prefab.GetComponentsInChildren<Graphic>();
            foreach (var graphic in allgraphics)
            {
                //�õ�������asset��(����ƴ�յķ�ʽ e.g. Default.mat Sprite.png)
                var usedassetname = string.Empty;
                if (graphic.material != null)
                {
                    if (EditorResourceUtilities.isBuildInResource(graphic.material))
                    {
                        usedassetname += graphic.material.name;
                        isusingbuildinresource = true;
                    }
                    if (graphic.material.shader != null)
                    {
                        if (EditorResourceUtilities.isBuildInResource(graphic.material.shader))
                        {
                            usedassetname += " " + graphic.material.shader.name;
                            isusingbuildinresource = true;
                        }
                    }
                }
                if (graphic.mainTexture != null)
                {
                    if (EditorResourceUtilities.isBuildInResource(graphic.mainTexture))
                    {
                        usedassetname += " " + graphic.mainTexture.name;
                        isusingbuildinresource = true;
                    }
                }
                if (!usedassetname.IsNullOrEmpty())
                {
                    assetinfolist.Add(new KeyValuePair<string, string>(graphic.transform.name, usedassetname));
                }
            }
            return isusingbuildinresource;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// �������Asset������Shader��������Ϣ
    /// </summary>
    /// <param name="assetpath"></param>
    /// <param name="assetinfolist"></param>
    /// <returns></returns>
    private void tryAddBuildInShaderReferenceAsset(string assetpath, List<KeyValuePair<string, string>> assetinfolist = null)
    {
        if (!mReferenceBuildInResourceAssetMap.ContainsKey(assetpath))
        {
            mReferenceBuildInResourceAssetMap.Add(assetpath, new List<KeyValuePair<string, string>>());
        }
        foreach (var assetinfo in assetinfolist)
        {
            mReferenceBuildInResourceAssetMap[assetpath].Add(assetinfo);
        }
    }
    #endregion

    #region Asset������Դ������ȡ
    /// <summary>
    /// ���ò�����ȡ������Ŀ¼
    /// </summary>
    private const string mBuildInMaterialExtractRelativeOutputFolderPath = "/Res/buildinresources/buildinmaterials/";

    /// <summary>
    /// ���ò�����ȡ������Ŀ¼
    /// </summary>
    private const string mBuildInTextureExtractRelativeOutputFolderPath = "/Res/buildinresources/buildintextures/";

    /// <summary>
    /// ������Դ��ȡ���
    /// </summary>
    private bool mBuildInResourceExtractionResult;

    /// <summary>
    /// ������Դ��ȡ����б�
    /// </summary>
    private List<string> mBuildInResourceExtractedList = new List<string>();

    /// <summary>
    /// Asset������Դ��ȡ���
    /// </summary>
    private void displayAssetBuildInResourceRefExtractionResult()
    {
        GUILayout.BeginVertical();
        uiScrollPos = GUILayout.BeginScrollView(uiScrollPos);
        GUILayout.Label(string.Format("������Դ��ȡ{0}!", mBuildInResourceExtractionResult == false ? "������" : "���"));
        GUILayout.Label("������Դ��ȡ����б�:");
        foreach (var extractedres in mBuildInResourceExtractedList)
        {
            GUILayout.Label(extractedres);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// ��ȡ������Դ
    /// </summary>
    /// <param name="asset"></param>
    private void extractBuildInResource(UnityEngine.GameObject asset)
    {
        mBuildInResourceExtractionResult = false;
        var materialextractoutputfolderpath = Application.dataPath + mBuildInMaterialExtractRelativeOutputFolderPath;
        var textureextractoutputfolderpath = Application.dataPath + mBuildInTextureExtractRelativeOutputFolderPath;
        Debug.Log("materialextractoutputfolderpath = " + materialextractoutputfolderpath);
        Debug.Log("textureextractoutputfolderpath = " + textureextractoutputfolderpath);
        Utilities.CheckAndCreateSpecificFolder(materialextractoutputfolderpath);
        Utilities.CheckAndCreateSpecificFolder(textureextractoutputfolderpath);
        var referencebuildinobjectlist = getReferenceBuildInResourceExcludeShader(asset);
        referencebuildinobjectlist = referencebuildinobjectlist.Distinct().ToList();
        Debug.Log(string.Format("����������Դ����:{0}", referencebuildinobjectlist.Count));
        mBuildInResourceExtractedList.Clear();
        foreach (var buildinobject in referencebuildinobjectlist)
        {
            if (buildinobject is Material mt)
            {
                Material mat = GameObject.Instantiate<Material>(mt);
                var outputfolderpath = "Assets" + mBuildInMaterialExtractRelativeOutputFolderPath + buildinobject.name + ".mat";
                Debug.Log(string.Format("����������·��:{0}", outputfolderpath));
                AssetDatabase.CreateAsset(mat, outputfolderpath);
                mBuildInResourceExtractedList.Add(outputfolderpath);
            }
            else if (buildinobject is Texture)
            {
                var texturepreview = AssetPreview.GetAssetPreview(buildinobject);
                var outputfolderpath = "Assets" + mBuildInTextureExtractRelativeOutputFolderPath + buildinobject.name + ".png";
                Debug.Log(string.Format("����������·��:{0}", outputfolderpath));
                File.WriteAllBytes(outputfolderpath, texturepreview.EncodeToPNG());
                mBuildInResourceExtractedList.Add(outputfolderpath);
            }
            else
            {
                Debug.LogError(string.Format("��֧�ֵ�������Դ����:{0}", buildinobject.GetType()));
            }
        }
        mBuildInResourceExtractionResult = true;
    }

    /// <summary>
    /// ��ȡ��Shader��������Դ����
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    private List<UnityEngine.Object> getReferenceBuildInResourceExcludeShader(UnityEngine.GameObject asset)
    {
        // ��Ҫ�������¼�����Դ:
        // 1. ����Texture
        // 2. ���ò���
        // ����ģ�ͺ�������Ч
        var assetlist = new List<UnityEngine.Object>();
        var allrenders = asset.GetComponentsInChildren<Renderer>();
        foreach (var render in allrenders)
        {
            if (render.sharedMaterial != null && EditorResourceUtilities.isBuildInResource(render.sharedMaterial))
            {
                assetlist.Add(render.sharedMaterial);
            }
        }
        // UI���
        var allgraphics = asset.GetComponentsInChildren<Graphic>();
        foreach (var graphic in allgraphics)
        {
            if (graphic.material != null && EditorResourceUtilities.isBuildInResource(graphic.material))
            {
                assetlist.Add(graphic.material);
            }
            if (graphic.mainTexture != null && EditorResourceUtilities.isBuildInResource(graphic.mainTexture))
            {
                assetlist.Add(graphic.mainTexture);
            }
        }
        return assetlist;
    }
    #endregion

    #region Shader�����Ѽ�
    /// <summary>
    /// Shader�����Ѽ���Ŀ¼
    /// </summary>
    private string ShaderCollectRootFolderPath;

    /// <summary>
    /// Shader�����Ѽ��ļ����Ŀ¼
    /// </summary>
    private string ShaderVariantOuputFolderPath;

    /// <summary>
    /// �����Ѽ�Cube���ڵ�(����ͳһɾ��)
    /// </summary>
    private GameObject SVCCubeParentGo;

    /// <summary>
    /// Shader�����Ѽ����
    /// </summary>
    private bool mShaderVariantCollectionResult;

    /// <summary>
    /// ��Ҫ�Ѽ��Ĳ���Asset·���б�
    /// </summary>
    private List<string> mMaterialNeedCollectedList = new List<string>();

    /// <summary>
    /// ��ʾ�����Ѽ����
    /// </summary>
    private void displayShaderVariantsCollectionResult()
    {
        GUILayout.BeginVertical();
        uiScrollPos = GUILayout.BeginScrollView(uiScrollPos);
        GUILayout.Label(string.Format("Shader�����Ѽ�{0}!", mShaderVariantCollectionResult == false ? "������" : "���"));
        GUILayout.Label("��Ҫ�Ѽ��Ĳ�����Դ·��:");
        foreach (var matcollectedpath in mMaterialNeedCollectedList)
        {
            GUILayout.Label(matcollectedpath);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// �Ѽ����е�Shader����
    /// </summary>
    private async void collectAllShaderVariants()
    {
        mShaderVariantCollectionResult = false;
        ShaderCollectRootFolderPath = Application.dataPath;
        ShaderVariantOuputFolderPath = Application.dataPath + "/Res/shadervariants";
        var preactivescene = EditorSceneManager.GetActiveScene();
        var preactivescenepath = preactivescene.path;
        Debug.Log(string.Format("֮ǰ�򿪵ĳ�����Դ·��:{0}", preactivescenepath));
        // Shader�����Ѽ�����
        // 1. ��Shader�����Ѽ�����
        // 2. ���Shader�����Ѽ�����
        // 3. ���Ŵ���ʹ��ÿһ����Ч���ʵ�Cube��Ⱦһ֡
        // 4. ���������Ѽ�����������Ѽ��ļ�
        Debug.Log("��ʼ�Ѽ�Shader����!");
        await openShaderVariantsCollectSceneAsync();
        await clearAllShaderVariantsAsync();
        await createAllValideMaterialCudeAsync();
        await doShaderVariantsCollectAsync();
        Debug.Log("�����Ѽ�Shader����!");
        // ��֮ǰ�ĳ���
        EditorSceneManager.OpenScene(preactivescenepath);
        mShaderVariantCollectionResult = true;
    }

    /// <summary>
    /// ��Shader�����Ѽ�����
    /// </summary>
    private async Task openShaderVariantsCollectSceneAsync()
    {
        Debug.Log("openShaderVariantsCollectScene()");
        EditorSceneManager.OpenScene("Assets/Res/scenes/ShaderVariantsCollectScene.unity");
        Debug.Log("��Shader�����ռ�����!");
        await Task.Delay(1000);
    }

    /// <summary>
    /// ���Shader��������
    /// </summary>
    private async Task clearAllShaderVariantsAsync()
    {
        Debug.Log("clearAllShaderVariants()");
        MethodInfo clearcurrentsvc = typeof(ShaderUtil).GetMethod("ClearCurrentShaderVariantCollection", BindingFlags.NonPublic | BindingFlags.Static);
        clearcurrentsvc.Invoke(null, null);
        Debug.Log("���Shader��������!");
        await Task.Delay(1000);
    }

    /// <summary>
    /// ����������Ч���ʵĶ�ӦCube
    /// </summary>
    private async Task createAllValideMaterialCudeAsync()
    {
        Debug.Log("createAllValideMaterialCude()");
        SVCCubeParentGo = new GameObject("SVCCubeParentGo");
        SVCCubeParentGo.transform.position = Vector3.zero;
        var posoffset = new Vector3(0.05f, 0f, 0f);
        var cubeworldpos = Vector3.zero;
        var svccubetemplate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Res/prefabs/pre_SVCCube.prefab");
        var allmatassets = getAllValideMaterial();
        mMaterialNeedCollectedList.Clear();
        Debug.Log(string.Format("��Ҫ�Ѽ��Ĳ�������:{0}", allmatassets));
        for (int i = 0, length = allmatassets.Count; i < length; i++)
        {
            var matassetpath = AssetDatabase.GetAssetPath(allmatassets[i]);
            mMaterialNeedCollectedList.Add(matassetpath);
            var cube = GameObject.Instantiate<GameObject>(svccubetemplate);
            cube.name = allmatassets[i].name + "Cube";
            cube.transform.position = posoffset * i;
            cube.GetComponent<MeshRenderer>().material = allmatassets[i];
            cube.transform.SetParent(SVCCubeParentGo.transform);
        }
        EditorSceneManager.SaveOpenScenes();
        //��ʱ�ȴ�һ�ᣬȷ���������ݸ���
        Debug.Log("������Cube����ʼ�ȴ�5��!");
        await Task.Delay(5000);
        Debug.Log("������Cube���ȴ�5�����!");
    }

    /// <summary>
    /// ִ�б����Ѽ�
    /// </summary>
    private async Task doShaderVariantsCollectAsync()
    {
        Debug.Log("doShaderVariantsCollect()");
        ShaderVariantOuputFolderPath = Application.dataPath + "/Res/shadervariants/";
        var outputassetsindex = ShaderVariantOuputFolderPath.IndexOf("Assets");
        var outputrelativepath = ShaderVariantOuputFolderPath.Substring(outputassetsindex, ShaderVariantOuputFolderPath.Length - outputassetsindex);
        var svcoutputfilepath = outputrelativepath + "DIYShaderVariantsCollection.shadervariants";
        Debug.Log(string.Format("Shader�����ļ����Ŀ¼:{0}", ShaderVariantOuputFolderPath));
        Debug.Log(string.Format("Shader�����ļ�������·��:{0}", svcoutputfilepath));
        if (!Directory.Exists(ShaderVariantOuputFolderPath))
        {
            Debug.Log(string.Format("Shader�����ļ����Ŀ¼:{0}�����ڣ����´���һ��!", ShaderVariantOuputFolderPath));
            Directory.CreateDirectory(ShaderVariantOuputFolderPath);
        }
        EditorSceneManager.SaveOpenScenes();
        MethodInfo savecurrentsvc = typeof(ShaderUtil).GetMethod("SaveCurrentShaderVariantCollection", BindingFlags.NonPublic | BindingFlags.Static);
        savecurrentsvc.Invoke(null, new object[] { svcoutputfilepath });
        // ֱ������AB���ֺ�Shader�����һ��
        var svcassetimporter = AssetImporter.GetAtPath(svcoutputfilepath);
        if (svcassetimporter != null)
        {
            svcassetimporter.assetBundleName = "shaderlist";
            DIYLog.Log(string.Format("������Դ:{0}��AB����Ϊ:shaderlist", svcoutputfilepath));
            AssetDatabase.SaveAssets();
        }
        GameObject.DestroyImmediate(SVCCubeParentGo);
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("������Shader�����ļ�!");
        await Task.Delay(1000);
    }

    /// <summary>
    /// ��ȡ������Ч����(��Ч��ָ��ʹ��Shader)
    /// </summary>
    /// <returns></returns>
    private List<Material> getAllValideMaterial()
    {
        ShaderCollectRootFolderPath = Application.dataPath;
        var assetsindex = ShaderCollectRootFolderPath.IndexOf("Assets");
        var collectrelativepath = ShaderCollectRootFolderPath.Substring(assetsindex, ShaderCollectRootFolderPath.Length - assetsindex);
        var assets = AssetDatabase.FindAssets("t:Prefab", new string[] { collectrelativepath }).ToList();
        var assets2 = AssetDatabase.FindAssets("t:Material", new string[] { collectrelativepath });
        assets.AddRange(assets2);

        List<Material> allmatassets = new List<Material>();
        List<string> allmats = new List<string>();

        //GUID to assetPath
        for (int i = 0; i < assets.Count; i++)
        {
            var p = AssetDatabase.GUIDToAssetPath(assets[i]);
            //��ȡ�����е�mat
            var dependenciesPath = AssetDatabase.GetDependencies(p, true);

            var mats = dependenciesPath.ToList().FindAll((dp) => dp.EndsWith(".mat"));
            allmats.AddRange(mats);
        }

        //�������е� material
        allmats = allmats.Distinct().ToList();

        foreach (var mat in allmats)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(mat);
            if (obj is Material)
            {
                allmatassets.Add(obj as Material);
            }
        }
        return allmatassets;
    }
    #endregion

    #region Ĭ����Ч����
    /// <summary>
    /// ��ʾ��Ч���ͽ��
    /// </summary>
    private void displayInvalideResult()
    {
        GUILayout.BeginVertical();
        uiScrollPos = GUILayout.BeginScrollView(uiScrollPos);
        GUILayout.Label("û����Ч����!");
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
    #endregion
}