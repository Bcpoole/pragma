using System;
using System.IO;
using System.Collections.Generic;

namespace dpl {
  public class EnvironmentTest {
    static public int Main() {
      Lexeme env = null;

      Console.WriteLine("Creating a new environment");
      env = Environment.Create();
      Environment.Print(env);

      Console.WriteLine("Adding variable x with value 3");
      Lexeme vari1 = new Lexeme("ID", "x");
      Lexeme val1 = new Lexeme("INTEGER", 3);
      Environment.Insert(vari1, val1, env);
      Environment.Print(env);

      Console.WriteLine("Extending the environment with y:4 and z:\"hello\"");
      var childEnv = Environment.Create();

      Lexeme vari2 = new Lexeme("ID", "y");
      Lexeme val2 = new Lexeme("INTEGER", 4);
      Environment.Insert(vari2, val2, childEnv);

      Lexeme vari3 = new Lexeme("ID", "z");
      Lexeme val3 = new Lexeme("STRING", "hello");

      vari2.Right = vari3;
      val2.Right = val3;
      Environment.Insert(vari3, val3, childEnv);

      env = Environment.Extend(childEnv.Left.Left, childEnv.Left.Right, env); //something not right here... (tried with childEnv, don't think I need that variable though)
      Console.WriteLine("The local environment is:");
      Environment.Print(env);
      Console.WriteLine("The environment is:");
      Environment.Print(Environment.GetParent(env));

      return 0;
    }
  }
}