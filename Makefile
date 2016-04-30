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
	cat Programs/errorProgram1.prag

run-error1:
	recognizer Programs/errorProgram1.prag

cat-error2:
	cat Programs/errorProgram2.prag

run-error2:
	recognizer Programs/errorProgram2.prag

cat-error3:
	cat Programs/errorProgram3.prag

run-error3:
	recognizer Programs/errorProgram3.prag

cat-arrays:
	cat Programs/arrays.prag

run-arrays:
	evaluator Programs/arrays.prag;

cat-conditionals:
	cat Programs/conditionals.prag

run-conditionals:
	evaluator Programs/conditionals.prag

cat-recursion:
	cat Programs/recursion.prag

run-recursion:
	evaluator Programs/recursion.prag

cat-iteration:
	cat Programs/iteration.prag

run-iteration:
	evaluator Programs/iteration.prag

cat-functions:
	cat Programs/functions.prag

run-functions:
	evaluator Programs/functions.prag
	
cat-problem:
	cat Programs/dictionary.prag
	
run-problem:
	printf "I had a strange problem calling an object's methods that I was unable to solve to no working program for this :("

clean:
	rm *.exe
