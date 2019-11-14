using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityTools;

public class Test : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
	string text=" System.Collections.你好123.Generic;";
	//匹配三个"任意字符."
	Regex regex=new Regex(@"(\w+.){3}",RegexOptions.Compiled);
	Match match=regex.Match(text);
	if(match.Success){
		Debug.Log(match.Value);//output: System.Collections.你好123.
	}
	}

	// Update is called once per frame
	void Update()
	{
	
	}
	///
	public void onClickHandler(){
		
	}
	
}
