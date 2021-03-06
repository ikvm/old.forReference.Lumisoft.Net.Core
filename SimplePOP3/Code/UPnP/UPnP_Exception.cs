﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace LumiSoft.Net.UPnP
{
    /// <summary>
    /// This class represents UPnP error.
    /// </summary>
    public class UPnP_Exception : Exception
    {
        private int    m_ErrorCode = 0;
        private string m_ErrorText = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="errorCode">UPnP error code.</param>
        /// <param name="errorText">UPnP error text.</param>
        public UPnP_Exception(int errorCode,string errorText) : base("UPnP error: " + errorCode + " " + errorText + ".")
        {
            m_ErrorCode = errorCode;
            m_ErrorText = errorText;
        }

        
        /// <summary>
        /// Parses UPnP exception from UPnP xml error.
        /// </summary>
        /// <param name="stream">Error xml stream.</param>
        /// <returns>Returns UPnP exception.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        public static UPnP_Exception Parse(Stream stream)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }

            int    errorCode = -1;
            string errorText = null;

            try{
                XmlDocument xml = new XmlDocument();
                xml.Load(stream);
                
                // Loop XML tree by nodes.
                List<XmlNode> queue = new List<XmlNode>();
                queue.Add(xml);
                while(queue.Count > 0){
                    XmlNode currentNode = queue[0];
                    queue.RemoveAt(0);
               
                    if(String2.Equals("UPnPError",currentNode.Name,StringComparison2.InvariantCultureIgnoreCase)){                        
                        foreach(XmlNode node in currentNode.ChildNodes){                        
                            if(String2.Equals("errorCode",node.Name,StringComparison2.InvariantCultureIgnoreCase)){
                                errorCode = Convert.ToInt32(node.InnerText);
                            }
                            else if(String2.Equals("errorDescription",node.Name,StringComparison2.InvariantCultureIgnoreCase)){
                                errorText = node.InnerText;
                            }
                        }

                        break;
                    }
                    else if(currentNode.ChildNodes.Count > 0){
                        for(int i=0;i<currentNode.ChildNodes.Count;i++){
                            queue.Insert(i,currentNode.ChildNodes[i]);
                        }
                    }                    
                }
            }
            catch{
            }

            if(errorCode == -1 || errorText == null){
                throw new ParseException("Failed to parse UPnP error.");
            }
                        
            return new UPnP_Exception(errorCode,errorText);
        }

        


        
        /// <summary>
        /// Gets UPnP error code.
        /// </summary>
        public int ErrorCode
        {
            get{ return m_ErrorCode; }
        }

        /// <summary>
        /// Gets UPnP error text.
        /// </summary>
        public string ErrorText
        {
            get{ return m_ErrorText; }
        }

        
    }
}
