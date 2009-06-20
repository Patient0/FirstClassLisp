/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 19/06/2009
 * Time: 19:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;

using Lisp;
using System.IO;

namespace Lisp.Tests
{
	[TestFixture]
	public class LexerTests
	{
		private List<object> tokenize(string s, TokenDefinition[] defs)
		{
			List<object> tokens = new List<object>();
			using(Lexer l = new Lexer(new StringReader(s), defs))
			{
				while(l.Next())
				{
					Console.WriteLine("{0}: '{1}'",
					                  l.Token, l.TokenContents);
					tokens.Add(l.Token);
				}
			}
			return tokens;
		}
		
		private static List<object> l(params object[] items)
		{
			return new List<object>(items);
		}
		
		[Test]
		public void TestSimple()
		{
			var defs = new TokenDefinition[]
			{
				new TokenDefinition("one", "ONE"),
				new TokenDefinition("two", "TWO"),
				new TokenDefinition("three", "THREE")
			};
			Assert.AreEqual(l("ONE", "TWO", "THREE"), tokenize("onetwothree",defs));
		}
		
		[Test]
		public void TestMultiline()
		{
			var defs = new TokenDefinition[]
			{
				new TokenDefinition("one", "ONE"),
				new TokenDefinition("two", "TWO"),
				new TokenDefinition("three", "THREE"),
				new TokenDefinition("four", "FOUR"),
				new TokenDefinition("five", "FIVE"),
				new TokenDefinition("six", "SIX")
			};
			Assert.AreEqual(tokenize("onetwothree\r\nfourfivesix",defs), l("ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX"));
		}
		
		[Test]
		public void TestNull()
		{
			Assert.IsNull(new System.IO.StringReader("").ReadLine());
		}
		
		[Test]
		public void TestEmpty()
		{
			var defs = new TokenDefinition[]
			{
				new TokenDefinition("one", "ONE"),
				new TokenDefinition("two", "TWO"),
				new TokenDefinition("three", "THREE"),
				new TokenDefinition("four", "FOUR"),
				new TokenDefinition("five", "FIVE"),
				new TokenDefinition("six", "SIX")
			};
			Assert.AreEqual(l(), tokenize("",defs));
		}
		
		[Test]
		public void TestMultipleEmpty()
		{
			var defs = new TokenDefinition[]
			{
				new TokenDefinition("one", "ONE"),
				new TokenDefinition("two", "TWO"),
			};
			Assert.AreEqual(l(), tokenize("\r\n\r\n\r\n",defs));			
		}
		
		[Test]
		public void TestQuoted()
		{
			var defs = new TokenDefinition[]
			{
				new TokenDefinition(@""".*""", "STRING-LITERAL"),
				new TokenDefinition("one", "ONE"),
				new TokenDefinition("two", "TWO"),
				new TokenDefinition("\\(", "LEFT"),
				new TokenDefinition("\\)", "RIGHT"),
				new TokenDefinition("\\s", "SPACE")
			};
			Assert.AreEqual(l("SPACE", "STRING-LITERAL", "SPACE", "TWO", "SPACE", "ONE"),
			               tokenize(@" ""one"" two one", defs));
		}
		
		[Test]
		public void TestDoubleQuoted()
		{
			// Thanks heaps to http://blog.stevenlevithan.com/archives/match-quoted-string
			// for this brilliant Regex for quoted strings!
			var defs = new TokenDefinition[]
			{
				new TokenDefinition(@"([""'])(?:\\\1|.)*?\1", "STRING-LITERAL"),
				new TokenDefinition("one", "ONE"),
				new TokenDefinition("two", "TWO"),
				new TokenDefinition("\\(", "LEFT"),
				new TokenDefinition("\\)", "RIGHT"),
				new TokenDefinition("\\s", "SPACE")
			};
			
			
			Assert.AreEqual(l("STRING-LITERAL", "SPACE", "STRING-LITERAL"),
			               tokenize(@"""one"" ""three""", defs));
		}
		
		[Test]
		public void TestDoubleEscapeQuoted()
		{
			// Thanks heaps to http://blog.stevenlevithan.com/archives/match-quoted-string
			// for this brilliant Regex for quoted strings!
			var defs = new TokenDefinition[]
			{
				new TokenDefinition(@"([""'])(?:\\\1|.)*?\1", "STRING-LITERAL"),
				new TokenDefinition("one", "ONE"),
				new TokenDefinition("two", "TWO"),
				new TokenDefinition("\\(", "LEFT"),
				new TokenDefinition("\\)", "RIGHT"),
				new TokenDefinition("\\s", "SPACE")
			};
			
			using(Lexer l = new Lexer(new StringReader(@"""one \"" two"""), defs))
			{
				l.Next();
				Assert.AreEqual("STRING-LITERAL", l.Token);
				Assert.AreEqual(@"""one \"" two""", l.TokenContents);
			}
		}
		
		[Test]
		public void TestTypicalLisp()
		{
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
				new TokenDefinition(@"\s", "SPACE")
			};
			
			Assert.AreEqual(l("LEFT", "QUOTED-STRING", "SPACE",
			                  "FLOAT", "SPACE",
			                  "INT", "SPACE",
			                  "INT", "SPACE",
			                  "INT", "SPACE",
			                  "INT", "SPACE",
			                  "FLOAT", "SPACE",
			                  "FLOAT", "SPACE",
			                  "FLOAT", "SPACE",
			                  "FLOAT", "SPACE",
			                  "SYMBOL", "SPACE",
			                  "FALSE", "SPACE",
			                  "TRUE", "SPACE",
			                  "DOT", "RIGHT"),
			                tokenize(@"(""one \"" two"" -34.6 5 -5 -05 06 +.4 .3 2.5 0.3 string->set! #f #t .)", defs));
		}
	}
}