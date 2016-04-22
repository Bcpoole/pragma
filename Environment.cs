using System;

namespace dpl {
  public static class Environment {
    public static Lexeme Create() {
      return Extend(null, null, null);
    }

    public static string Lookup(string id, Lexeme env) {
      while (env != null) {
        var table = Car(env);
        var vars = Car(table);
        var vals = Cdr(table);

        while (vars != null) {
          if (id == Car(vars).sval) {
            return Car(vals).GetValue();
          }
          vars = Cdr(vars);
          vals = Cdr(vals);
        }
        env = Cdr(env);
      }

      throw new Exception(String.Format("variable '{0}' is undefined!", id));
    }

    public static void Update(string id, Lexeme env, string val) {
      while (env != null) {
        var table = Car(env);
        var vars = Car(table);
        var vals = Cdr(table);

        while (vars != null) {
          if (id == Car(vars).sval) {
            Car(vals).SetValue(val);
            return;
          }
          vars = Cdr(vars);
          vals = Cdr(vals);
        }
        env = Cdr(env);
      }

      throw new Exception(String.Format("variable '{0}' is undefined!", id));
    }

    public static Lexeme Insert(Lexeme variable, Lexeme val, Lexeme env) {
      var table = Car(env);
      SetCar(table, Cons(CreateJOINLexeme(), variable, Car(table)));
      SetCdr(table, Cons(CreateJOINLexeme(), val, Cdr(table)));
      return env;
    }

    public static Lexeme Extend(Lexeme vars, Lexeme vals, Lexeme env) {
      return Cons(new Lexeme("ENV"),
        Cons(new Lexeme("VALUES"), vars, vals),
        env);
    }

    public static Lexeme GetParent(Lexeme env) {
      return Cdr(env);
    }

    public static void Print(Lexeme env) {
      if (env != null) {
        var table = Car(env);
        var vars = Car(table);
        var vals = Cdr(table);

        while (vars != null) {
          if (Car(vals).type == "STRING") {
            Console.WriteLine(String.Format("\t{0} : '{1}'", Car(vars).sval, Car(vals).GetValue()));
          }
          else {
            Console.WriteLine(String.Format("\t{0} : {1}", Car(vars).sval, Car(vals).GetValue()));
          }

          vars = Cdr(vars);
          vals = Cdr(vals);
        }
      }
    }
    public static void PrintAll(Lexeme env) {
      int depth = 0;
      while (env != null) {
        var table = Car(env);
        var vars = Car(table);
        var vals = Cdr(table);

        if (depth > 0) {
          Console.WriteLine("\n---Parent Environment---");
        }
        Console.WriteLine("The environment is:");
        while (vars != null) {
          Console.WriteLine(String.Format("\t{0} : {1}", Car(vars).sval, Car(vals).GetValue()));
          
          vars = Cdr(vars);
          vals = Cdr(vals);
        }
        env = Cdr(env);
        depth++;
      }
    }

    //cons functions
    public static Lexeme Cons(Lexeme val, Lexeme left, Lexeme right) {
      val.Left = left;
      val.Right = right;

      return val;
    }

    public static Lexeme Car(Lexeme cons) {
      return cons.Left;
    }
    public static void SetCar(Lexeme cons, Lexeme val) {
      cons.Left = val;
    }

    public static Lexeme Cdr(Lexeme cons) {
      return cons.Right;
    }
    public static void SetCdr(Lexeme cons, Lexeme val) {
      cons.Right = val;
    }

    public static Lexeme Cadr(Lexeme cons) {
      return cons.Right.Left;
    }

    public static Lexeme Caddr(Lexeme cons) {
      return cons.Right.Right.Left;
    }

    private static Lexeme CreateJOINLexeme() {
      return new Lexeme("JOIN");
    }
  }
}
