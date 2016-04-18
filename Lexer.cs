using System;
using System.IO;

namespace dpl { 
  public class Lexer {
    const char EOF = '▌';
    private int lineNumber = 1;
    private string program = "";
    
    public Lexer(string filename) {
      using (var sr = new StreamReader(filename)) {
        while (sr.Peek() >= 0) {
          program += ((char)sr.Read());
        }
        program += EOF; //represents EOF character
      }
    }
    

    char getCharacter() {
      var ch = program[0];
      if (ch == '\n') {
        lineNumber++;
      }
      program = program.Remove(0,1);
      return ch;
    }
    
    char peekCharacter() {
      return program[0];
    }
    
    char peekPeekCharacter() {
      return program[1];
    }
    
    void pushbackCharacter(char ch) {
      program = ch + program;
    }
    
    void skipWhiteSpace() {
      if (!Char.IsWhiteSpace(peekCharacter()) && peekCharacter() != '/') {
        return;
      }
      var ch = getCharacter();
      char nextCh;
      
      if (ch == '/') {
        nextCh = getCharacter();
        if (nextCh == '/') { //line comment
          while (ch != '\n' && ch != EOF) {
            ch = getCharacter();
          }
          if (ch == EOF) {
            pushbackCharacter(ch);
          }
        } else if (nextCh == '*') { //block comment
          while (ch != '*' && peekCharacter() != '/') {
            ch = getCharacter();
          }
          getCharacter(); //get / of block comment
          ch = getCharacter(); //get past block comment
        } else { //just a / character
          pushbackCharacter(nextCh);
        }
      }
      else if (Char.IsWhiteSpace(ch)) { //skip whitespace, should handle tabs and similiar
        while (Char.IsWhiteSpace(ch)) {
          ch = getCharacter();
        }
        pushbackCharacter(ch);
      }
      
      if (Char.IsWhiteSpace(ch) || (peekCharacter() == '/' && peekPeekCharacter() == '*')) {
        skipWhiteSpace();
      }
    }
    
    public Lexeme lex() {
      skipWhiteSpace();
      var ch = getCharacter();
      char nextCh;
      
      switch (ch) {
        //single symbols
        case EOF:
          return new Lexeme("END_OF_FILE");
        case '(':
          return new Lexeme("OPAREN");
        case ')':
          return new Lexeme("CPAREN");
        case '{':
          return new Lexeme("OBRACE");
        case '}':
          return new Lexeme("CBRACE");
        case '[':
          return new Lexeme("OBRACKET");
        case ']':
          return new Lexeme("CBRACKET");
        case ';':
          return new Lexeme("SEMI");
        case ':':
          return new Lexeme("COLON");
        case ',':
          return new Lexeme("COMMA");
        //operations
        case '+':
          nextCh = getCharacter();
          if (nextCh == '+') { // ++
            return new Lexeme("INCREMENT");
          } else if (nextCh == '=') { // +=
            return new Lexeme("PLUS_TO");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("PLUS");
          }
        case '*':
          nextCh = getCharacter();
          if (nextCh == '=') { // *=
            return new Lexeme("TIMES_TO");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("TIMES");
          }
        case '-':
          nextCh = getCharacter();
          if (nextCh == '-') { // --
            return new Lexeme("DECREMENT");
          } else if (nextCh == '=') { // -=
            return new Lexeme("MINUS_TO");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("MINUS");
          }
        case '/':
          nextCh = getCharacter();
          if (nextCh == '/') { // //
            pushbackCharacter(nextCh);
            pushbackCharacter(ch);
            skipWhiteSpace();
            return lex();
          }
          else if (nextCh == '=') { // /=
            return new Lexeme("DIVIDES_TO");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("DIVIDES");
          }
        case '%':
        nextCh = getCharacter();
          if (nextCh == '=') { // %=
            return new Lexeme("MOD_TO");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("MOD");
          }
        case '^':
        nextCh = getCharacter();
          if (nextCh == '=') { // ^=
            return new Lexeme("EXPN_TO");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("EXPN");
          }
        //comparisons and assign
        case '<':
          nextCh = getCharacter();
          if (nextCh == '=') { // <=
            return new Lexeme("LESSTHAN_EQUALTO");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("LESSTHAN");
          }
        case '>':
          nextCh = getCharacter();
          if (nextCh == '=') { // >=
            return new Lexeme("GREATERTHAN_EQUALTO");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("GREATERTHAN");
          }
        case '=':
          nextCh = getCharacter();
          if (nextCh == '=') { // ==
            return new Lexeme("EQUAL");
          }
          else if (nextCh == '>') { // =>
            return new Lexeme("LAMBDA_DEF");
          } else {
            pushbackCharacter(nextCh);
            return new Lexeme("ASSIGN");
          }
        case '!':
          nextCh = getCharacter();
          if (nextCh == '=') { // ==
            return new Lexeme("NOT_EQUAL");
          } else {
            pushbackCharacter(nextCh);
            Console.WriteLine("Error with Character: '" + ch + "'");
            throw new System.Exception(String.Format("Error! Invalid Lexeme on line {0}", lineNumber));
          }
        case '|':
          nextCh = getCharacter();
          if (nextCh != '|') {
            pushbackCharacter(nextCh);
          }
          return new Lexeme("OR");// | and ||
        case '&':
          nextCh = getCharacter();
          if (nextCh != '&') {
            pushbackCharacter(nextCh);
          }
          return new Lexeme("AND");// & and &&
        //numbers, variables/keywords, and strings
        default:
          if (Char.IsDigit(ch)) { //number
            return lexNumber(ch);
          }
          else if (Char.IsLetter(ch)) { //variable/keyword
            return lexWord(ch);
          }
          else if (ch == '"') { //string
            //pushbackCharacter(ch);
            return lexString();
          }
          else {
            Console.WriteLine("Error with Character: '" + ch + "'");
            throw new System.Exception(String.Format("Error! Invalid Lexeme on line {0}", lineNumber));
          }
      }
    }

    Lexeme lexString() {
      var buffer = "";
      var ch = getCharacter();
      while (ch != '"') {
        if (ch == '\\') {
          ch = getCharacter();
        }
        buffer += ch;
        ch = getCharacter();
      }
      
      return new Lexeme("STRING", buffer);
    }
    
    Lexeme lexNumber(char ch) {
      var buffer = "";
      while (Char.IsDigit(ch)) {
        buffer += ch;
        ch = getCharacter();
      }
      pushbackCharacter(ch);
      return new Lexeme("INTEGER", Int32.Parse(buffer));
    }
    
    //variables and keywords
    Lexeme lexWord(char ch) {
      var buffer = "" + ch;
      ch = getCharacter();
      while (Char.IsDigit(ch) || Char.IsLetter(ch) || ch == '_') {
        buffer += ch;
        ch = getCharacter();
      }
      pushbackCharacter(ch);
      
      switch (buffer) {
        case "while": return new Lexeme("WHILE");
        case "if": return new Lexeme("IF");
        case "elif": return new Lexeme("ELIF");
        case "else": return new Lexeme("ELSE");
        case "return": return new Lexeme("RETURN");
        default: return new Lexeme("ID", buffer);
      }
    }
  }
}
