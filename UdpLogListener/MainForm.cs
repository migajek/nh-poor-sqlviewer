/*
 * Created by SharpDevelop.
 * User: michal
 * Date: 2013-09-27
 * Time: 17:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace UdpLogListener
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	/// 
	
	internal class QueryParameter 
	{
		private string _rawType;		
		
		public string Name {get; set;}
		public string RawValue {get; set;}

		public int? RawTypeLength {get; protected set; }
		
		public string RawType {
		get{
				return _rawType;
			}
		set {
				_rawType = value;
				Match m = Regex.Match(value, @"^Type\s?\:\s?(?<Type>[\w\d]+)(\s\((?<Len>\d+)\))?", RegexOptions.IgnoreCase);
				if (m.Success)
				{
					this.TypeName = m.Groups["Type"].Value;
					if (m.Groups["Len"].Success)
						this.RawTypeLength = int.Parse(m.Groups["Len"].Value);
				}
			}
		}
		
		public string TypeName{get; protected set;}
		
		public string ToSQL()
		{
			if (this.TypeName == "DateTime")
			{
				return "'" + this.ToString() + "'";
			}
			return this.RawValue;
		}
		
		public override string ToString()
		{
			if (this.TypeName == "String")
			{
				Match m = Regex.Match(this.RawValue, @"\'(?<V>[^\']+)");
				if (m.Success)
					return m.Groups["V"].Value;
			} else if (this.TypeName == "Int32")
			{
				return Int32.Parse(this.RawValue).ToString();
			} else if (this.TypeName == "DateTime")
			{
				return DateTime.Parse(this.RawValue).ToString();
			}
			return this.RawValue;
			/*Dictionary<string,string> handled = new Dictionary<string, string>() {
				{"Int32", "System.Int32"},
				{"String", "System.String"},				
				{"DateTime", "System.DateTime"},
			};
			if (handled.ContainsKey(this.TypeName))
			{
				var t = Type.GetType(handled[this.TypeName]);
				if (t != null)
				{
					MethodInfo theMethod = t.GetMethod("Parse");
					object inst;
					try 
					{
						inst = Activator.CreateInstance(t);
					} catch
					{
						return this.RawValue;
					}
					if (inst != null && theMethod != null)
					{
						return (string)theMethod.Invoke(inst, new object[] {this.RawValue});
					}
				}
			}
			return this.RawValue;*/
		}
		
	}
	
	public partial class MainForm : Form
	{		
		UdpClient udpClient;
		
		private PoorMansTSqlFormatterLib.Interfaces.ISqlTreeFormatter _formatter;
		private PoorMansTSqlFormatterLib.Interfaces.ISqlTokenizer _tokenizer;
		private PoorMansTSqlFormatterLib.Interfaces.ISqlTokenParser _parser;

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
		
		}
		
		private void DataReceived(IAsyncResult ar)
		{
			UdpClient c = (UdpClient)((UdpClient)ar.AsyncState);
			IPEndPoint rem = new IPEndPoint(IPAddress.Any, 0);
			Byte[] receiveBytes = c.EndReceive(ar, ref rem);			
		    var loggingEvent = System.Text.Encoding.UTF8.GetString(receiveBytes);
		    
		    this.Invoke((MethodInvoker)delegate {
		                	listBox1.Items.Add(loggingEvent); // runs on UI thread
			});
		    udpClient.BeginReceive(new AsyncCallback(DataReceived),c);
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{			
			textBox1.Text = 
@"/** Simple UDP Log Listener by Michał migajek Gajek
              for NHibernate & log4net
     
Features
* Handles incoming NHibernate SQL log messages
* recreates query (replacing binding parameters with 
 their respective values (only String, Integer, DateTime are supported right now)
*/";       
       
			
			udpClient = new UdpClient(8095);
			udpClient.BeginReceive(new AsyncCallback(DataReceived),udpClient);
			
			
			_formatter = new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatter();
			_tokenizer = new PoorMansTSqlFormatterLib.Tokenizers.TSqlStandardTokenizer();
            _parser = new PoorMansTSqlFormatterLib.Parsers.TSqlStandardParser();			
		}
		
		private Dictionary<string,QueryParameter> BuildQueryParams(string s)
		{
			// https://www.debuggex.com/  && http://regexhero.net/ !! <3 :D
			Dictionary<string,QueryParameter> paramsDict = new Dictionary<string, QueryParameter>();
			MatchCollection matches = Regex.Matches(s, 				
				@"\?p(?<PId>\d+)\s?\=\s?(?<Value>[^\[]+)\s\[(?<Type>[^\]]+)\]");
			foreach (Match match in matches)
			{
				//\s?\]\?s$
				string name = match.Groups["PId"].Value;
				paramsDict[name] = new QueryParameter() {
					Name = match.Groups["Value"].Value,
					RawType = match.Groups["Type"].Value,
					RawValue = match.Groups["Value"].Value
				};				
			}
			return paramsDict;
		}
		
		private string PrepareStatement(string s)
		{
			
			var splitPoint = s.LastIndexOf(';');
			if (splitPoint != -1)
			{
				var actualQuery = s.Substring(0, splitPoint);
				var paramsPart = s.Substring(splitPoint + 1);
				
				Dictionary<string,QueryParameter> pd = BuildQueryParams(paramsPart);
				foreach (var p in pd)
				{
					tbLog.AppendText(p.Key + " = " + p.Value.TypeName + " | " + p.Value.RawTypeLength 
					                 + " | " + p.Value.RawValue + " >>> " + p.Value.ToString()+ Environment.NewLine );
					
					var name = "?p" + p.Key;
					actualQuery = actualQuery.Replace(name, p.Value.ToSQL());
				}
				return actualQuery;
			}			
			
			return s;
		}
		
		private string DoFormatting(string s)
        {
            var tokenizedSql = _tokenizer.TokenizeSQL(s);

            /*if (!splitContainer4.Panel2Collapsed && !splitContainer5.Panel1Collapsed)
                txt_TokenizedSql.Text = tokenizedSql.PrettyPrint();*/

            var parsedSql = _parser.ParseSQL(tokenizedSql);
            
            /*if (!splitContainer4.Panel2Collapsed && !splitContainer5.Panel2Collapsed)
                txt_ParsedXml.Text = parsedSql.OuterXml;*/

            return _formatter.FormatSQLTree(parsedSql);
        }

		
		void BackgroundWorker1DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
		
		}
		
		void ListBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex > -1)
				textBox1.Text = DoFormatting(PrepareStatement(listBox1.SelectedItem.ToString()));
		}
		
		void ListBox1Click(object sender, EventArgs e)
		{
			
		}
	}
}
