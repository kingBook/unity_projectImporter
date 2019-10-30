using System;
namespace unity_tags{
	using System.Collections;
	using System.	Collections .Generic;
	using System.	Text.
	RegularExpressions;
	using UnityEngine;
	using UnityEngine.
	
	SceneManagement;
	using  static   UnityEngine  . Mathf  ;
	using Mathx=UnityEngine.Mathf;
	
	public delegate int Callee(int a);
	/// <summary>
    /// Main类
    /// </summary>
	public class Main : MonoBehaviour{
		public SpriteRenderer spriteRenderer;
	    void Start(){
			SortingLayer2 [] list0=SortingLayer2.layers;
			SortingLayer2[ ] list1=SortingLayer2.layers;
			var list2 =SortingLayer2.layers;
			SortingLayer2 layer=list1[1];
            //"."
            string text="a\"//b\ncdefghi/*jk*\"/" +
			"lmn";
            
            int a=1/* 123*/;//456
            string bb="/*int b=a+5;*/";
			joinString("ye\"  s","	yo u");
			joinString(@"y""e  s","	yo u");
			joinString(@"a""b  c",@"	def");
			joinString(@"aaa",@"b@""b""b","ccc");

			Regex regex=new Regex(@"/w+[(@"")]""\s*""");
		}

		private string joinString(string a,string b,string c=null){
			return a+b;
		}
		
		/*private void capSortingLayerText(Match m){
			SceneManager2.GetSceneByName("sdf");
			
		}*/
		

	}

	namespace KPFSDF{
		using System.Dynamic;
	}
	

}

namespace XXSS {
		using System.Data;
		namespace OOAA .    Koo   {
			public class HelloName{
					private enum TypeCC{
						
					}
					private struct MyStruct{
						
					}
					protected class Test{
						
					}
					private interface IInterface{
						
					}
			}
			
			public class HelloB{}
			public class HelloC{}
			
			public class HelloD<T>:HelloName where T: class, new()  {
				
			}
			public class HelloE:HelloD < HelloE>{
				
			}
		}
		public struct AA{
			
		}

		public interface ssdf{
			void hello();
		}
		
}

public interface OOA{
	void hello2();
}

public enum TypeA
	
		{
	A
}

public delegate void OnComplete(int a);