using System;
using System.IO;

namespace dpl {
  public class Lexeme {
    public string type;
    public int ival;
    public string sval;
    //real rval;

    public Lexeme(string type) {
      this.type = type;
    }

    public Lexeme(string type, int ival) {
      this.type = type;
      this.ival = ival;
    }

    public Lexeme(string type, string sval) {
      this.type = type;
      this.sval = sval;
    }

    public void Print() {
      switch (type) {
        case "STRING":
          Console.WriteLine(String.Format("type : {0} , value {1}", type, sval));
          break;
        case "INTEGER":
          Console.WriteLine(String.Format("type : {0} , value {1}", type, ival));
          break;
        default:
          Console.WriteLine(String.Format("type : {0}", type));
          break;
      }
    }

    public string GetValue() {
      if (type == "STRING") {
        return sval;
      }
      else if (type == "INTEGER") {
        return ival.ToString();
      }
      throw new Exception("Invalid value type");
    }

    public void SetValue(string val) {
      if (type == "STRING") {
        sval = val;
      }
      else if (type == "INTEGER") {
        ival = Convert.ToInt32(val);
      }
      else {
        throw new Exception("Invalid value type");
      }
    }
  }
}
