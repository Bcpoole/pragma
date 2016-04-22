using System;

namespace dpl {
  public class Environment {
    public Environment() {

    }

    public Node Create() {
      return Extend(null, null, null);
    }

    public string Lookup(string id, Node env) {
      while (env != null) {
        var table = Car(env);
        var vars = Car(table);
        var vals = Cdr(table);

        while (vars != null) {
          if (id == Car(vars).Value.sval) {
            return Car(vals).Value.GetValue();
          }
          vars = Cdr(vars);
          vals = Cdr(vars);
        }
        env = Cdr(env);
      }

      throw new Exception(String.Format("variable '{0}' is undefined!", id));
    }

    public void Update(string id, Node env, string val) {
      while (env != null) {
        var vars = Car(env);
        var vals = Cadr(env);
        while (vars != null) {
          if (id == Car(vars).Value.sval) {
            Car(vals).Value.SetValue(val);
            return;
          }
          vars = Cdr(vars);
          vals = Cdr(vars);
        }
        env = Cdr(Cdr(env));
      }

      throw new Exception(String.Format("variable '{0}' is undefined!", id));
    }

    public Node Insert(Node variable, Node val, Node env) {
      var table = Car(env);
      SetCar(table, Cons(CreateJOINNode(), variable, Car(table)));
      SetCdr(table, Cons(CreateJOINNode(), val, Cdr(table)));
      return val;
    }

    public Node Extend(Node vars, Node vals, Node env) {
      return Cons(env, vars,
          Cons(CreateJOINNode(), vals, Cons(
              CreateJOINNode(), env, null)));
    }

    //cons functions
    public Node Cons(Node val, Node left, Node right) {
      val.Left = left;
      val.Right = right;

      return val;
    }

    public Node Car(Node cons) {
      return cons.Left;
    }
    public void SetCar(Node cons, Node val) {
      cons.Left = val;
    }

    public Node Cdr(Node cons) {
      return cons.Right;
    }
    public void SetCdr(Node cons, Node val) {
      cons.Right = val;
    }

    public Node Cadr(Node cons) {
      return cons.Right.Left;
    }

    public Node Caddr(Node cons) {
      return cons.Right.Right.Left;
    }

    private Node CreateJOINNode() {
      return new Node(new Lexeme("JOIN"));
    }
  }
}
