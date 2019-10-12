using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		string str="0123456_7_89";
        Regex regexBracket=new Regex(@"_7_",RegexOptions.Compiled|RegexOptions.RightToLeft);
		Match match=regexBracket.Match(str,7,3);
		Debug.Log(match.Success);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void onClickHandler(){
		
	}
}
