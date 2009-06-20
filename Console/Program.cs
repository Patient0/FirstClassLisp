/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 20/06/2009
 * Time: 01:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using Lisp;
using System.IO;

namespace ConsoleClient
{
	class Program
	{
	
	    static void Main(string[] args)
	    {
	    	try
	    	{
			    string sample = "( one\t (two\t  456 -43.2 \" \\\" quoted\" ))";
			
				var defs = new TokenDefinition[]
				{
					// Thanks to http://blog.stevenlevithan.com/archives/match-quoted-string
					new TokenDefinition(@"([""'])(?:\\\1|.)*?\1", "QUOTED-STRING"),
					// Thanks to http://www.regular-expressions.info/floatingpoint.html
					new TokenDefinition(@"[-+]?\d*\.\d+([eE][-+]?\d+)?", "FLOAT"),
					new TokenDefinition(@"[-+]?\d+", "INT"),
					new TokenDefinition(@"#t", "TRUE"),
					new TokenDefinition(@"#f", "FALSE"),
					new TokenDefinition(@"[*<>\?\-+/A-Za-z->!]+", "SYMBOL"),
					new TokenDefinition(@"\.", "DOT"),
					new TokenDefinition(@"\(", "LEFT"),
					new TokenDefinition(@"\)", "RIGHT"),
					new TokenDefinition(@"\s+", "SPACE")
				};
			
			    TextReader r = new StringReader(sample);
			    Lexer l = new Lexer(r, defs);
			    while (l.Next())
			    {
			        Console.WriteLine("Token: {0} Contents: '{1}'", l.Token, l.TokenContents);
			    }
					
		        Console.WriteLine("Finished.");
	    	}
	    	catch (Exception e)
	    	{
	    		Console.WriteLine("Error: {0}", e);
	    	}
    		Console.ReadLine();
	    }
	}
}