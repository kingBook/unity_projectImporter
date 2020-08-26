using UnityEngine;
using System.Collections;
using System.IO;

public class MidiParser{
	
	public static string Parse(FileStream stream){
		var headerChunk=ReadChunk(stream);
		return null;
	}

	private static (int id,int length,string data)ReadChunk(FileStream stream){
		return (1,1,"");
	}
	
}
