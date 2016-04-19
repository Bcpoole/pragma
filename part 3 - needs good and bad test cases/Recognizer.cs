using System;
using System.Collections.Generic;

namespace dpl {
  public class Recognizer {
    List<Lexeme> tokens;
    Lexeme CurrentLexeme {
      get { return tokens[0]; }
    }
    
    public Recognizer(List<Lexeme> tokens) {
      this.tokens = tokens;
      
      parse();
    }
    
    void parse() {
      program();
      match("END_OF_FILE");
    }
    
    //support functions
    bool check(string type) {
      return CurrentLexeme.type == type;
    }
    
    void advance() {
      tokens.RemoveAt(0);
    }
    
    Lexeme match(string type) {
      matchNoAdvance(type);
      var matchedLex = CurrentLexeme;
      advance();
      return matchedLex;
    }
    
    void matchNoAdvance(string type) {
      if (!check(type)) {
        throw new System.Exception(String.Format("Syntax error!\n" +
        "\tCurrentLexeme type : {0}\n" +
        "\tExpecting type : {1}" , CurrentLexeme.type, type));
      }
    }
    
    bool checkAhead(string type, int i) {
      if (tokens.Count < i + 1 || tokens[i].type != type) {
        return false;
      } else {
        return true;
      }
    }
    
    //Grammar Rules
    void program() {
      optStatementList();
    }
    
    void funcDef() {
      match("ID");
      match("OPAREN");
      optParamList();
      match("CPAREN");
      block();
    }
    bool funcDefPending() {
      bool isFuncDefPending = false;
      if (check("ID") && checkAhead("OPAREN", 1)) {
        int i = 2;
        while (i != tokens.Count && (tokens[i].type != "OBRACE" && tokens[i].type != "SEMI")) {
          i++;
        }
        if (checkAhead("OBRACE", i)) {
          isFuncDefPending = true;
        }
      }
      return isFuncDefPending;
    }
    
    void optParamList() {
      if (!check("CPAREN")) {
        paramList();
      }
    }
    
    void paramList() {
      expr();
      if (check("COMMA")) {
        match("COMMA");
        paramList();
      }
    }
    
    void optArgList() {
      if (!check("CPAREN")) {
        argList();
      }
    }
    
    void argList() {
      expr();
      if (check("COMMA")) {
        match("COMMA");
        argList();
      }
    }
    
    void expr() {
      primary();
      if (opPending()) {
        op();
        expr();
        optExprList();
      }
    }
    bool exprPending() {
      return primaryPending();
    }
    
    void optExprList() {
      if (check("AND")) {
        match("AND");
        expr();
      } else if (check("OR")) {
        match("OR");
        expr();
      }
    }
    
    void primary() {
      if (check("ID")) {
        match("ID");
        if (check("OPAREN")) {
          match("OPAREN");
          optArgList();
          match("CPAREN");
        }
      } else if (check("STRING")) {
        match("STRING");
      } else if (check("INTEGER")) {
        match("INTEGER");
      } else if (lambdaPending()) {
        lambda();
      } else if (funcDefPending()) {
        funcDef();
      } else if (check("MINUS")) {
        match("MINUS");
        match("INTEGER");
      } else if (dictionaryPending()) {
        dictionary();
      } else if (optArrayPending()) {
        optArray();
      }
    }
    bool primaryPending() {
      return check("ID") || check("STRING") || check("INTEGER") || check("OPAREN") ||
        lambdaPending() || funcDefPending() || check("MINUS") ||
        optArrayPending() || dictionaryPending();
    }
    
    void block() {
      match("OBRACE");
      optStatementList();
      match("CBRACE");
    }
    
    void optStatementList() {
      if (!check("CBRACE")) {
        statementList();
      }
    }
    bool optStatementListPending() {
      return (!check("CBRACE"));
    }
    
    void statementList() {
      statement();
      if (statementListPending()) {
        statementList();
      }
    }
    bool statementListPending() {
      return statementPending();
    }
    
    void statement() {
      if (funcDefPending()) {
        funcDef();
      } else if (varDefPending()) {
        varDef();
        match("SEMI");
      }
      else if (whileLoopPending()) {
        whileLoop();
      } else if (exprPending()) {
        expr();
        match("SEMI");
      } else if (ifStatementPending()) {
        ifStatement();
      } else if (check("RETURN")) {
        match("RETURN");
        expr();
        match("SEMI");
      }
    }
    bool statementPending() {
      return funcDefPending() || whileLoopPending() || exprPending() ||
      ifStatementPending() || check("RETURN") || varDefPending();
    }
    
    void op() {
      if (check("PLUS")) {
        match("PLUS");
      } else if (check("INCREMENT")) {
        match("INCREMENT");
      } else if (check("PLUS_TO")) {
        match("PLUS_TO");
      } else if (check("MINUS")) {
        match("MINUS");
      } else if (check("DECREMENT")) {
        match("DECREMENT");
      } else if (check("MINUS_TO")) {
        match("MINUS_TO");
      } else if (check("TIMES")) {
        match("TIMES");
      } else if (check("TIMES_TO")) {
        match("TIMES_TO");
      } else if (check("DIVIDES")) {
        match("DIVIDES");
      } else if (check("DIVIDES_TO")) {
        match("DIVIDES_TO");
      } else if (check("MOD")) {
        match("MOD");
      } else if (check("MOD_TO")) {
        match("MOD_TO");
      } else if (check("EXPN")) {
        match("EXPN");
      } else if (check("EXPN_TO")) {
        match("EXPN_TO");
      } else if (check("ASSIGN")) {
        match("ASSIGN");
      } else if (check("EQUAL")) {
        match("EQUAL");
      } else if (check("NOT_EQUAL")) {
        match("NOT_EQUAL");
      } else if (check("LESSTHAN")) {
        match("LESSTHAN");
      } else if (check("LESSTHAN_EQUALTO")) {
        match("LESSTHAN_EQUALTO");
      } else if (check("GREATERTHAN")) {
        match("GREATERTHAN");
      } else if (check("GREATERTHAN_EQUALTO")) {
        match("GREATERTHAN_EQUALTO");
      }
    }
    bool opPending() {
      return check("PLUS") || check("INCREMENT") || check("PLUS_TO") ||
        check("MINUS") || check("DECREMENT") || check("MINUS_TO") ||
        check("TIMES") || check("TIMES_TO") ||
        check("DIVIDES") || check("DIVIDES_TO") ||
        check("MOD") || check("MOD_TO") ||
        check("EXPN") || check("EXPN_TO") ||
        check("ASSIGN") ||
        check("EQUAL") || check("NOT_EQUAL") ||
        check("LESSTHAN") || check("LESSTHAN_EQUALTO") ||
        check("GREATERTHAN") || check("GREATERTHAN_EQUALTO");
    }
    
    void whileLoop() {
      match("WHILE");
      match("OPAREN");
      expr();
      match("CPAREN");
      block();
    }
    bool whileLoopPending() {
      return check("WHILE");
    }
    
    void ifStatement() {
      match("IF");
      match("OPAREN");
      expr();
      match("CPAREN");
      block();
      optElif();
    }
    bool ifStatementPending() {
      return check("IF");
    }
    
    void optElif() {
      if (check("ELIF")) {
        match("ELIF");
        match("OPAREN");
        expr();
        match("CPAREN");
        block();
        optElif();
      } else {
        optElse();
      }
    }
    
    void optElse() {
      if (check("ELSE")) {
        match("ELSE");
        block();
      }
    }
    
    void varDef() {
      match("ID");
      optAssign();
    }
    bool varDefPending() {
      return check("ID") && (checkAhead("SEMI",1) || checkAhead("ASSIGN",1));
    }
    
    void optAssign() {
      if (check("ASSIGN")) {
        match("ASSIGN");
        expr();
      }
    }
    
    void optArray() {
      match("OBRACKET");
      if (arrayPending()) {
        array();
      }
      match("CBRACKET");
    }
    bool optArrayPending() {
      return check("OBRACKET");
    }
    
    void array() {
      primary();
      if (check("COMMA")) {
        match("COMMA");
        array();
      }
    }
    bool arrayPending() {
      return primaryPending();
    }
    
    void lambda() {
      match("OPAREN");
      optArgList();
      match("CPAREN");
      match("LAMBDA_DEF");
      block();
    }
    bool lambdaPending() {
      bool isLambdaPending = false;
      if (check("OPAREN")) {
        int i = 1;
        while (tokens.Count != i && tokens[i].type != "CPAREN") {
          i++;
        }
        if (tokens[i+1].type == "LAMBDA_DEF") {
          isLambdaPending = true;
        }
      }
      return isLambdaPending;
    }
    
    void dictionary() {
      match("OBRACE");
      optDictList();
      match("CBRACE");
    }
    bool dictionaryPending() {
      return check("OBRACE");
    }
    
    void optDictList() {
      if (check("STRING")) {
        dictList();
      }
    }
    
    void dictList() {
      match("STRING");
      match("COLON");
      primary();
      if (check("COMMA")) {
        match("COMMA");
        dictList();
      }
    }
  }
}
