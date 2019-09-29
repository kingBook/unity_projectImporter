namespace unity_framework{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	/// <summary>
	/// 文件工具类
	/// </summary>
	public static class FileUtil {
	
		/// <summary>
		/// 返回文件的所有行
		/// </summary>
		/// <param name="filePath">文件路径,如果是'\'路径,需要加@转换，如:getFileLines(@"E:\unity_tags\Assets\test.txt")</param>
		/// <param name="isAddLineEndEnter">行尾是否添加回车</param>
		/// <param name="readCount">读取的行数，-1或<0:读取所有行</param>
		/// <returns></returns>
		public static List<string> getFileLines(string filePath,bool isAddLineEndEnter,int readCount=-1){
			StreamReader streamReader=File.OpenText(filePath);
	
			List<string> fileLines=new List<string>();
			string line;
			int count=0;
			if(readCount!=0){
				while((line=streamReader.ReadLine())!=null){
					if(isAddLineEndEnter){
						line+='\n';//行尾加回车
					}
					fileLines.Add(line);
	
					count++;
					if(readCount>0&&count>=readCount){
						break;
					}
				}
			}
			streamReader.Dispose();
			return fileLines;
		}
	
		/// <summary>
		/// 将行字符串数组写入到本地(UTF-8格式)
		/// </summary>
		/// <param name="fileLines">行字符数组</param>
		/// <param name="filePath">写入文件的路径,如果是'\'路径,需要加@转换，如:getFileLines(@"E:\unity_tags\Assets\test.txt")</param>
		public static void writeFileLines(string[] fileLines,string filePath){
			File.Delete(filePath);
			var fileStream=File.Create(filePath);
	
			StringBuilder strBuilder=new StringBuilder();
			int len=fileLines.Length;
			for(int i=0;i<len;i++){
				strBuilder.Append(fileLines[i]);
			}
			UTF8Encoding utf8Bom=new UTF8Encoding(true);
			byte[] bytes=utf8Bom.GetBytes(strBuilder.ToString());
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Dispose();
		}
	
	
	}

}