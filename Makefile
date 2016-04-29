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

clean:
	rm *.exe
