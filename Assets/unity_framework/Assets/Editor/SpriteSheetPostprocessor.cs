namespace unity_framework{
	using System.IO;
	using System.Xml;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// 切片flash导入出来的位图表
	/// </summary>
	public class SpriteSheetPostprocessor:AssetPostprocessor{
		
		//是否删除xml,(注意：只能用于测试,不删除时需要重新导入对应的png才能使切片数据正确导入)
		private bool _isDeleteXML=true;
		
		private void OnPreprocessAsset(){
			
		}
	
		private void OnPreprocessTexture(){
			
	    }
	
		private void OnPostprocessTexture(Texture2D texture){
			string dataPath=Application.dataPath;
			dataPath=dataPath.Substring(0,dataPath.LastIndexOf("/")+1);
	
			int dotIndex=assetPath.LastIndexOf('.');
			string xmlPath=assetPath.Substring(0,dotIndex)+".xml";
			xmlPath=dataPath+xmlPath;
	
			if(File.Exists(xmlPath)){
				OnSpriteSheetProcess(texture,xmlPath);
				if(_isDeleteXML){
					//删除.xml，一定要删除，否则下面重新导入会造成反复操作卡
					File.Delete(xmlPath);
					string pngPath=assetPath.Substring(0,dotIndex)+".png";
					//重新导入，更新切片数据（注意：请将.xml文件删除再重新导入，
					//否则会反复执行此项操作造成unity卡死）
					AssetDatabase.ImportAsset(pngPath);
				}
			}
		}
	
		private void OnSpriteSheetProcess(Texture2D texture,string xmlPath){
			var doc=new XmlDocument();
			doc.Load(xmlPath);
	
			var nodes=doc.DocumentElement.SelectNodes("SubTexture");
			var spritesheet=new SpriteMetaData[nodes.Count];
			float textureHeight=texture.height;
	
			Vector2 pivot=new Vector2();
			for(int i=0;i<nodes.Count;i++){
				XmlElement ele=nodes[i] as XmlElement;
				if(i==0){
					pivot.x=float.Parse(ele.GetAttribute("pivotX"));
					pivot.y=float.Parse(ele.GetAttribute("pivotY"));
				}
				string name=ele.GetAttribute("name");
				float x=float.Parse(ele.GetAttribute("x"));
				float y=float.Parse(ele.GetAttribute("y"));
				float width=float.Parse(ele.GetAttribute("width"));
				float height=float.Parse(ele.GetAttribute("height"));
				
				string frameXStr=ele.GetAttribute("frameX");
				if(string.IsNullOrEmpty(frameXStr))frameXStr="0";
				float frameX=float.Parse(frameXStr);
				string frameYStr=ele.GetAttribute("frameY");
				if(string.IsNullOrEmpty(frameYStr))frameYStr="0";
				float frameY=float.Parse(frameYStr);
	
				/*string frameWidthStr=ele.GetAttribute("frameWidth");
				if(string.IsNullOrEmpty(frameWidthStr))frameWidthStr="0";
				float frameWidth=float.Parse(frameWidthStr);
				string frameHeightStr=ele.GetAttribute("frameHeight");
				if(string.IsNullOrEmpty(frameHeightStr))frameHeightStr="0";
				float frameHeight=float.Parse(frameHeightStr);
	
				if(frameWidth>0)width=frameWidth;
				if(frameHeight>0)height=frameHeight;*/
	
				float poX=(pivot.x+frameX)/width;
				float poY=(height-pivot.y-frameY)/height;
				
				var spriteMetaData=new SpriteMetaData();
				spriteMetaData.name=name;
				spriteMetaData.alignment=(int)SpriteAlignment.Custom;
				spriteMetaData.pivot=new Vector2(poX,poY);
				spriteMetaData.rect=new Rect(x,-y+textureHeight-height,width,height);
				spritesheet[i]=spriteMetaData;
				//
			}
			var importer=assetImporter as TextureImporter;
			importer.spriteImportMode=SpriteImportMode.Multiple;
			importer.spritesheet=spritesheet;
		}
	
	    /*private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths){
	        foreach (string str in importedAssets){
				if(str.EndsWith(".png")||str.EndsWith(".PNG")){
					OnSpriteSheetPostprocess(str);
				}
	        }
	    }
	
		private static void OnSpriteSheetPostprocess(string path){
			string dataPath=Application.dataPath;
			dataPath=dataPath.Substring(0,dataPath.LastIndexOf("/")+1);
	
			int dotIndex=path.LastIndexOf('.');
			string xmlPath=path.Substring(0,dotIndex)+".xml";
			xmlPath=dataPath+xmlPath;
	
			if(File.Exists(xmlPath)){
				var doc=new XmlDocument();
				doc.Load(xmlPath);
				XmlElement firstEle=doc.DocumentElement.FirstChild as XmlElement;
				var nodes=doc.DocumentElement.SelectNodes("SubTexture");
				for(int i=0;i<nodes.Count;i++){
					XmlElement ele=nodes[i] as XmlElement;
					string name=ele.GetAttribute("name");
					
				}
			}
			//
			//parseAndExportXml(path);
			Debug.Log("OnSpriteSheetPostprocess");
			
			//var settings=new TextureGenerationSettings();
			//var spriteImportData=new SpriteImportData();
			//spriteImportData.rect=new Rect(0,0,50,50);
			//settings.spriteImportData=new SpriteImportData[]{spriteImportData};
			//TextureGenerator.GenerateTexture(settings,colorBuffer);
		}*/
	}

}