https://stackoverflow.com/questions/7898310/using-regex-to-balance-match-parenthesis


You are trying to import an asset which contains a global game manager. This is not allowed.
UnityEditor.AssetDatabase:Refresh()


\((?:[^()]|(?<open> \( )|(?<-open> \) ))+(?(open)(?!))\)
", RegexOptions.IgnorePatternWhitespace);







将其它unity项目导入到当前项目

实现的项目设置(Project Settings)：
Physics
Physics2D
Quality
Tags and Layers
Time
-------------------------
EditorBuildSettings
-------------------------


待完成：
-------------导入器--------------
打开子项目场景导致无法删除场景
正在编译代码时导入或删除子项目
在unity里删除项目文件夹时
子项目重命名
导入项目设置的警告
当前项目设置修改的重新导入
选择混淆当前项目时跳过**
importproject.xml非正常保存，空内容时，导致出错
识别导入项目是长屏/宽屏，在open时调整屏幕的旋转方向
-------------混淆器--------------
$符号字符串{}内的变量名
public delegate TOutput Converter<TInput, TOutput>(TInput from);

class Base { }
class Test<T, U>
    where U : struct
    where T : Base, new()
{ }


public class SampleClass<T, U, V> where T : V { }



using s = System.Text.RegularExpressions;
s.Regex reg=new s.Regex();

命名空间
类
结构
接口
枚举(Enum)
委托
	方法
	Lambda表达式
	属性和var属性
	索引器
	事件



泛型
元组和弃元
Iterators
public abstract class BaseApp<T>:BaseMonoBehaviour where T:class,new(){
public sealed class App:BaseApp<App>{







-------------------------
特殊文件夹:
可以在Assets的任意子目录
Editor
Resources

只能有一个，只能在根目录，直接位于 Assets 文件夹中
EditorDefaultResources
Gizmos
Plugins
StandardAssets
StreamingAssets
-------------------------

///////导入项目时将会忽略以下文件夹/////////////
Editor

EditorDefaultResources
Gizmos
Plugins
StandardAssets
StreamingAssets

DOTween
-------------------------


