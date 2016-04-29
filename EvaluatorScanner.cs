using System;
using System.IO;
using System.Collections.Generic;
using static dpl.Environment;
using static dpl.Evaluator;

namespace dpl {
  public class EvaluatorScanner {
    static public int Main(string[] args) {
      if (args.Length < 1) {
        Console.WriteLine("Error! Too few arguments! Provide a single filename for the Lexer!");
        return -1;
      }
      else if (args.Length > 1) {
        Console.WriteLine("Error! Too many arguments! Provide a single filename for the Lexer!");
        return -1;
      }

      var filename = args[0];
      if (!File.Exists(filename)) {
        Console.WriteLine(String.Format("Error! File '{0}' does not exist!", filename));
        return -1;
      }
      scanner(filename);

      return 0;
    }

    static void scanner(string filename) {
      var tokens = new List<Lexeme>();
      Lexeme token;
      var i = new Lexer(filename);
      token = i.lex();
      while (token.type != "END_OF_FILE") {
        tokens.Add(token);
        token = i.lex();
      }
      tokens.Add(token);

      var pt = new Recognizer(tokens).ProgramTree;
      var env = Create();
      Eval(pt, env);
    }
  }
}
