using System;
using System.IO;

namespace dpl {
  public class Lexeme {
    public string type;
    public int ival;
    public string sval;
    //real rval;
    public object[] aval;
    public Lexeme envVal;

    public Lexeme Left;
    public Lexeme Right;

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

    public Lexeme(string type, object[] aval) {
      this.type = type;
      this.aval = aval;
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
      } else if (type == "ARRAY") {
        var arr = "[";
        foreach (var item in aval) {
          arr += item + ", ";
        }
        arr = arr.Remove(arr.Length - 2) + "]";
        return arr;
      }
      throw new Exception("Invalid value type! Type: " + type);
    }

    public Lexeme GetEnvValue() {
      if (type == "ENV") {
        return envVal;
      }
      throw new Exception("Invalid type call to GetEnvValue()! Type: " + type);
    }

    public void SetValue(string val) {
      try {
        ival = Convert.ToInt32(val);
        type = "INTEGER";
        sval = "";
        aval = null;
      } catch {
        sval = val;
        type = "STRING";
        ival = 0;
        aval = null;
      }
    }
    public void SetArrayValue(object[] val) {
      aval = val;
      type = "ARRAY";
      ival = 0;
      sval = "";
    }
    public void SetEnvValue(Lexeme val) {
      type = "ENV";
      envVal = val;
    }
  }
}
