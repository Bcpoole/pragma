ifeq (run,$(firstword $(MAKECMDGOALS)))
  # use the rest as arguments for "run"
  RUN_ARGS := $(wordlist 2,$(words $(MAKECMDGOALS)),$(MAKECMDGOALS))
  # ...and turn them into do-nothing targets
  $(eval $(RUN_ARGS):;@:)
endif

build:
	mcs LexerScanner.cs Lexer.cs Lexeme.cs
	mcs RecognizerScanner.cs Recognizer.cs Lexer.cs Lexeme.cs
	mcs EnvironmentTest.cs Environment.cs Lexeme.cs ConsMethods.cs
	mcs EvaluatorScanner.cs Evaluator.cs Environment.cs Recognizer.cs Lexer.cs Lexeme.cs ConsMethods.cs

run:
	mono EvaluatorScanner.exe $(RUN_ARGS)

	
cat-error1:
	cat grammar.txt

run-error1:
	echo run-error1

cat-error2:
	echo cat-error2

run-error2:
	cat grammar.txt

cat-error3:
	cat grammar.txt

run-error3:
	cat grammar.txt

cat-arrays:
	cat grammar.txt

run-arrays:
	cat grammar.txt

cat-conditionals:
	cat grammar.txt

run-conditionals:
	cat grammar.txt

cat-recursion:
	cat grammar.txt

run-recursion:
	cat grammar.txt

cat-iteration:
	cat grammar.txt

run-iteration:
	cat grammar.txt

cat-functions:
	cat grammar.txt

run-functions:
	cat grammar.txt

clean:
	rm *.exe
