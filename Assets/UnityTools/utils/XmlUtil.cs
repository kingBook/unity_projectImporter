namespace UnityTools{ 
    using UnityEngine;
    using System.Xml;
    using System.IO;
    using System.Text;
    /// <summary>
    /// xml工具类
    /// </summary>
    public static class XmlUtil{
        
        /// <summary>
        /// 创建XmlDocument
        /// </summary>
        /// <param name="isAddXmlDeclaration">是否添加xml声明</param>
        /// <returns></returns>
        public static XmlDocument createXmlDocument(bool isAddXmlDeclaration){
            XmlDocument doc=new XmlDocument();
            if(isAddXmlDeclaration){
                XmlDeclaration xmlDeclaration=doc.CreateXmlDeclaration("1.0","UTF-8",null);
                doc.AppendChild(xmlDeclaration);
            }
            return doc;
        }

        /// <summary>
        /// 根据字符串创建XmlDocument
        /// </summary>
        /// <param name="xmlDocumentString"></param>
        /// <param name="isAddXmlDeclaration"></param>
        /// <returns></returns>
        public static XmlDocument createXmlDocument(string xml,bool isAddXmlDeclaration){
            XmlDocument doc=createXmlDocument(isAddXmlDeclaration);
            doc.LoadXml(xml);
            return doc;
        }

        /// <summary>
        /// 返回一个格式化的xml字符串
        /// </summary>
        /// <param name="xml">合法的xml字符串</param>
        /// <returns></returns>
        public static string formatXml(string xml){
            XmlDocument doc=createXmlDocument(xml,false);
            return formatXml(doc);
        }

        /// <summary>
        /// 返回格式化XmlDocument字符串
        /// </summary>
        /// <param name="doc">XmlDocument</param>
        /// <returns></returns>
        public static string formatXml(XmlDocument doc){
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);  
            XmlTextWriter xtw = null;  
            try{  
                xtw = new XmlTextWriter(sw);  
                xtw.Formatting = Formatting.Indented;  
                xtw.Indentation = 1;  
                xtw.IndentChar = '\t';  
                doc.WriteTo(xtw);  
            }finally{  
                if (xtw != null)  
                    xtw.Close();  
            }  
            return sb.ToString();
        } 


    }
}