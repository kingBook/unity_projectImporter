using UnityEngine;

[CreateAssetMenu(fileName = "UntitledInstaller", menuName = "MyAsset/UntitledInstaller")]//添加这个特性就能在资源窗口右键创建资源
public class UntitledInstaller : ScriptableObject
{
    public string name;
    public string age;

    public TestClass tc;
}

[System.Serializable]//标记可序列化 要不然在Inspector 面板看不到这个字段
public class TestClass
{
    public int TestInt;
}