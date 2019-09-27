namespace UnityProjectImporter{
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using UnityEngine;
	
	/// <summary>
	/// 文件加载器
	/// </summary>
	public class FileLoader{
		
		/// <summary>
		/// 文件加载完成事件
		/// <br>void(byte[][] bytesList)</br>
		/// <br>bytesList：表示加载完成后各个文件的总字节(索引与加载时传递的参数对应)</br>
		/// </summary>
		public event Action<byte[][]> onComplete;
		
		private bool _isDestroyed;
		private FileStream _fileStream;
		private bool _isLoading;
		
		/// <summary>
		/// 异步加载一个或多个本地文件
		/// <br>如果文件不存在将在onComplete(byte[][] bytesList)事件参数bytesList添加一个null</br>
		/// </summary>
		/// <param name="filePaths">可变长度文件路径列表，如: @"C:\Users\Administrator\Desktop\views0.xml"</param>
		public async void loadAsync(params string[] filePaths){
			onLoadStart();

			byte[][] outBytesList=new byte[filePaths.Length][];
			for(int i=0;i<filePaths.Length;i++){
				byte[] buffer=null;
				string filePath=filePaths[i];
				await Task.Run(()=>{
					if(File.Exists(filePath)){
						_fileStream=File.OpenRead(filePath);

						int fileLength=(int)_fileStream.Length;
						buffer=new byte[fileLength];

						_fileStream.Read(buffer,0,fileLength);
					}
				});
				if(_isDestroyed){
					//加载过程中，删除该脚本绑定的对象时，打断
					break;
				}
				outBytesList[i]=buffer;
				dispose();
			}

			//所有加载完成
			if(!_isDestroyed){
				onLoadCompleteAll(outBytesList);
			}
		}

		private void onLoadStart(){
			_isLoading=true;
		}

		private void onLoadCompleteAll(byte[][] outBytesList){
			_isLoading=false;
			onComplete?.Invoke(outBytesList);
		}

		private void dispose(){
			if(_fileStream!=null){
				_fileStream.Dispose();
				_fileStream.Close();
				_fileStream=null;
			}
		}
		
		public void destroy(){
			if(_isDestroyed)return;
			_isDestroyed=true;
			
			dispose();
		}
		
		public bool isLoading{ get => _isLoading; }

	}
}