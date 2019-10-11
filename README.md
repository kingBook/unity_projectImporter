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
打开子项目场景导致无法删除场景
正在编译代码时导入或删除子项目
在unity里删除项目文件夹时
子项目重命名
导入项目设置的警告
当前项目设置修改的重新导入
选择混淆当前项目时跳过**

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


