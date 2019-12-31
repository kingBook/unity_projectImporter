-------------导入器待完成：--------------
Resources、Plugins文件夹的处理
打开子项目场景导致无法删除场景
正在编译代码时导入或删除子项目
在unity里删除项目文件夹时
子项目重命名
当前项目设置修改的重新导入
选择混淆当前项目时跳过**
importproject.xml非正常保存，空内容时，导致出错
识别导入项目是长屏/宽屏，在open时调整屏幕的旋转方向
导入.cs文件时重命名已存在的命名空间(防止命名空间冲突)
PlayerSettings里的自定义条件编译常量**
-------------混淆器待完成：--------------
$符号字符串{}内的变量名
C#特性
#if#endif
------------------------------------------

将其它unity项目导入到当前项目,实现的项目设置(Project Settings)：
Physics
Physics2D
Quality
Tags and Layers
Time
EditorBuildSettings
-------------------------


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


https://stackoverflow.com/questions/7898310/using-regex-to-balance-match-parenthesis



\((?:[^()]|(?<open> \( )|(?<-open> \) ))+(?(open)(?!))\)
", RegexOptions.IgnorePatternWhitespace);




public delegate TOutput Converter<TInput, TOutput>(TInput from);

class Base { }
class Test<T, U>
    where U : struct
    where T : Base, new()
{ }


public class SampleClass<T, U, V> where T : V { }



using s = System.Text.RegularExpressions;
s.Regex reg=new s.Regex();
//委托
public delegate int Func<T> () where T:class;
public delegate TResult Func<out TResult>();
public delegate TResult Func<in T1, out TResult>(T1 arg);
public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);

public delegate bool Predicate<in T>(T obj);

//属性
public string FirstName;
public string FirstName { get; set; }
public string FirstName { get; set; } = string.Empty;
public string FirstName { get; private set; }
public ICollection<DataPoint> points { get; } = new List<DataPoint>();

public class Person
{
    public string FirstName
    {
        get { return firstName; }
        set { firstName = value; }
    }
    private string firstName;
    // remaining implementation removed from listing
}

public class Person
{
    public string FirstName
    {
        get => firstName;
        set => firstName = value;
    }
    private string firstName;
    // remaining implementation removed from listing
}

//lamded
public Person(string firstName) => this.FirstName = firstName;
public string FullName => $"{FirstName} {LastName}";

=====C#概念=====
.cs文件（using，命名空间，接口，类，结构，枚举，委托）
using
命名空间(using，命名空间，接口，类，结构，枚举，委托)
接口(接口，类，结构，枚举，事件，委托，接口方法)
类(接口，类，结构，枚举，事件，委托，属性，方法)
结构(接口，类，结构，枚举，事件，委托，属性，方法)
枚举
事件(未实现)
委托
属性
方法
------
Lambda
元组
泛型
索引器
================


泛型(接口、委托的泛型可以带“in”和"out"在尖括号)


Iterators


public abstract class BaseApp<T>:BaseMonoBehaviour where T:class,new(){
public sealed class App:BaseApp<App>{










