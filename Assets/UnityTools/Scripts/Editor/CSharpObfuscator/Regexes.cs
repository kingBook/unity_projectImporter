namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using System.Text.RegularExpressions;
	/// <summary>
	/// 正则表达式集合
	/// </summary>
    public static class Regexes{
		
		/// <summary>
		/// 匹配一个单词的表达式
		/// <para><c>Groups["word"] //表示单词</c></para>
		/// </summary>
		public static readonly Regex wordRegex=new Regex(@"(?<word>\b\w+\b)",RegexOptions.Compiled);

		/// <summary>
		/// 匹配"."路径的表达式(<see cref="DotPath"/>)
		/// <para><c>Groups["dotPath"] //表示整个路径</c></para>
		/// </summary>
		public static readonly Regex dotPathRegex=new Regex(@"(?<dotPath>"+wordRegex+@"(\s*\.\s*"+wordRegex+@")+)",RegexOptions.Compiled);

		/// <summary>
		/// 匹配一对尖括号"&lt;...&gt;"
		/// <para><c>Groups["angleBrackets"] //表示尖括号内容(包含尖括号)</c></para>
		/// </summary>
		public static readonly Regex angleBracketsRegex=new Regex(@"(?<angleBrackets>\<(?:[^<>]|(?<openAngleBracket>\<)|(?<-openAngleBracket>\>))*(?(openAngleBracket)(?!))\>)",RegexOptions.Compiled);
		
		/// <summary>
		/// 匹配一对小括号"(...)"
		/// <para><c>Groups["parentheses"] //表示尖括号内容(包含尖括号)</c></para>
		/// </summary>
		public static readonly Regex parenthesesRegex=new Regex(@"(?<parentheses>\((?:[^()]|(?<openParenthesis>\()|(?<-openParenthesis>\)))*(?(openParenthesis)(?!))\))",RegexOptions.Compiled);

		/// <summary>
		/// 匹配一对大括号"{...}"
		/// <para><c>Groups["braces"] //表示尖括号内容(包含尖括号)</c></para>
		/// </summary>
		public static readonly Regex bracesRegex=new Regex(@"(?<braces>\{(?:[^{}]|(?<openBrace>\{)|(?<-openBrace>\}))*(?(openBrace)(?!))\})",RegexOptions.Compiled);
		
		/// <summary>
		/// 匹配单词+尖括号的表达式(<see cref="WordAngleBrackets"/>)
		/// <para><c>Groups["wordAngleBrackets"] //表示单词+尖括号</c></para>
		/// </summary>
		public static readonly Regex wordAngleBracketsRegex=new Regex(@"(?<wordAngleBrackets>"+wordRegex+@"\s*"+angleBracketsRegex+@")",RegexOptions.Compiled);

		/// <summary>
		/// 匹配点路径+尖括号的表达式
		/// <para><c>Groups["wordAngleBrackets"] //表示点路径+尖括号</c></para>
		/// </summary>
		public static readonly Regex dotPathAngleBracketsRegex=new Regex(@"(?<dotPathAngleBrackets>"+dotPathRegex+@"\s*"+angleBracketsRegex+")",RegexOptions.Compiled);

		/// <summary>
		/// 匹配"点路径+尖括号"|"名称+尖括号"|"点路径"|"名称"
		/// </summary>
		public static readonly Regex dotPathAngleBrackets_nameAngleBrackets_dotPath_wordRegex=new Regex(@"("+dotPathAngleBracketsRegex+@")|("+wordAngleBracketsRegex+@")|("+dotPathRegex+@")|("+wordRegex+@")",RegexOptions.Compiled);

		/// <summary>
		/// 分隔尖括号里的内容表达式(以","作分隔符,尖括号可能出现的内容有:"点路径+尖括号"，"名称+尖括号"，"点路径"，"名称")
		///	<para><c>Groups["splitContent"].Captures //表示以","分隔的各个内容块</c></para>
		/// </summary>
		public static readonly Regex splitAngleBracketsRegex=new Regex(@"(?<splitContent>"+dotPathAngleBrackets_nameAngleBrackets_dotPath_wordRegex+@")(\s*,\s*(?<splitContent>"+dotPathAngleBrackets_nameAngleBrackets_dotPath_wordRegex+@"))*",RegexOptions.Compiled);


	}
}
