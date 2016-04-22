using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpl {
  static class ConsMethods {
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
  }
}
