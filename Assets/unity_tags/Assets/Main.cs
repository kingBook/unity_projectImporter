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
            string text="a\"//b\ncdefghi*/*jk*\"/" +
			"lmn";
            
            int a=1/* 123*/;//456
            string bb="/*int b=a+5;*/";
			/*joinString("ye\"  s","	yo u");
			joinString(@"y""e  s","	yo u");*/
			joinString(@"a""b  c",@"	def");
			joinString(@"aaa",@"b@""b""b","ccc");
			/*
			Regex regex=new Regex(@"/w+[(@"")]""\s*""");*/
		}
		
		private string joinString(string a,string b,string c=null){
			return a+b;
		}
		
		/*private void capSortingLayerText(Match m){
			SceneManager2.GetSceneByName("sdf");
			
		}*/
		

	}
	/**KPFSDF*/
	namespace 
	KPFSDF
	
	{
		using System.Dynamic;
	}
	

}
/**
 * XXSS
 * a
 * bd
 * d
 * */
namespace XXSS {
    using System.Collections.Generic;
    using System.Data;
	using XXSS.OOAA.Koo;

	namespace OOAA 
		.    
		Koo  
		{
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

			public interface IName<T>{
                void name();
            }
			public interface IGood<T> where T:class,new(){
                void good();
            }
			public interface IHei{
				void hei();
			}
			public interface IDot:IHei,IName<int>{ }
			
			public class HelloB{string b="HelloB";}
			public class HelloC{string c="HelloC";}
			
			public class HelloD<T>
			:HelloName,IHei,XXSS.OOAA.Koo.IDot where T:class,new(){
				public void hei(){ }
				public void name(){}
				public void good(){}
			}
			public class HelloE:IName<int>,XXSS.OOAA.Koo.IDot{
				public void hei(){ }
				public void name(){}

				protected readonly struct HelloEStruct{}

				protected interface HelloEInterface{}

				private enum HelloEEnumA{A,B,C,D}
				private enum HelloEEnumB:int{A,B,C,D}
			}
			public class HelloF<T,U,K>:HelloE where T:class,new() where U:IName<int> where K:IHei,IDot{
				
			}

			public class 中文父类{ }
			public class 中文子类:中文父类{}
		}
		public struct AA{
			
		}

		public struct BB:IHei{
			public void hei(){ }
		}

		public interface ssdf{
			void hello();
		}
		
}

public interface OOA{
	void hello2();
}

public enum TypeA : int
	
		{
	A
}

public delegate void OnComplete(int a);