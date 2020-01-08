namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using System.Text.RegularExpressions;
	/// <summary>
	/// 正则表达式集合
	/// </summary>
    public static class Regexes{
		
		/// <summary>
		/// 匹配任何内容
		/// </summary>
		public static readonly Regex anyRegex=new Regex(@"[\s\S]",RegexOptions.Compiled);
		
		/// <summary>
		/// 匹配一个单词的表达式
		/// <para><c>Groups["word"] //表示单词</c></para>
		/// </summary>
		public static readonly Regex wordRegex=new Regex(@"(?<word>\b\w+\b)",RegexOptions.Compiled);

		/// <summary>
		/// 匹配单词+空格+单词
		/// </summary>
		public static readonly Regex wordSpaceword=new Regex(wordRegex+@"\s+"+wordRegex,RegexOptions.Compiled);

		/// <summary>
		/// 匹配"new()"
		/// </summary>
		public static readonly Regex newParenthesesRegex=new Regex(@"(?<newParentheses>new\s*\(\s*\))",RegexOptions.Compiled);

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
		/// 匹配"点路径+尖括号"|"名称+尖括号"|"点路径"|"单词"
		/// </summary>
		public static readonly Regex dotPathAngleBrackets_wordAngleBrackets_dotPath_wordRegex=new Regex(@"("+dotPathAngleBracketsRegex+@"|"+wordAngleBracketsRegex+@"|"+dotPathRegex+@"|"+wordRegex+@")",RegexOptions.Compiled);
		
		/// <summary>
		/// 匹配"点路径+尖括号"|"名称+尖括号"|"点路径"|"单词+空格+单词"|"单词"
		/// </summary>
		public static readonly Regex dotPathAngleBrackets_wordAngleBrackets_dotPath_wordSpaceWord_wordRegex=new Regex(@"("+dotPathAngleBracketsRegex+@"|"+wordAngleBracketsRegex+@"|"+dotPathRegex+@"|"+wordSpaceword+@"|"+wordRegex+@")",RegexOptions.Compiled);

		/// <summary>
		/// 匹配"点路径+尖括号"|"名称+尖括号"|"点路径"|"new()"|"单词"
		/// </summary>
		public static readonly Regex dotPathAngleBrackets_wordAngleBrackets_dotPath_newParentheses_wordRegex=new Regex(@"("+dotPathAngleBracketsRegex+@"|"+wordAngleBracketsRegex+@"|"+dotPathRegex+@"|"+newParenthesesRegex+@"|"+wordRegex+@")",RegexOptions.Compiled);

		/// <summary>
		/// 分隔尖括号里的内容表达式(以","作分隔符,尖括号可能出现的内容有:"点路径+尖括号"，"名称+尖括号"，"点路径"，"单词+空格+单词"，"单词")
		///	<para><c>Groups["splitContent"].Captures //表示以","分隔的各个内容块</c></para>
		/// </summary>
		public static readonly Regex splitAngleBracketsRegex=new Regex(@"(?<splitAngleBracketsContent>"+dotPathAngleBrackets_wordAngleBrackets_dotPath_wordSpaceWord_wordRegex+@")(\s*,\s*(?<splitAngleBracketsContent>"+dotPathAngleBrackets_wordAngleBrackets_dotPath_wordSpaceWord_wordRegex+@"))*",RegexOptions.Compiled);

		/// <summary>
		/// 匹配字符串
		/// </summary>
		public static readonly Regex stringTextRegex=new Regex(@"(?<stringText>(@"".*"")|("".*""))",RegexOptions.Compiled);

		/// <summary>
		/// 匹配行注释
		/// </summary>
		public static readonly Regex lineCommentsRegex=new Regex(@"(?<lineComments>//.*)",RegexOptions.Compiled);

		/// <summary>
		/// 匹配一个泛型约束的表达式
		/// </summary>
		public static readonly Regex genericConstraintRegex=new Regex(@"where\s+(?<genericConstraintName>\w+)\s*:\s*(?<genericConstraintSplitContent>"+dotPathAngleBrackets_wordAngleBrackets_dotPath_newParentheses_wordRegex+@")(\s*,\s*(?<genericConstraintSplitContent>"+dotPathAngleBrackets_wordAngleBrackets_dotPath_newParentheses_wordRegex+"))*",RegexOptions.Compiled);

		
		public static readonly Regex sharpIfRegex=new Regex(@"#if\s+(?<condition>(\s|\S)+?)(\r|\n)",RegexOptions.Compiled);
		public static readonly Regex sharpElifRegex=new Regex(@"#elif\s+(?<condition>(\s|\S)+?)(\r|\n)",RegexOptions.Compiled);
		public static readonly Regex sharpElseRegex=new Regex(@"#else\s+(?<condition>(\s|\S)+?)(\r|\n)",RegexOptions.Compiled);
		public static readonly Regex sharpEndifRegex=new Regex(@"#endif(\r|\n)",RegexOptions.Compiled);

	}
}
