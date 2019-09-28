namespace unity_framework{
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using UnityEngine;
	/// <summary>
	/// 文件加载器
	/// </summary>
	public class FileLoader:BaseMonoBehaviour{
		[Tooltip("进度条")]
		[SerializeField]
		private Progressbar _progressbar=null;
		
		/// <summary>
		/// 文件加载进度事件（是假模拟的进度）
		/// <br>void(float progress)</br>
		/// <br>progress：表示加载进度，范围[0,1]</br>
		/// </summary>
		public event Action<float> onProgress;
		/// <summary>
		/// 文件加载完成事件
		/// <br>void(byte[][] bytesList)</br>
		/// <br>bytesList：表示加载完成后各个文件的总字节(索引与加载时传递的参数对应)</br>
		/// </summary>
		public event Action<byte[][]> onComplete;
	
		private FileStream _fileStream;
		private bool _isLoading;
		private float _progressValue;
	
		/// <summary>
		/// 异步加载一个或多个本地文件
	    /// <br>如果文件不存在将在onComplete(byte[][] bytesList)事件参数bytesList添加一个null</br>
		/// </summary>
		/// <param name="filePaths">可变长度文件路径列表，如: @"C:\Users\Administrator\Desktop\views0.xml"</param>
		/// <param name="progressBarVisible">是否显示进度条</param>
		public async void loadAsync(bool progressbarVisible,params string[] filePaths){
			onLoadStart(progressbarVisible);
	
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
	
		private void onLoadStart(bool progressbarVisible){
			_isLoading=true;
			_progressValue=0.0f;
			if(_progressbar!=null){
				_progressbar.setProgress(_progressValue);
				_progressbar.setText("loading 0%...");
				_progressbar.gameObject.SetActive(progressbarVisible);
			}
			gameObject.SetActive(true);
		}
	
		private void onLoadCompleteAll(byte[][] outBytesList){
			_isLoading=false;
			_progressValue=1.0f;
			if(_progressbar!=null){
				_progressbar.setProgress(_progressValue);
				_progressbar.setText("loading 100%...");
				_progressbar.gameObject.SetActive(false);
			}
			gameObject.SetActive(false);
			
			onComplete?.Invoke(outBytesList);
		}
	
		protected override void Update2() {
			base.Update2();
			if(_isLoading){
				//模拟假的加载进度
				_progressValue=Mathf.Min(_progressValue+0.1f,0.9f);
				_progressbar.setProgress(_progressValue);
				_progressbar.setText("loading "+Mathf.FloorToInt(_progressValue*100)+"%...");
				onProgress?.Invoke(_progressValue);
			}
		}
	
		private void dispose(){
			if(_fileStream!=null){
				_fileStream.Dispose();
				_fileStream.Close();
				_fileStream=null;
			}
		}
	
		protected override void OnDestroy() {
			dispose();
			base.OnDestroy();
		}
	
	}

}