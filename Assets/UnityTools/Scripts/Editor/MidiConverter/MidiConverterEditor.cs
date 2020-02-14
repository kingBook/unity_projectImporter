using UnityEngine;
using UnityEditor;
using System.IO;

public class MidiConverterEditor:ScriptableObject{
	
	[MenuItem("Tools/MidiToJSON")]
	public static void MidiToJSON(){
		string dataPath=Application.dataPath.Replace('/','\\');
		string midiFilePath=dataPath+@"\UnityTools\song.mid";
		
		FileStream fileStream=File.OpenRead(midiFilePath);

		string midiJSON=MidiParser.Parse(fileStream);

	}

	
}