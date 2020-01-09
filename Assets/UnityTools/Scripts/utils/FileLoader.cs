using System;
using System.IO;
using System.Threading.Tasks;

namespace UnityTools{
	/// <summary>
	/// 文件加载器
	/// </summary>
	public class FileLoader{
		
		/// <summary>
		/// 一个文件加载完成事件
		/// <br>void(byte[] bytes,int id)</br>
		/// <br>bytes：表示加载完成的文件的总字节数组</br>
		/// <br>id：表示完成的索引号（与加载时传递的参数对应）</br>
		/// </summary>
		public event Action<byte[],int> onComplete;
		
		/// <summary>
		/// 所有文件加载完成事件
		/// <br>void(byte[][] bytesList)</br>
		/// <br>bytesList：表示加载完成后各个文件的总字节数组（索引与加载时传递的参数对应）</br>
		/// </summary>
		public event Action<byte[][]> onCompleteAll;
		
		private bool m_isDestroyed;
		private FileStream m_fileStream;
		private bool m_isLoading;
		
		/// <summary>
		/// 异步加载一个或多个本地文件
		/// <br>如果文件不存在将在onComplete(byte[][] bytesList)事件参数bytesList添加一个null</br>
		/// </summary>
		/// <param name="filePaths">可变长度文件路径列表，如: @"C:\Users\Administrator\Desktop\views0.xml"</param>
		public async void LoadAsync(params string[] filePaths){
			OnLoadStart();

			byte[][] outBytesList=new byte[filePaths.Length][];
			for(int i=0;i<filePaths.Length;i++){
				byte[] buffer=null;
				string filePath=filePaths[i];
				await Task.Run(()=>{
					if(File.Exists(filePath)){
						m_fileStream=File.OpenRead(filePath);

						int fileLength=(int)m_fileStream.Length;
						buffer=new byte[fileLength];

						m_fileStream.Read(buffer,0,fileLength);
					}
				});
				if(m_isDestroyed){
					//加载过程中，删除该脚本绑定的对象时，打断
					break;
				}
				outBytesList[i]=buffer;
				onComplete?.Invoke(buffer,i);
				Dispose();
			}

			//所有加载完成
			if(!m_isDestroyed){
				OnLoadCompleteAll(outBytesList);
			}
		}

		private void OnLoadStart(){
			m_isLoading=true;
		}

		private void OnLoadCompleteAll(byte[][] outBytesList){
			m_isLoading=false;
			onCompleteAll?.Invoke(outBytesList);
		}

		private void Dispose(){
			if(m_fileStream!=null){
				m_fileStream.Dispose();
				m_fileStream.Close();
				m_fileStream=null;
			}
		}
		
		public void Destroy(){
			if(m_isDestroyed)return;
			m_isDestroyed=true;
			
			Dispose();
		}
		
		public bool isLoading{ get => m_isLoading; }

	}
}